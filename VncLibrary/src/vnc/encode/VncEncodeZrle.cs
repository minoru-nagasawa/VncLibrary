using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VncLibrary
{
    public class VncEncodeZrle : VncEncodeAbstract
    {
        private byte[] m_zlibData;
        private int    m_offset;
        public VncEncodeZrle(UInt16 a_x, UInt16 a_y, UInt16 a_width, UInt16 a_height, byte[] a_zlibData, int a_offset)
        {
            X = a_x;
            Y = a_y;
            Width  = a_width;
            Height = a_height;
            m_zlibData = a_zlibData;
            m_offset   = a_offset;
            EncodeType = VncEnum.EncodeType.ZRLE;
        }

        public override void Draw(IVncPixelGetter a_pixelGetter, MatOfByte3 a_mat)
        {
            int readPos = m_offset + 4; // Skip length
            for (int y = Y; y < Y + Height; y += 64)
            {
                int h = (Height - y) > 64 ? 64 : (Height - y);
                for (int x = X; x < X + Width; x += 64)
                {
                    int w = (Width - x) > 64 ? 64 : (Width - x);
                    int subencodingType = m_zlibData[readPos++];
                    if (subencodingType == 0)
                    {
                        var indexer  = a_mat.GetIndexer();
                        int byteSize = a_pixelGetter.GetPixelByteSize();
                        for (int posY = y; posY < y + h; ++posY)
                        {
                            for (int posX = x; posX < x + w; ++posX)
                            {
                                indexer[posY, posX] = a_pixelGetter.GetPixelVec3b(m_zlibData, readPos);
                                readPos += byteSize;
                            }
                        }
                    }
                    else if (subencodingType == 1)
                    {
                        Scalar background = (Scalar)a_pixelGetter.GetPixelVec3b(m_zlibData, readPos);
                        readPos += a_pixelGetter.GetPixelByteSize();

                        a_mat.Rectangle(new Rect(x, y, w, h), background, -1 /* Fill */);
                    }
                    else if (2 <= subencodingType && subencodingType <= 16)
                    {
                        Vec3b[] palette = new Vec3b[subencodingType];
                        for (int i = 0; i < subencodingType; ++i)
                        {
                            palette[i] = a_pixelGetter.GetPixelVec3b(m_zlibData, readPos);
                            readPos += a_pixelGetter.GetPixelByteSize();
                        }
                        
                        var indexer  = a_mat.GetIndexer();
                        int byteSize = a_pixelGetter.GetPixelByteSize();
                        int[] ppa    = createPackedPixelsArray(w, h, m_zlibData, readPos, subencodingType);
                        for (int i = 0, posY = y; posY < y + h; ++posY)
                        {
                            for (int posX = x; posX < x + w; ++posX)
                            {
                                indexer[posY, posX] = palette[ppa[i]];
                                ++i;
                            }
                        }

                        readPos += getPackedPixelsBytesSize(w, h, subencodingType);
                    }
                    else if (17 <= subencodingType && subencodingType <= 127)
                    {
                        throw new NotSupportedException($"SubencodingType ({subencodingType}) is not supported.");
                    }
                    else if (subencodingType == 128)
                    {
                        var indexer = a_mat.GetIndexer();
                        int posX = x;
                        int posY = y;

                        int totalLen = 0;
                        int size = w * h;
                        while (totalLen < size)
                        {
                            Vec3b pixel = a_pixelGetter.GetPixelVec3b(m_zlibData, readPos);
                            readPos += a_pixelGetter.GetPixelByteSize();

                            // count length
                            int b;
                            int len = 1;
                            do
                            {
                                b = m_zlibData[readPos++];
                                len += b;
                            } while (b != 255);

                            // set pixel
                            for (int i = 0; i < len; ++i)
                            {
                                indexer[posY, posX] = pixel;

                                // to next position
                                ++posX;
                                if (posX >= x + w)
                                {
                                    posX = x;
                                    ++posY;
                                }
                            }

                            totalLen += len;
                        }
                    }
                    else if (subencodingType == 129)
                    {
                        throw new NotSupportedException($"SubencodingType ({subencodingType}) is not supported.");
                    }
                    else if (130 <= subencodingType && subencodingType <= 255)
                    {
                        // create palette
                        int paletteSize = subencodingType - 128;
                        Vec3b[] palette = new Vec3b[paletteSize];
                        for (int i = 0; i < paletteSize; ++i)
                        {
                            palette[i] = a_pixelGetter.GetPixelVec3b(m_zlibData, readPos);
                            readPos += a_pixelGetter.GetPixelByteSize();
                        }

                        var indexer = a_mat.GetIndexer();
                        int posX = x;
                        int posY = y;

                        int totalLen = 0;
                        int size = w * h;
                        while (totalLen < size)
                        {
                            int paletteIndex = m_zlibData[readPos++];
                            if ((paletteIndex & 0b10000000) == 0)
                            {
                                // length is 1
                                indexer[posY, posX] = palette[paletteIndex];
                                ++totalLen;

                                // to next position
                                ++posX;
                                if (posX >= x + w)
                                {
                                    posX = x;
                                    ++posY;
                                }
                            }
                            else
                            {
                                int b;
                                int len = 1;
                                do
                                {
                                    b = m_zlibData[readPos++];
                                    len += b;
                                } while (b != 255);

                                for (int i = 0; i < len; ++i)
                                {
                                    indexer[posY, posX] = palette[paletteIndex];

                                    // to next position
                                    ++posX;
                                    if (posX >= x + w)
                                    {
                                        posX = x;
                                        ++posY;
                                    }
                                }

                                totalLen += len;
                            }
                        }
                    }
                    else
                    {
                        throw new NotSupportedException($"SubencodingType ({subencodingType}) is not supported.");
                    }
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

            // skip zrle length
            pos += 4;
            
            using (var tmpOutput = new MemoryStream())
            {
                for (int y = Y; y < Y + Height; y += 64)
                {
                    int h = (Height - y) > 64 ? 64 : (Height - y);
                    for (int x = X; x < X + Width; x += 64)
                    {
                        int w = (Width - x) > 64 ? 64 : (Width - x);
                        int subencodingType = a_body[pos++];
                        tmpOutput.WriteByte((byte) subencodingType);
                        if (subencodingType == 0)
                        {
                            for (int posY = y; posY < y + h; ++posY)
                            {
                                for (int posX = x; posX < x + w; ++posX)
                                {
                                    writePixelColor(a_pixelGetter, a_body, pos, tmpOutput);
                                    pos += byteSize;
                                }
                            }
                        }
                        else if (subencodingType == 1)
                        {
                            writePixelColor(a_pixelGetter, a_body, pos, tmpOutput);
                            pos += byteSize;
                        }
                        else if (2 <= subencodingType && subencodingType <= 16)
                        {
                            Vec3b[] palette = new Vec3b[subencodingType];
                            for (int i = 0; i < subencodingType; ++i)
                            {
                                writePixelColor(a_pixelGetter, a_body, pos, tmpOutput);
                                pos += byteSize;
                            }

                            int ppaSize = getPackedPixelsBytesSize(w, h, subencodingType);
                            tmpOutput.Write(a_body, pos, ppaSize);
                            pos += ppaSize;
                        }
                        else if (17 <= subencodingType && subencodingType <= 127)
                        {
                            throw new NotSupportedException($"SubencodingType ({subencodingType}) is not supported.");
                        }
                        else if (subencodingType == 128)
                        {
                            int totalLen = 0;
                            int size = w * h;
                            while (totalLen < size)
                            {
                                writePixelColor(a_pixelGetter, a_body, pos, tmpOutput);
                                pos += byteSize;

                                // count length
                                int b;
                                int len = 1;
                                do
                                {
                                    b = a_body[pos++];
                                    len += b;
                                    tmpOutput.WriteByte((byte) b);
                                } while (b != 255);

                                totalLen += len;
                            }
                        }
                        else if (subencodingType == 129)
                        {
                            throw new NotSupportedException($"SubencodingType ({subencodingType}) is not supported.");
                        }
                        else if (130 <= subencodingType && subencodingType <= 255)
                        {
                            // create palette
                            int paletteSize = subencodingType - 128;
                            for (int i = 0; i < paletteSize; ++i)
                            {
                                writePixelColor(a_pixelGetter, a_body, pos, tmpOutput);
                                pos += byteSize;
                            }

                            int totalLen = 0;
                            int size = w * h;
                            while (totalLen < size)
                            {
                                int paletteIndex = a_body[pos++];
                                tmpOutput.WriteByte((byte)paletteIndex);
                                if ((paletteIndex & 0b10000000) == 0)
                                {
                                    ++totalLen;
                                }
                                else
                                {
                                    int b;
                                    int len = 1;
                                    do
                                    {
                                        b = a_body[pos++];
                                        len += b;
                                        tmpOutput.WriteByte((byte)b);
                                    } while (b != 255);

                                    totalLen += len;
                                }
                            }
                        }
                        else
                        {
                            throw new NotSupportedException($"SubencodingType ({subencodingType}) is not supported.");
                        }
                    }
                }

                // zrle length
                Int32 zrleLength = pos - a_basePos - 12 - 4;
                byte[] zrleLengthByte = BigEndianBitConverter.GetBytes(zrleLength);
                a_output.Write(zrleLengthByte, 0, 4);

                // optimized body
                tmpOutput.WriteTo(a_output);
            }

            return pos - a_basePos;
        }

        static private int[] createPackedPixelsArray(int a_width, int a_height, byte[] a_zlibData, int a_offset, int a_paletteSize)
        {
            int bitsSize;
            int startOrBits;
            int startShiftBits;
            if (a_paletteSize == 2)
            {
                bitsSize       = 1;
                startOrBits    = 0b10000000;
                startShiftBits = 7;
            }
            else if (a_paletteSize <= 4)
            {
                bitsSize       = 2;
                startOrBits    = 0b11000000;
                startShiftBits = 6;
            }
            else if (a_paletteSize <= 16)
            {
                bitsSize       = 4;
                startOrBits    = 0b11110000;
                startShiftBits = 4;
            }
            else
            {
                throw new NotSupportedException($"PaletteSize ({a_paletteSize}) is not supported.");
            }

            int[] retVal = new int[a_width * a_height];
            int packedPixelsSize = getPackedPixelsBytesSize(a_width, a_height, a_paletteSize);
            for (int i = 0, pos = 0; i < packedPixelsSize; ++i)
            {
                int w = 0;
                int orBits    = startOrBits;
                int shiftBits = startShiftBits;
                while (orBits != 0)
                {
                    retVal[pos] = (a_zlibData[a_offset + i] & orBits) >> shiftBits;
                    ++pos;

                    orBits    >>= bitsSize;
                    shiftBits -=  bitsSize;
                    ++w;

                    if (w >= a_width)
                    {
                        break;
                    }
                }
            }

            return retVal;
        }

        static private int getPackedPixelsBytesSize(int a_width, int a_height, int a_paletteSize)
        {
            if (a_paletteSize == 2)
            {
                return (int)Math.Floor((a_width + 7) / 8.0) * a_height;
            }
            else if (a_paletteSize <= 4)
            {
                return (int)Math.Floor((a_width + 3) / 4.0) * a_height;
            }
            else if (a_paletteSize <= 16)
            {
                return (int)Math.Floor((a_width + 1) / 2.0) * a_height;
            }
            else
            {
                throw new NotSupportedException($"PaletteSize ({a_paletteSize}) is not supported.");
            }
        }
    }
}
