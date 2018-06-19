using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VncLibrary
{
    public class VncEncodeRreSubrectangle
    {
        public byte[] PixelBinary
        {
            get;
            private set;
        }

        public UInt16 X
        {
            get;
            private set;
        }

        public UInt16 Y
        {
            get;
            private set;
        }

        public UInt16 Width
        {
            get;
            private set;
        }

        public UInt16 Height
        {
            get;
            private set;
        }

        public VncEncodeRreSubrectangle(byte[] a_pixelBinary, UInt16 a_x, UInt16 a_y, UInt16 a_width, UInt16 a_height)
        {
            PixelBinary = a_pixelBinary;
            X           = a_x;
            Y           = a_y;
            Width       = a_width;
            Height      = a_height;
        }
    }
    public class VncEncodeRre : VncEncodeAbstract
    {
        public byte[] Background
        {
            get;
            private set;
        }
        public VncEncodeRreSubrectangle[] Subrectangle
        {
            get;
            private set;
        }

        public VncEncodeRre(UInt16 a_x, UInt16 a_y, UInt16 a_width, UInt16 a_height, byte[] a_background, VncEncodeRreSubrectangle[] a_subrectangle)
        {
            X = a_x;
            Y = a_y;
            Width  = a_width;
            Height = a_height;
            Background   = a_background;
            Subrectangle = a_subrectangle;
            EncodeType   = VncEnum.EncodeType.RRE;
        }

        public override void Draw(IVncPixelGetter a_pixelGetter, MatOfByte3 a_mat)
        {
            Scalar background = (Scalar)a_pixelGetter.GetPixelVec3b(Background, 0);
            a_mat.Rectangle(new Rect(X, Y, Width, Height), background, -1 /* Fill */);

            foreach (var v in Subrectangle)
            {
                Scalar pixelValue = (Scalar)a_pixelGetter.GetPixelVec3b(v.PixelBinary, 0);
                a_mat.Rectangle(new Rect(v.X, v.Y, v.Width, v.Height), pixelValue, -1 /* Fill */);
            }
        }

        public override int WriteOptimizedBody(IVncPixelGetter a_pixelGetter, byte[] a_body, int a_basePos, MemoryStream a_output)
        {
            int byteSize = a_pixelGetter.GetPixelByteSize();
            int pos = a_basePos;

            // 12 == x(2) + y(2) + w(2) + h(2) + encodeType(4)
            a_output.Write(a_body, pos, 12);
            pos += 12;

            // numberOfSubrectangles
            a_output.Write(a_body, pos, 4);
            pos += 4;

            // Background
            writePixelColor(a_pixelGetter, Background, 0, a_output);
            pos += byteSize;
            
            foreach (var v in Subrectangle)
            {
                // Pixel
                writePixelColor(a_pixelGetter, v.PixelBinary, 0, a_output);
                pos += byteSize;

                // xpos(2) - ypos(2) - width(2) - height(2)
                a_output.Write(a_body, pos, 8);
                pos += 8;
            }

            return pos - a_basePos;
        }
    }
}
