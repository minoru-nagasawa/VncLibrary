using System;
using System.Collections.Generic;
using System.Text;

namespace VncLibrary
{
    public class PixelFormat
    {
        public byte BytesPerPixel
        {
            get;
            private set;
        }

        public byte Depth
        {
            get;
            private set;
        }

        public bool BigEndianFlag
        {
            get;
            private set;
        }

        public bool TrueColorFlag
        {
            get;
            private set;
        }

        public UInt16 RedMax
        {
            get;
            private set;
        }

        public UInt16 GreenMax
        {
            get;
            private set;
        }

        public UInt16 BlueMax
        {
            get;
            private set;
        }

        public byte RedShift
        {
            get;
            private set;
        }

        public byte GreenShift
        {
            get;
            private set;
        }

        public byte BlueShift
        {
            get;
            private set;
        }

        public PixelFormat(byte[] a_buffer)
        {
            const int BASE_POS = 4;
            byte bitsPerPixel = a_buffer[BASE_POS + 0];
            BytesPerPixel = (byte) (bitsPerPixel / 8);
            Depth         = a_buffer[BASE_POS + 1];
            BigEndianFlag = a_buffer[BASE_POS + 2] != 0 ? true : false;
            TrueColorFlag = a_buffer[BASE_POS + 3] != 0 ? true : false;
            RedMax        = BigEndianBitConverter.ToUInt16(a_buffer, BASE_POS + 4);
            GreenMax      = BigEndianBitConverter.ToUInt16(a_buffer, BASE_POS + 6);
            BlueMax       = BigEndianBitConverter.ToUInt16(a_buffer, BASE_POS + 8);
            RedShift      = a_buffer[BASE_POS + 10];
            GreenShift    = a_buffer[BASE_POS + 11];
            BlueShift     = a_buffer[BASE_POS + 12];

            if (bitsPerPixel != 8
            &&  bitsPerPixel != 16
            &&  bitsPerPixel != 32)
            {
                throw new ArgumentException($"BitsPerPixel({bitsPerPixel}) is invalid.");
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_bytesPerPixel"></param>
        /// <param name="a_depth"></param>
        /// <param name="a_bigEndianFlag"></param>
        /// <param name="a_trueColorFlag"></param>
        /// <param name="a_redMax"></param>
        /// <param name="a_greenMax"></param>
        /// <param name="a_blueMax"></param>
        /// <param name="a_redShift"></param>
        /// <param name="a_greenShift"></param>
        /// <param name="a_blueShift"></param>
        public PixelFormat(byte a_bytesPerPixel, byte a_depth, bool a_bigEndianFlag, bool a_trueColorFlag,
                           UInt16 a_redMax, UInt16 a_greenMax, UInt16 a_blueMax, 
                           byte a_redShift, byte a_greenShift, byte a_blueShift)
        {
            BytesPerPixel = a_bytesPerPixel;
            Depth         = a_depth;
            BigEndianFlag = a_bigEndianFlag;
            TrueColorFlag = a_trueColorFlag;
            RedMax        = a_redMax;
            GreenMax      = a_greenMax;
            BlueMax       = a_blueMax;
            RedShift      = a_redShift;
            GreenShift    = a_greenShift;
            BlueShift     = a_blueShift;
        }
    }

    public class VncServerInitBody
    {
        public UInt16 FramebufferWidth
        {
            get;
            private set;
        }

        public UInt16 FramebufferHeight
        {
            get;
            private set;
        }

        public PixelFormat ServerPixelFormat
        {
            get;
            private set;
        }

        public UInt32 NameLength
        {
            get;
            private set;
        }

        public string NameString
        {
            get;
            private set;
        }

        public VncServerInitBody(byte[] a_buffer)
        {
            FramebufferWidth  = BigEndianBitConverter.ToUInt16(a_buffer, 0);
            FramebufferHeight = BigEndianBitConverter.ToUInt16(a_buffer, 2);
            ServerPixelFormat = new PixelFormat(a_buffer);
            NameLength        = BigEndianBitConverter.ToUInt32(a_buffer, 20);
            NameString        = Encoding.ASCII.GetString(a_buffer, 24, (int)NameLength);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_framebufferWidth"></param>
        /// <param name="a_framebufferHeight"></param>
        /// <param name="a_serverPixelFormat"></param>
        /// <param name="a_nameString"></param>
        /// <remarks>This system don't use. It is for testing.</remarks>
        public VncServerInitBody(UInt16 a_framebufferWidth, UInt16 a_framebufferHeight, PixelFormat a_serverPixelFormat, string a_nameString)
        {
            FramebufferWidth  = a_framebufferWidth;
            FramebufferHeight = a_framebufferHeight;
            ServerPixelFormat = a_serverPixelFormat;
            NameLength        = (UInt32)a_nameString.Length;
            NameString        = a_nameString;
        }
    }
}
