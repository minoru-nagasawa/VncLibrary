using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VncLibrary
{
    public class VncEncodeRaw : VncEncodeAbstract
    {
        private byte[] m_pixelData;
        private int    m_offset;
        public VncEncodeRaw(UInt16 a_x, UInt16 a_y, UInt16 a_width, UInt16 a_height, byte[] a_pixelData, int a_offset)
        {
            X = a_x;
            Y = a_y;
            Width  = a_width;
            Height = a_height;
            m_pixelData = a_pixelData;
            m_offset    = a_offset;
            EncodeType  = VncEnum.EncodeType.Raw;
        }

        public override void Draw(IVncPixelGetter a_pixelGetter, MatOfByte3 a_mat)
        {
            var indexer  = a_mat.GetIndexer();
            int byteSize = a_pixelGetter.GetPixelByteSize();
            int offset   = m_offset;
            for (int y = Y; y < Y + Height; ++y)
            {
                for (int x = X; x < X + Width; ++x)
                {
                    indexer[y, x] = a_pixelGetter.GetPixelVec3b(m_pixelData, offset);
                    offset += byteSize;
                }
            }
        }
    }
}
