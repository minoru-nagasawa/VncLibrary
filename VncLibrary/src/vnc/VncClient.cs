using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using OpenCvSharp;
using System.IO;

namespace VncLibrary
{
    public class VncClient : IVncClient
    {
        #region for IDisposable
        bool m_disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool a_disposing)
        {
            if (m_disposed)
            {
                return;
            }

            if (a_disposing)
            {
                m_readStream?.Dispose();  m_readStream  = null;
                m_writeStream?.Dispose(); m_writeStream = null;
                m_tcpClient?.Dispose();   m_tcpClient   = null;
                m_canvas?.Dispose();      m_canvas      = null;
            }
            m_disposed = true;
        }
        #endregion

        public bool Connecting
        {
            get;
            private set;
        }
        public bool Connected
        {
            get
            {
                if (m_tcpClient == null)
                {
                    return false;
                }
                if (!m_tcpClient.Connected)
                {
                    return false;
                }
                return m_connected;
            }
        }
        public bool Disconnected
        {
            get
            {
                return !Connecting && !Connected;
            }
        }
        public VncConfig ClientConfig
        {
            get;
            private set;
        }

        public VncServerInitBody ServerInitBody
        {
            get
            {
                return m_serverInitBody;
            }
        }

        public Mat InternalCanvas
        {
            get
            {
                return m_canvas;
            }
        }
        public object CanvasLock
        {
            get;
            private set;
        }

        public event VncSimpleEventHandler ConnectedEvent;
        protected void onConnected()
        {
            ConnectedEvent?.Invoke(this, EventArgs.Empty);
        }

        public event VncReadEventHandler ReadEvent;
        protected void onRead(VncReadEventArgs a_args)
        {
            ReadEvent?.Invoke(this, a_args);
        }

        public event VncCauseEventHandler ConnectFailedEvent;
        protected void onConnectFailed(VncCauseEventArgs a_args)
        {
            ConnectFailedEvent?.Invoke(this, a_args);
        }

        public event VncCauseEventHandler DisconnectedEvent;
        protected void onDisconnected(VncCauseEventArgs a_args)
        {
            DisconnectedEvent?.Invoke(this, a_args);
        }

        private Func<Socket, Stream> m_readStreamCreator;
        private Func<Socket, Stream> m_writeStreamCreator;
        private TcpClient            m_tcpClient;
        private Stream               m_readStream;
        private Stream               m_writeStream;

        private VncServerInitBody   m_serverInitBody;
        private bool                m_readingServerMessage;
        private bool                m_connected;
        private IVncPixelGetter     m_pixelGetterNormal;
        private IVncPixelGetter     m_pixelGetterZrle;
        private MatOfByte3          m_canvas;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_clientConfig"></param>
        /// <param name="a_streamCreator"></param>
        /// <example>
        /// var config = new VncConfig("192.168.1.1", 5900, "password", true);
        /// var client = new VncClient(config, (s) => new BufferedStream(new NetworkStream(s)), (s) => new NetworkStream(s));
        /// </example>
        public VncClient(VncConfig a_clientConfig, Func<Socket, Stream> a_readStreamCreator, Func<Socket, Stream> a_writeStreamCreator)
        {
            ClientConfig         = a_clientConfig;
            m_readStreamCreator  = a_readStreamCreator;
            m_writeStreamCreator = a_writeStreamCreator;
            CanvasLock           = new object();
        }

        public void DisconnectVnc()
        {
            m_readStream?.Dispose();  m_readStream  = null;
            m_writeStream?.Dispose(); m_writeStream = null;
            m_tcpClient?.Dispose();   m_tcpClient = null;
        }

        public bool ConnectVnc()
        {
            Connecting = true;

            try
            {
                m_tcpClient = new TcpClient();
                m_tcpClient.Connect(ClientConfig.Address, ClientConfig.Port);

                m_readStream  = m_readStreamCreator(m_tcpClient.Client);
                m_writeStream = m_writeStreamCreator(m_tcpClient.Client);

                //-----------------------
                // Handshake
                //-----------------------
                // Server -> (ProtocolVersion) -> Client
                var version = VncComm.ReadProtocolVersion(m_readStream, ClientConfig.ForceVersion);

                // Server <- (ProtocolVersion) <- Client
                VncComm.WriteProtocolVersion(m_writeStream, version);

                // (Security)
                if (version == VncEnum.Version.Version33)
                {
                    // Server -> (Security) -> Client
                    var securityType = VncComm.ReadSecurityType(m_readStream);
                    if (securityType != VncEnum.SecurityType.None
                    &&  securityType != VncEnum.SecurityType.VNCAuthentication)
                    {
                        throw new SecurityException($"VNC Version is 3.3. Security type is {securityType}.");
                    }

                    if (securityType == VncEnum.SecurityType.VNCAuthentication)
                    {
                        // Server -> (VNC Authentication Challenge) -> Client
                        var challenge = VncComm.ReadVncChallange(m_readStream);

                        // Server <- (VNC Authentication Response) <- Client
                        byte[] response = encryptChallenge(ClientConfig.Password, challenge);
                        VncComm.WriteVncResponse(m_writeStream, response);

                        // Server -> (Security Result) -> Client
                        VncComm.ReadSecurityResult(m_readStream, version); // Result is checked in method. So don't check here, 
                    }
                }
                else
                {
                    // Server -> (SecurityTypes) -> Client
                    var securityTypes = VncComm.ReadSecurityTypes(m_readStream);

                    if (securityTypes.Contains(VncEnum.SecurityType.None))
                    {
                        // Server <- (SecurityType) <- Client
                        VncComm.WriteSecurityType(m_writeStream, VncEnum.SecurityType.None);
                    }
                    else if (securityTypes.Contains(VncEnum.SecurityType.VNCAuthentication))
                    {
                        // Server <- (SecurityType) <- Client
                        VncComm.WriteSecurityType(m_writeStream, VncEnum.SecurityType.VNCAuthentication);

                        // Server -> (VNC Authentication Challenge) -> Client
                        var challenge = VncComm.ReadVncChallange(m_readStream);

                        // Server <- (VNC Authentication Response) <- Client
                        byte[] response = encryptChallenge(ClientConfig.Password, challenge);
                        VncComm.WriteVncResponse(m_writeStream, response);
                    }
                    else
                    {
                        throw new SecurityException($"Unknown security-types. Server can use [{string.Join(",", securityTypes)}].");
                    }

                    // Server -> (Security Result) -> Client
                    VncComm.ReadSecurityResult(m_readStream, version);
                }

                //-----------------------
                // Initial Message
                //-----------------------
                // Server <- (ClientInit) <- Client
                VncComm.WriteClientInit(m_writeStream, VncEnum.SharedFlag.Share);

                // Server -> (ServerInit) -> Client
                m_serverInitBody = VncComm.ReadServerInit(m_readStream);

                //-----------------------
                // InitialSettings
                //-----------------------
                // Server <- (SetEncodings) <- Client
                var encodings = new VncEnum.EncodeType[] {  VncEnum.EncodeType.Raw,
                                                            VncEnum.EncodeType.CopyRect,
                                                            VncEnum.EncodeType.RRE,
                                                            VncEnum.EncodeType.Hextile,
                                                            VncEnum.EncodeType.ZRLE,
                                                         };
                VncComm.WriteSetEncodings(m_writeStream, encodings);

                //-----------------------
                // Refresh Framebuffer
                //-----------------------
                // Server <- (Refresh Framebuffer) <- Client
                VncComm.WriteFramebufferUpdateRequest(m_writeStream,
                                                      VncEnum.FramebufferUpdateRequestIncremental.UpdateAll,
                                                      0,
                                                      0,
                                                      m_serverInitBody.FramebufferWidth,
                                                      m_serverInitBody.FramebufferHeight);
                
                // Create Data
                m_pixelGetterNormal = VncPixelGetterFactory.CreateVncPixelGetter(m_serverInitBody.ServerPixelFormat, VncEnum.EncodeType.Raw);
                m_pixelGetterZrle   = VncPixelGetterFactory.CreateVncPixelGetter(m_serverInitBody.ServerPixelFormat, VncEnum.EncodeType.ZRLE);

                m_canvas?.Dispose();
                m_canvas = new MatOfByte3(m_serverInitBody.FramebufferHeight, m_serverInitBody.FramebufferWidth);

                // Successful connection
                m_connected = true;
                onConnected();
            }
            catch (Exception a_ex)
            {
                cleanupForDisconnect(a_ex);
                onConnectFailed(new VncCauseEventArgs(a_ex));
            }
            finally
            {
                Connecting = false;
            }

            return m_connected;
        }

        public void WriteFramebufferUpdateRequest()
        {
            try
            {
                VncComm.WriteFramebufferUpdateRequest(m_writeStream,
                                                      VncEnum.FramebufferUpdateRequestIncremental.Incremental,
                                                      0,
                                                      0,
                                                      m_serverInitBody.FramebufferWidth,
                                                      m_serverInitBody.FramebufferHeight);
            }
            catch (Exception a_ex)
            {
                cleanupForDisconnect(a_ex);
                onDisconnected(new VncCauseEventArgs(a_ex));
            }
        }

        public VncReadMessageBody ReadServerMessage()
        {
            if (m_readingServerMessage)
            {
                return null;
            }
            
            m_readingServerMessage = true;
            try
            {
                var readBody = VncComm.ReadServerMessage(m_readStream,
                                                         m_serverInitBody.ServerPixelFormat.BytesPerPixel,
                                                         m_serverInitBody.ServerPixelFormat.BigEndianFlag);

                // Set after WriteFramebufferUpdateRequest to prevent the excess data
                VncReadMessageBody retBody = null;
                var messageType = (VncEnum.MessageTypeServerToClient)readBody[0];
                if (messageType == VncEnum.MessageTypeServerToClient.FramebufferUpdate)
                {
                    var encodeList = VncEncodeFactory.CreateVncEncodeFromBinary(readBody,
                                                                                m_serverInitBody.ServerPixelFormat.BytesPerPixel,
                                                                                m_serverInitBody.ServerPixelFormat.BigEndianFlag);
                    
                    // Draw to canvas
                    lock (CanvasLock)
                    {
                        foreach (var e in encodeList)
                        {
                            // Draw to canvas
                            IVncPixelGetter pixelGetter = (e.EncodeType == VncEnum.EncodeType.ZRLE) ? m_pixelGetterZrle : m_pixelGetterNormal;
                            e.Draw(pixelGetter, m_canvas);
                        }
                    }

                    retBody = new VncReadMessageBody(messageType, encodeList, null);
                }
                else if (messageType == VncEnum.MessageTypeServerToClient.SetColorMapEntries)
                {
                    var colorMap = new VncSetColorMapEntriesBody(readBody);
                    m_pixelGetterNormal.SetColorMap(colorMap);
                    retBody = new VncReadMessageBody(messageType, null, colorMap);
                }

                onRead(new VncReadEventArgs(messageType, retBody));
                return retBody;
            }
            catch (Exception a_ex)
            {
                cleanupForDisconnect(a_ex);
                onDisconnected(new VncCauseEventArgs(a_ex));
                return null;
            }
            finally
            {
                m_readingServerMessage = false;
            }
        }

        public void WriteKeyEvent(VncEnum.KeyEventDownFlag a_downFlag, UInt32 a_key)
        {
            try
            {
                VncComm.WriteKeyEvent(m_writeStream, a_downFlag, a_key);
            }
            catch (Exception a_ex)
            {
                cleanupForDisconnect(a_ex);
                onDisconnected(new VncCauseEventArgs(a_ex));
            }
        }

        public void WritePointerEvent(VncEnum.PointerEventButtonMask a_buttonMask, UInt16 a_x, UInt16 a_y)
        {
            try
            {
                VncComm.WritePointerEvent(m_writeStream, a_buttonMask, a_x, a_y);
            }
            catch (Exception a_ex)
            {
                cleanupForDisconnect(a_ex);
                onDisconnected(new VncCauseEventArgs(a_ex));
            }
        }

        public byte[] CreateCanvasImage()
        {
            // Lock to not output intermediate results.
            lock (CanvasLock)
            {
                return m_canvas.ImEncode(".png", new ImageEncodingParam(ImwriteFlags.PngCompression, 3));
            }
        }

        private void cleanupForDisconnect(Exception a_ex)
        {
            m_connected = false;

            m_readStream?.Dispose();  m_readStream  = null;
            m_writeStream?.Dispose(); m_writeStream = null;
            m_tcpClient?.Dispose();   m_tcpClient = null;
        }

        private static byte[] encryptChallenge(string a_keyString, byte[] a_challenge)
        {
            byte[] key = new byte[8];
            string headPassword = (a_keyString + "\0\0\0\0\0\0\0\0").Substring(0, key.Length);
            Encoding.ASCII.GetBytes(headPassword, 0, key.Length, key, 0);
            for (var i = 0; i < 8; i++)
            {
                // Apparently RFB mirrors each bit.
                byte b = key[i];
                b = (byte)((b & 0xF0) >> 4 | (b & 0x0F) << 4);
                b = (byte)((b & 0xCC) >> 2 | (b & 0x33) << 2);
                b = (byte)((b & 0xAA) >> 1 | (b & 0x55) << 1);
                key[i] = b;
            }

            var schedule = new byte[16, 6];
            byte[] challengeHead = new byte[8];
            Buffer.BlockCopy(a_challenge, 0, challengeHead, 0, 8);
            byte[] responseHead = new byte[8];
            DES.KeySchedule(key, schedule, DES.ENCRYPT);
            DES.Crypt(challengeHead, responseHead, DES.ToJaggedArray(schedule));

            byte[] challengeTail = new byte[8];
            Buffer.BlockCopy(a_challenge, 8, challengeTail, 0, 8);
            byte[] responseTail = new byte[8];
            DES.KeySchedule(key, schedule, DES.ENCRYPT);
            DES.Crypt(challengeTail, responseTail, DES.ToJaggedArray(schedule));

            byte[] response = new byte[16];
            Buffer.BlockCopy(responseHead, 0, response, 0, 8);
            Buffer.BlockCopy(responseTail, 0, response, 8, 8);

            return response;
        }
    }
}
