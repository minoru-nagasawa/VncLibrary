using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VncLibrary
{
    public class VncEncodeCopyRect : VncEncodeAbstract
    {
        public UInt16 SrcX
        {
            get;
            protected set;
        }
        public UInt16 SrcY
        {
            get;
            protected set;
        }
        public VncEncodeCopyRect(UInt16 a_x, UInt16 a_y, UInt16 a_width, UInt16 a_height, UInt16 a_srcX, UInt16 a_srcY)
        {
            X = a_x;
            Y = a_y;
            Width  = a_width;
            Height = a_height;
            SrcX   = a_srcX;
            SrcY   = a_srcY;
            EncodeType = VncEnum.EncodeType.CopyRect;
        }

        public override void Draw(IVncPixelGetter a_pixelGetter, MatOfByte3 a_mat)
        {
            Rect srcRect = new Rect(SrcX, SrcY, Width, Height);
            Rect dstRect = new Rect(X, Y, Width, Height);
            if (srcRect.IntersectsWith(dstRect))
            {
                using (var src = a_mat.Clone(srcRect))
                using (var dst = new MatOfByte3(a_mat, dstRect))
                {
                    src.CopyTo(dst);
                }
            }
            else
            {
                using (var src = new MatOfByte3(a_mat, srcRect))
                using (var dst = new MatOfByte3(a_mat, dstRect))
                {
                    src.CopyTo(dst);
                }
            }
        }

        public override int WriteOptimizedBody(IVncPixelGetter a_pixelGetter, byte[] a_body, int a_basePos, MemoryStream a_output)
        {
            // 16 == x(2) + y(2) + w(2) + h(2) + encodeType(4) + srcX(2) + srcY(2)
            a_output.Write(a_body, a_basePos, 16);
            return 16;
        }
    }
}
