using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VncLibrary
{
    public abstract class VncEncodeAbstract
    {
        public UInt16 X
        {
            get;
            protected set;
        }
        public UInt16 Y
        {
            get;
            protected set;
        }
        public UInt16 Width
        {
            get;
            protected set;
        }
        public UInt16 Height
        {
            get;
            protected set;
        }
        public VncEnum.EncodeType EncodeType
        {
            get;
            protected set;
        }

        public abstract void Draw(IVncPixelGetter a_pixelGetter, MatOfByte3 a_mat);
        public void WriteBackwardBody(MatOfByte3 a_mat, MemoryStream a_output)
        {
            // 12 == x(2) + y(2) + w(2) + h(2) + encodeType(4)
            a_output.Write(BigEndianBitConverter.GetBytes(X), 0, 2);
            a_output.Write(BigEndianBitConverter.GetBytes(Y), 0, 2);
            a_output.Write(BigEndianBitConverter.GetBytes(Width), 0, 2);
            a_output.Write(BigEndianBitConverter.GetBytes(Height), 0, 2);
            a_output.Write(BigEndianBitConverter.GetBytes((Int32)VncEnum.EncodeType.Raw), 0, 4);

            var indexer = a_mat.GetIndexer();
            for (int y = Y; y < Y + Height; ++y)
            {
                for (int x = X; x < X + Width; ++x)
                {
                    var bgr = indexer[y, x];
                    a_output.WriteByte(bgr.Item0);
                    a_output.WriteByte(bgr.Item1);
                    a_output.WriteByte(bgr.Item2);
                    a_output.WriteByte(255);
                }
            }
        }

        protected static void writePixelColor(IVncPixelGetter a_pixelGetter, byte[] a_body, int a_basePos, MemoryStream a_output)
        {
            var bgr = a_pixelGetter.GetPixelVec3b(a_body, a_basePos);
            a_output.WriteByte(bgr.Item2);
            a_output.WriteByte(bgr.Item1);
            a_output.WriteByte(bgr.Item0);
            a_output.WriteByte(255);
        }
    }
}
