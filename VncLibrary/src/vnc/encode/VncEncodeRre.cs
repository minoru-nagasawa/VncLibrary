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
                a_mat.Rectangle(new Rect(X + v.X, Y + v.Y, v.Width, v.Height), pixelValue, -1 /* Fill */);
            }
        }
    }
}
