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
                m_stream?.Dispose();    m_stream    = null;
                m_tcpClient?.Dispose(); m_tcpClient = null;
                m_canvas?.Dispose();    m_canvas    = null;
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

        private Func<Socket, Stream> m_streamCreator;
        private TcpClient            m_tcpClient;
        private Stream               m_stream;

        private VncServerInitBody   m_serverInitBody;
        private bool                m_readingServerMessage;
        private bool                m_connected;
        private IVncPixelGetter     m_pixelGetterNormal;
        private IVncPixelGetter     m_pixelGetterZrle;
        private MatOfByte3          m_canvas;
        private object              m_canvasLock;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_clientConfig"></param>
        /// <param name="a_streamCreator"></param>
        /// <example>
        /// var config = new VncConfig("192.168.1.1", 5900, "password", true);
        /// var client = new VncClient(config, (s) => new BufferedStream(new NetworkStream(s)));
        /// </example>
        public VncClient(VncConfig a_clientConfig, Func<Socket, Stream> a_streamCreator)
        {
            ClientConfig    = a_clientConfig;
            m_streamCreator = a_streamCreator;
            m_canvasLock    = new object();
        }

        public void CloseVnc()
        {
            m_stream?.Dispose();    m_stream = null;
            m_tcpClient?.Dispose(); m_tcpClient = null;
        }

        public async Task<bool> ConnectVncAsync()
        {
            Connecting = true;

            try
            {
                m_tcpClient = new TcpClient();
                await m_tcpClient.ConnectAsync(ClientConfig.Address, ClientConfig.Port);

                m_stream = m_streamCreator(m_tcpClient.Client);

                //-----------------------
                // Handshake
                //-----------------------
                // Server -> (ProtocolVersion) -> Client
                var version = await VncComm.ReadProtocolVersionAsync(m_stream, ClientConfig.ForceVersion);

                // Server <- (ProtocolVersion) <- Client
                await VncComm.WriteProtocolVersionAsync(m_stream, version);

                // (Security)
                if (version == VncEnum.Version.Version33)
                {
                    // Server -> (Security) -> Client
                    var securityType = await VncComm.ReadSecurityTypeAsync(m_stream);
                    if (securityType != VncEnum.SecurityType.None
                    &&  securityType != VncEnum.SecurityType.VNCAuthentication)
                    {
                        throw new SecurityException($"VNC Version is 3.3. Security type is {securityType}.");
                    }

                    if (securityType == VncEnum.SecurityType.VNCAuthentication)
                    {
                        // Server -> (VNC Authentication Challenge) -> Client
                        var challenge = await VncComm.ReadVncChallangeAsync(m_stream);

                        // Server <- (VNC Authentication Response) <- Client
                        byte[] response = encryptChallenge(ClientConfig.Password, challenge);
                        await VncComm.WriteVncResponseAsync(m_stream, response);

                        // Server -> (Security Result) -> Client
                        await VncComm.ReadSecurityResultAsync(m_stream, version); // Result is checked in method. So don't check here, 
                    }
                }
                else
                {
                    // Server -> (SecurityTypes) -> Client
                    var securityTypes = await VncComm.ReadSecurityTypesAsync(m_stream);

                    if (securityTypes.Contains(VncEnum.SecurityType.None))
                    {
                        // Server <- (SecurityType) <- Client
                        await VncComm.WriteSecurityTypeAsync(m_stream, VncEnum.SecurityType.None);
                    }
                    else if (securityTypes.Contains(VncEnum.SecurityType.VNCAuthentication))
                    {
                        // Server <- (SecurityType) <- Client
                        await VncComm.WriteSecurityTypeAsync(m_stream, VncEnum.SecurityType.VNCAuthentication);

                        // Server -> (VNC Authentication Challenge) -> Client
                        var challenge = await VncComm.ReadVncChallangeAsync(m_stream);

                        // Server <- (VNC Authentication Response) <- Client
                        byte[] response = encryptChallenge(ClientConfig.Password, challenge);
                        await VncComm.WriteVncResponseAsync(m_stream, response);
                    }
                    else
                    {
                        throw new SecurityException($"Unknown security-types. Server can use [{string.Join(",", securityTypes)}].");
                    }

                    // Server -> (Security Result) -> Client
                    await VncComm.ReadSecurityResultAsync(m_stream, version);
                }

                //-----------------------
                // Initial Message
                //-----------------------
                // Server <- (ClientInit) <- Client
                await VncComm.WriteClientInitAsync(m_stream, VncEnum.SharedFlag.Share);

                // Server -> (ServerInit) -> Client
                m_serverInitBody = await VncComm.ReadServerInitAsync(m_stream);

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
                await VncComm.WriteSetEncodingsAsync(m_stream, encodings);

                //-----------------------
                // Refresh Framebuffer
                //-----------------------
                // Server <- (Refresh Framebuffer) <- Client
                await VncComm.WriteFramebufferUpdateRequestAsync(m_stream,
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

        public async Task WriteFramebufferUpdateRequestAsync()
        {
            try
            {
                await VncComm.WriteFramebufferUpdateRequestAsync(m_stream,
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

        public async Task<byte[]> ReadServerMessageAsync()
        {
            if (m_readingServerMessage)
            {
                return new byte[0];
            }
            
            m_readingServerMessage = true;
            try
            {
                var readBody = await VncComm.ReadServerMessage(m_stream,
                                                               m_serverInitBody.ServerPixelFormat.BytesPerPixel,
                                                               m_serverInitBody.ServerPixelFormat.BigEndianFlag);

                // Set after WriteFramebufferUpdateRequest to prevent the excess data
                var messageType = (VncEnum.MessageTypeServerToClient)readBody[0];
                if (messageType == VncEnum.MessageTypeServerToClient.FramebufferUpdate)
                {
                    var encodeList = VncEncodeFactory.CreateVncEncodeFromBinary(readBody,
                                                                                m_serverInitBody.ServerPixelFormat.BytesPerPixel,
                                                                                m_serverInitBody.ServerPixelFormat.BigEndianFlag);
                    
                    // Draw to canvas
                    lock (m_canvasLock)
                    {
                        foreach (var e in encodeList)
                        {
                            // Draw to canvas
                            IVncPixelGetter pixelGetter = (e.EncodeType == VncEnum.EncodeType.ZRLE) ? m_pixelGetterZrle : m_pixelGetterNormal;
                            e.Draw(pixelGetter, m_canvas);
                        }
                    }
                }
                else if (messageType == VncEnum.MessageTypeServerToClient.SetColorMapEntries)
                {
                    var colorMap = new VncSetColorMapEntriesBody(readBody);
                    m_pixelGetterNormal.SetColorMap(colorMap);
                }

                onRead(new VncReadEventArgs(messageType, readBody));
                return readBody;
            }
            catch (Exception a_ex)
            {
                cleanupForDisconnect(a_ex);
                onDisconnected(new VncCauseEventArgs(a_ex));
                return new byte[0];
            }
            finally
            {
                m_readingServerMessage = false;
            }
        }

        public byte[] CreateCanvasImage()
        {
            // Lock to not output intermediate results.
            lock (m_canvasLock)
            {
                return m_canvas.ImEncode(".png", new ImageEncodingParam(ImwriteFlags.PngCompression, 3));
            }
        }

        private void cleanupForDisconnect(Exception a_ex)
        {
            m_connected = false;

            m_stream?.Dispose();    m_stream    = null;
            m_tcpClient?.Dispose(); m_tcpClient = null;
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
