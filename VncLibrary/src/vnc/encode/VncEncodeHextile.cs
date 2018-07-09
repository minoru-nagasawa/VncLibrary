using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VncLibrary
{
    public class VncEncodeHextileSubencodingMask
    {
        public bool Raw                 { get; private set; }
        public bool BackgroundSpecified { get; private set; }
        public bool ForegroundSpecified { get; private set; }
        public bool AnySubrects         { get; private set; }
        public bool SubrectsColored     { get; private set; }
        
        public VncEncodeHextileSubencodingMask(byte a_subencodingMask)
        {
            Raw                 = ((a_subencodingMask & 0x01) != 0) ? true : false;
            BackgroundSpecified = ((a_subencodingMask & 0x02) != 0) ? true : false;
            ForegroundSpecified = ((a_subencodingMask & 0x04) != 0) ? true : false;
            AnySubrects         = ((a_subencodingMask & 0x08) != 0) ? true : false;
            SubrectsColored     = ((a_subencodingMask & 0x10) != 0) ? true : false;
        }
    }

    public interface IVncEncodeHextileTile
    {
        void DrawHexTile(int a_x, int a_y, int a_width, int a_height, IVncPixelGetter a_pixelGetter, MatOfByte3 a_mat);
    }

    public class VncEncodeHextileTileRaw : IVncEncodeHextileTile
    {
        public byte[] RawPixel
        {
            get;
            private set;
        }
        public int Offset
        {
            get;
            private set;
        }

        public VncEncodeHextileTileRaw(byte[] a_rawPixel, int a_offset)
        {
            RawPixel = a_rawPixel;
            Offset   = a_offset;
        }

        public void DrawHexTile(int a_x, int a_y, int a_width, int a_height, IVncPixelGetter a_pixelGetter, MatOfByte3 a_mat)
        {
            var indexer  = a_mat.GetIndexer();
            int byteSize = a_pixelGetter.GetPixelByteSize();
            int offset   = Offset;
            for (int y = a_y; y < a_y + a_height; ++y)
            {
                for (int x = a_x; x < a_x + a_width; ++x)
                {
                    indexer[y, x] = a_pixelGetter.GetPixelVec3b(RawPixel, offset);
                    offset += byteSize;
                }
            }
        }
    }

    public class VncEncodeHextileTileRre : IVncEncodeHextileTile
    {
        public byte[] Background
        {
            get;
            private set;
        }

        public VncEncodeHextileSubrect[] Subrects
        {
            get;
            private set;
        }

        public VncEncodeHextileTileRre(byte[] a_background, VncEncodeHextileSubrect[] a_subrects)
        {
            Background = a_background;
            Subrects   = a_subrects;
        }

        public void DrawHexTile(int a_x, int a_y, int a_width, int a_height, IVncPixelGetter a_pixelGetter, MatOfByte3 a_mat)
        {
            Scalar background = (Scalar)a_pixelGetter.GetPixelVec3b(Background, 0);
            a_mat.Rectangle(new Rect(a_x, a_y, a_width, a_height), background, -1 /* Fill */);

            foreach (var v in Subrects)
            {
                Scalar color = (Scalar)a_pixelGetter.GetPixelVec3b(v.SubrectsColor, 0);
                a_mat.Rectangle(new Rect(a_x + v.X, a_y + v.Y, v.Width, v.Height), color, -1 /* Fill */);
            }
        }
    }

    public class VncEncodeHextileSubrect
    {
        public byte[] SubrectsColor
        {
            get;
            private set;
        }

        public byte X
        {
            get;
            private set;
        }

        public byte Y
        {
            get;
            private set;
        }

        public byte Width
        {
            get;
            private set;
        }

        public byte Height
        {
            get;
            private set;
        }

        public VncEncodeHextileSubrect(byte[] a_subrectColor, byte a_x_and_y, byte a_width_and_height)
        {
            SubrectsColor = a_subrectColor;
            X = (byte)((a_x_and_y >> 4) & 0x0F);
            Y = (byte)((a_x_and_y     ) & 0x0F);
            Width  = (byte)(((a_width_and_height >> 4) & 0x0F) + 1);
            Height = (byte)(((a_width_and_height     ) & 0x0F) + 1);
        }
    }

    public class VncEncodeHextile : VncEncodeAbstract
    {
        public IVncEncodeHextileTile[] Hextile
        {
            get;
            private set;
        }

        public VncEncodeHextile(UInt16 a_x, UInt16 a_y, UInt16 a_width, UInt16 a_height, IVncEncodeHextileTile[] a_hextile)
        {
            X = a_x;
            Y = a_y;
            Width   = a_width;
            Height  = a_height;
            Hextile = a_hextile;
            EncodeType = VncEnum.EncodeType.Hextile;
        }

        public override void Draw(IVncPixelGetter a_pixelGetter, MatOfByte3 a_mat)
        {
            int tile = 0;
            for (int y = Y; y < Y + Height; y += 16)
            {
                int h = (Y + Height - y) > 16 ? 16 : (Y + Height - y);
                for (int x = X; x < X + Width; x += 16)
                {
                    int w = (X + Width - x) > 16 ? 16 : (X + Width - x);
                    Hextile[tile].DrawHexTile(x, y, w, h, a_pixelGetter, a_mat);
                    ++tile;
                }
            }
        }

        public override int WriteOptimizedBody(IVncPixelGetter a_pixelGetter, byte[] a_body, int a_basePos, MemoryStream a_output)
        {
            int byteSize = a_pixelGetter.GetPixelByteSize();
            int pos = a_basePos;

            // 12 == x(2) + y(2) + w(2) + h(2) + encodeType(4)
            a_output.Write(a_body, pos, 12);
            pos += 12;

            int xTileCount = (int)Math.Ceiling(Width  / 16.0);
            int yTileCount = (int)Math.Ceiling(Height / 16.0);
            for (int ytile = 0; ytile < yTileCount; ++ytile)
            {
                int tileHeight = ((ytile == yTileCount - 1) && ((Height % 16) != 0)) ? Height % 16 : 16;
                for (int xtile = 0; xtile < xTileCount; ++xtile)
                {
                    int tileWidth = ((xtile == xTileCount - 1) && ((Width % 16) != 0)) ? Width % 16 : 16;

                    // SubencodingMask
                    var subencodingMask = new VncEncodeHextileSubencodingMask(a_body[pos]);
                    a_output.WriteByte(a_body[pos++]);

                    if (subencodingMask.Raw)
                    {
                        for (int y = 0; y < tileHeight; ++y)
                        {
                            for (int x = 0; x < tileWidth; ++x)
                            {
                                writePixelColor(a_pixelGetter, a_body, pos, a_output);
                                pos += byteSize;
                            }
                        }
                    }
                    else
                    {
                        // Read BackgroundSpecified if needed
                        if (subencodingMask.BackgroundSpecified)
                        {
                            writePixelColor(a_pixelGetter, a_body, pos, a_output);
                            pos += byteSize;
                        }

                        // Read ForegroundSpecified if needed
                        if (subencodingMask.ForegroundSpecified)
                        {
                            writePixelColor(a_pixelGetter, a_body, pos, a_output);
                            pos += byteSize;
                        }

                        // Subrects
                        if (subencodingMask.AnySubrects)
                        {
                            // Read number-of-subrectangles
                            byte numberOfSubrectanglesHextile = a_body[pos++];
                            a_output.WriteByte(numberOfSubrectanglesHextile);

                            // Read subrectangle
                            if (subencodingMask.SubrectsColored)
                            {
                                for (int i = 0; i < numberOfSubrectanglesHextile; ++i)
                                {
                                    writePixelColor(a_pixelGetter, a_body, pos, a_output);
                                    pos += byteSize;

                                    a_output.Write(a_body, pos, 2);
                                    pos += 2;
                                }
                            }
                            else
                            {
                                for (int i = 0; i < numberOfSubrectanglesHextile; ++i)
                                {
                                    a_output.Write(a_body, pos, 2);
                                    pos += 2;
                                }
                            }
                        }
                    }
                }
            }

            return pos - a_basePos;
        }
    }
}
