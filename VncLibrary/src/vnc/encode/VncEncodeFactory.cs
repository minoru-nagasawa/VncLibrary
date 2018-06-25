using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;

namespace VncLibrary
{
    public static class VncEncodeFactory
    {
        public static async Task<List<VncEncodeAbstract>> CreateVncEncodeFromStream(Stream a_stream, byte a_bytesPerPixel, bool a_isBigendian)
        {
            var vncEncodeList = new List<VncEncodeAbstract>();

            // Read padding(1) - number-of-rectangles(2)
            byte[] head = new byte[3];
            await a_stream.ReadAllAsync(head, 0, head.Length);
            UInt16 numberOfRectangle = BigEndianBitConverter.ToUInt16(head, 1);

            // Read rectangles
            for (int i = 0; i < numberOfRectangle; ++i)
            {
                var encode = await createVncEncodeFromStreamSub(a_stream, a_bytesPerPixel, a_isBigendian);
                vncEncodeList.Add(encode);
            }

            return vncEncodeList;
        }

        private static async Task<VncEncodeAbstract> createVncEncodeFromStreamSub(Stream a_stream, byte a_bytesPerPixel, bool a_isBigendian)
        {
            byte[] header = new byte[12];
            await a_stream.ReadAllAsync(header, 0, header.Length);

            UInt16 x = BigEndianBitConverter.ToUInt16(header, 0);
            UInt16 y = BigEndianBitConverter.ToUInt16(header, 2);
            UInt16 w = BigEndianBitConverter.ToUInt16(header, 4);
            UInt16 h = BigEndianBitConverter.ToUInt16(header, 6);
            var encodeType = (VncEnum.EncodeType)BigEndianBitConverter.ToInt32(header, 8);

            switch (encodeType)
            {
            case VncEnum.EncodeType.Raw:
                byte[] rawPixel = new byte[w * h * a_bytesPerPixel];
                await a_stream.ReadAllAsync(rawPixel, 0, rawPixel.Length);
                return new VncEncodeRaw(x, y, w, h, rawPixel, 0);

            case VncEnum.EncodeType.CopyRect:
                byte[] copyRectPixel = new byte[4];
                await a_stream.ReadAllAsync(copyRectPixel, 0, copyRectPixel.Length);
                UInt16 sx = BigEndianBitConverter.ToUInt16(copyRectPixel, 0);
                UInt16 sy = BigEndianBitConverter.ToUInt16(copyRectPixel, 2);
                return new VncEncodeCopyRect(x, y, w, h, sx, sy);

            case VncEnum.EncodeType.RRE:
                // Read header
                byte[] rreHeader = new byte[4 + a_bytesPerPixel];
                await a_stream.ReadAllAsync(rreHeader, 0, rreHeader.Length);
                UInt32 numberOfSubrectangles = BigEndianBitConverter.ToUInt32(rreHeader, 0);
                byte[] backgroundPixelValue  = VncPixelByteGetter.GetPixelByte(a_bytesPerPixel, rreHeader, 4);

                // Read subrectangles
                int subrectSize = a_bytesPerPixel + 8;
                byte[] rreSubrectangles = new byte[subrectSize * numberOfSubrectangles];
                await a_stream.ReadAllAsync(rreSubrectangles, 0, rreSubrectangles.Length);

                // Create subrectangles
                var subrectangles = new VncEncodeRreSubrectangle[numberOfSubrectangles];
                for (int i = 0; i < numberOfSubrectangles; ++i)
                {
                    int offset = subrectSize * i;
                    byte[] pixel  = VncPixelByteGetter.GetPixelByte(a_bytesPerPixel, rreSubrectangles, 0 + offset);
                    UInt16 xpos   = BigEndianBitConverter.ToUInt16(rreSubrectangles, a_bytesPerPixel + 0 + offset);
                    UInt16 ypos   = BigEndianBitConverter.ToUInt16(rreSubrectangles, a_bytesPerPixel + 2 + offset);
                    UInt16 width  = BigEndianBitConverter.ToUInt16(rreSubrectangles, a_bytesPerPixel + 4 + offset);
                    UInt16 height = BigEndianBitConverter.ToUInt16(rreSubrectangles, a_bytesPerPixel + 6 + offset);
                    subrectangles[i] = new VncEncodeRreSubrectangle(pixel, xpos, ypos, width, height);
                }

                return new VncEncodeRre(x, y, w, h, backgroundPixelValue, subrectangles);

            case VncEnum.EncodeType.Hextile:
                int xTileCount = (int)Math.Ceiling(w / 16.0);
                int yTileCount = (int)Math.Ceiling(h / 16.0);
                var hextiles = new IVncEncodeHextileTile[xTileCount * yTileCount];

                // Loop for each tile
                int index = 0;
                byte[] backgroundPixel = null;
                byte[] foregroundPixel = null;
                for (int ytile = 0; ytile < yTileCount; ++ytile, ++index)
                {
                    int tileHeight = ((ytile == yTileCount - 1) && ((h % 16) != 0)) ? h % 16 : 16;
                    for (int xtile = 0; xtile < xTileCount; ++xtile, ++index)
                    {
                        int tileWidth = ((xtile == xTileCount - 1) && ((w % 16) != 0)) ? w % 16 : 16;

                        // Read header
                        byte[] subencodingMaskBuffer = new byte[1];
                        await a_stream.ReadAllAsync(subencodingMaskBuffer, 0, subencodingMaskBuffer.Length);
                        var subencodingMask = new VncEncodeHextileSubencodingMask(subencodingMaskBuffer[0]);

                        if (subencodingMask.Raw)
                        {
                            // Read raw pixel data
                            byte[] tilesRawPixel = new byte[tileWidth * tileHeight * a_bytesPerPixel];
                            await a_stream.ReadAllAsync(tilesRawPixel, 0, tilesRawPixel.Length);

                            hextiles[index] = new VncEncodeHextileTileRaw(tilesRawPixel, 0);
                        }
                        else
                        {
                            // Read BackgroundSpecified if needed
                            if (subencodingMask.BackgroundSpecified)
                            {
                                await a_stream.ReadAllAsync(backgroundPixel, 0, a_bytesPerPixel);
                            }

                            // Read ForegroundSpecified if needed
                            if (subencodingMask.ForegroundSpecified)
                            {
                                await a_stream.ReadAllAsync(foregroundPixel, 0, a_bytesPerPixel);
                            }

                            // Subrects
                            VncEncodeHextileSubrect[] hextileSubrect;
                            if (subencodingMask.AnySubrects)
                            {
                                // Read number-of-subrectangles
                                byte[] numberOfSubrectanglesHextileBuffer = new byte[1];
                                await a_stream.ReadAllAsync(numberOfSubrectanglesHextileBuffer, 0, numberOfSubrectanglesHextileBuffer.Length);
                                byte numberOfSubrectanglesHextile = numberOfSubrectanglesHextileBuffer[0];

                                // Read subrectangle
                                hextileSubrect = new VncEncodeHextileSubrect[numberOfSubrectanglesHextile];
                                if (subencodingMask.SubrectsColored)
                                {
                                    for (int i = 0; i < numberOfSubrectanglesHextile; ++i)
                                    {
                                        byte[] hextileSubrectBuffer = new byte[a_bytesPerPixel + 2];
                                        await a_stream.ReadAllAsync(hextileSubrectBuffer, 0, hextileSubrectBuffer.Length);
                                        hextileSubrect[i] = new VncEncodeHextileSubrect(VncPixelByteGetter.GetPixelByte(a_bytesPerPixel, hextileSubrectBuffer, 0),
                                                                                        hextileSubrectBuffer[a_bytesPerPixel],
                                                                                        hextileSubrectBuffer[a_bytesPerPixel + 1]);
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < numberOfSubrectanglesHextile; ++i)
                                    {
                                        byte[] hextileSubrectBuffer = new byte[2];
                                        await a_stream.ReadAllAsync(hextileSubrectBuffer, 0, hextileSubrectBuffer.Length);
                                        hextileSubrect[i] = new VncEncodeHextileSubrect(foregroundPixel,
                                                                                        hextileSubrectBuffer[a_bytesPerPixel],
                                                                                        hextileSubrectBuffer[a_bytesPerPixel + 1]);
                                    }
                                }
                            }
                            else
                            {
                                // Set empty subrects
                                hextileSubrect = new VncEncodeHextileSubrect[0];
                            }

                            hextiles[index] = new VncEncodeHextileTileRre(backgroundPixel, foregroundPixel, hextileSubrect);
                        }
                    }
                }

                return new VncEncodeHextile(x, y, w, h, hextiles);

            case VncEnum.EncodeType.ZRLE:
                // Read length
                byte[] zrleLengthBuffer = new byte[4];
                await a_stream.ReadAllAsync(zrleLengthBuffer, 0, zrleLengthBuffer.Length);
                UInt32 zrleLength = BigEndianBitConverter.ToUInt32(zrleLengthBuffer, 0);

                // Read zlib data
                byte[] zrleZlibData = new byte[zrleLength];
                await a_stream.ReadAllAsync(zrleZlibData, 0, zrleZlibData.Length);

                return new VncEncodeZrle(x, y, w, h, zrleZlibData, 0);

            case VncEnum.EncodeType.Cursor:
            case VncEnum.EncodeType.DesktopSize:
            default:
                throw new NotSupportedException($"Encode type ({encodeType}) is Not supported.");
            }
        }

        public static List<VncEncodeAbstract> CreateVncEncodeFromBinary(byte[] a_body, byte a_bytesPerPixel, bool a_isBigendian)
        {
            var vncEncodeList = new List<VncEncodeAbstract>();

            // Message Type(1) - Read padding(1) - number-of-rectangles(2)
            UInt16 numberOfRectangle = BigEndianBitConverter.ToUInt16(a_body, 2);

            // Read rectangles (pos = 4 == Message Type(1) + Read padding(1) + number-of-rectangles(2)
            for (int i = 0, pos = 4; i < numberOfRectangle; ++i)
            {
                var dat = createVncEncodeFromBinarySub(a_body, pos, a_bytesPerPixel, a_isBigendian);
                vncEncodeList.Add(dat.encode);
                pos += dat.length;
            }

            return vncEncodeList;
        }

        private static (VncEncodeAbstract encode, int length) createVncEncodeFromBinarySub(byte[] a_body, int a_basePos, byte a_bytesPerPixel, bool a_isBigendian)
        {
            UInt16 x = BigEndianBitConverter.ToUInt16(a_body, a_basePos + 0);
            UInt16 y = BigEndianBitConverter.ToUInt16(a_body, a_basePos + 2);
            UInt16 w = BigEndianBitConverter.ToUInt16(a_body, a_basePos + 4);
            UInt16 h = BigEndianBitConverter.ToUInt16(a_body, a_basePos + 6);
            var encodeType = (VncEnum.EncodeType)BigEndianBitConverter.ToInt32(a_body, a_basePos + 8);

            int offset;
            switch (encodeType)
            {
            case VncEnum.EncodeType.Raw:
                return (new VncEncodeRaw(x, y, w, h, a_body, a_basePos + 12), 12 + w * h * a_bytesPerPixel);

            case VncEnum.EncodeType.CopyRect:
                UInt16 sx = BigEndianBitConverter.ToUInt16(a_body, a_basePos + 12);
                UInt16 sy = BigEndianBitConverter.ToUInt16(a_body, a_basePos + 14);
                return (new VncEncodeCopyRect(x, y, w, h, sx, sy), 12 + 4);

            case VncEnum.EncodeType.RRE:
                // Read header
                UInt32 numberOfSubrectangles = BigEndianBitConverter.ToUInt32(a_body, a_basePos + 12);
                byte[] backgroundPixelValue  = VncPixelByteGetter.GetPixelByte(a_bytesPerPixel, a_body, a_basePos + 16);
                
                // Create subrectangles
                int subrectSize   = a_bytesPerPixel + 8;
                var subrectangles = new VncEncodeRreSubrectangle[numberOfSubrectangles];
                for (int i = 0; i < numberOfSubrectangles; ++i)
                {
                    offset = a_basePos + subrectSize * i;
                    byte[] pixel  = VncPixelByteGetter.GetPixelByte(a_bytesPerPixel, a_body, (16 + a_bytesPerPixel) + 0 + offset);
                    UInt16 xpos   = BigEndianBitConverter.ToUInt16(a_body,             (16 + a_bytesPerPixel + a_bytesPerPixel) + 0 + offset);
                    UInt16 ypos   = BigEndianBitConverter.ToUInt16(a_body,             (16 + a_bytesPerPixel + a_bytesPerPixel) + 2 + offset);
                    UInt16 width  = BigEndianBitConverter.ToUInt16(a_body,             (16 + a_bytesPerPixel + a_bytesPerPixel) + 4 + offset);
                    UInt16 height = BigEndianBitConverter.ToUInt16(a_body,             (16 + a_bytesPerPixel + a_bytesPerPixel) + 6 + offset);
                    subrectangles[i] = new VncEncodeRreSubrectangle(pixel, xpos, ypos, width, height);
                }

                int len = 12 + (4 + a_bytesPerPixel) + (a_bytesPerPixel + 2 + 2 + 2 + 2) * (int)numberOfSubrectangles;
                return (new VncEncodeRre(x, y, w, h, backgroundPixelValue, subrectangles), len);

            case VncEnum.EncodeType.Hextile:
                int xTileCount = (int)Math.Ceiling(w / 16.0);
                int yTileCount = (int)Math.Ceiling(h / 16.0);
                var hextiles = new IVncEncodeHextileTile[xTileCount * yTileCount];

                // Loop for each tile
                offset = a_basePos + 12;
                int index  = 0;
                byte[] backgroundPixel = null;
                byte[] foregroundPixel = null;
                for (int ytile = 0; ytile < yTileCount; ++ytile, ++index)
                {
                    int tileHeight = ((ytile == yTileCount - 1) && ((h % 16) != 0)) ? h % 16 : 16;
                    for (int xtile = 0; xtile < xTileCount; ++xtile, ++index)
                    {
                        int tileWidth = ((xtile == xTileCount - 1) && ((w % 16) != 0)) ? w % 16 : 16;

                        // Read header
                        var subencodingMask = new VncEncodeHextileSubencodingMask(a_body[offset]);
                        ++offset;

                        if (subencodingMask.Raw)
                        {
                            // Raw pixel data
                            hextiles[index] = new VncEncodeHextileTileRaw(a_body, offset);
                            offset += tileWidth * tileHeight * a_bytesPerPixel;
                        }
                        else
                        {
                            // Read BackgroundSpecified if needed
                            if (subencodingMask.BackgroundSpecified)
                            {
                                backgroundPixel = VncPixelByteGetter.GetPixelByte(a_bytesPerPixel, a_body, offset);
                                offset += a_bytesPerPixel;
                            }

                            // Read ForegroundSpecified if needed
                            if (subencodingMask.ForegroundSpecified)
                            {
                                foregroundPixel = VncPixelByteGetter.GetPixelByte(a_bytesPerPixel, a_body, offset);
                                offset += a_bytesPerPixel;
                            }

                            // Subrects
                            VncEncodeHextileSubrect[] hextileSubrect;
                            if (subencodingMask.AnySubrects)
                            {
                                // Read number-of-subrectangles
                                byte numberOfSubrectanglesHextile = a_body[offset];
                                ++offset;

                                // Read subrectangle
                                hextileSubrect = new VncEncodeHextileSubrect[numberOfSubrectanglesHextile];
                                if (subencodingMask.SubrectsColored)
                                {
                                    for (int i = 0; i < numberOfSubrectanglesHextile; ++i)
                                    {
                                        hextileSubrect[i] = new VncEncodeHextileSubrect(VncPixelByteGetter.GetPixelByte(a_bytesPerPixel, a_body, offset),
                                                                                        a_body[offset + a_bytesPerPixel],
                                                                                        a_body[offset + a_bytesPerPixel + 1]);
                                        offset += a_bytesPerPixel + 2;
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < numberOfSubrectanglesHextile; ++i)
                                    {
                                        hextileSubrect[i] = new VncEncodeHextileSubrect(foregroundPixel, a_body[0], a_body[1]);
                                        offset += 2;
                                    }
                                }
                            }
                            else
                            {
                                // Set empty subrects
                                hextileSubrect = new VncEncodeHextileSubrect[0];
                            }

                            hextiles[index] = new VncEncodeHextileTileRre(backgroundPixel, foregroundPixel, hextileSubrect);
                        }
                    }
                }

                return (new VncEncodeHextile(x, y, w, h, hextiles), offset - a_basePos);

            case VncEnum.EncodeType.ZRLE:
                int zrleLength = BigEndianBitConverter.ToInt32(a_body, a_basePos + 12);
                return (new VncEncodeZrle(x, y, w, h, a_body, a_basePos + 12), 4 + zrleLength);

            case VncEnum.EncodeType.Cursor:
            case VncEnum.EncodeType.DesktopSize:
            default:
                throw new NotSupportedException($"Encode type ({encodeType}) is Not supported.");
            }
        }

        public static async Task<List<byte[]>> CreateVncEncodeBinaryFromStream(Stream a_stream, byte a_bytesPerPixel, bool a_isBigendian)
        {
            var readDataList = new List<byte[]>();

            // Read padding(1) - number-of-rectangles(2)
            byte[] head = new byte[3];
            await a_stream.ReadAllAsync(head, 0, head.Length);
            readDataList.Add(head);

            UInt16 numberOfRectangle = BigEndianBitConverter.ToUInt16(head, 1);

            // Read rectangles
            for (int i = 0; i < numberOfRectangle; ++i)
            {
                var encode = await createVncEncodeBinaryFromStreamSub(a_stream, a_bytesPerPixel, a_isBigendian);
                readDataList.AddRange(encode);
            }

            return readDataList;
        }

        private static async Task<List<byte[]>> createVncEncodeBinaryFromStreamSub(Stream a_stream, byte a_bytesPerPixel, bool a_isBigendian)
        {
            // Add the read data to this list.
            var readDataList = new List<byte[]>();

            // Local function for Reading and Storing
            async Task readAsyncAndStore(byte[] a_buffer, int a_offset, int a_count)
            {
                await a_stream.ReadAllAsync(a_buffer, a_offset, a_count);
                readDataList.Add(a_buffer);
            };

            byte[] header = new byte[12];
            await readAsyncAndStore(header, 0, header.Length);

            UInt16 x = BigEndianBitConverter.ToUInt16(header, 0);
            UInt16 y = BigEndianBitConverter.ToUInt16(header, 2);
            UInt16 w = BigEndianBitConverter.ToUInt16(header, 4);
            UInt16 h = BigEndianBitConverter.ToUInt16(header, 6);
            var encodeType = (VncEnum.EncodeType)BigEndianBitConverter.ToInt32(header, 8);

            switch (encodeType)
            {
            case VncEnum.EncodeType.Raw:
                byte[] rawPixel = new byte[w * h * a_bytesPerPixel];
                await readAsyncAndStore(rawPixel, 0, rawPixel.Length);
                break;

            case VncEnum.EncodeType.CopyRect:
                byte[] copyRectPixel = new byte[4];
                await readAsyncAndStore(copyRectPixel, 0, copyRectPixel.Length);
                break;

            case VncEnum.EncodeType.RRE:
                // Read header
                byte[] rreHeader = new byte[4 + a_bytesPerPixel];
                await readAsyncAndStore(rreHeader, 0, rreHeader.Length);

                UInt32 numberOfSubrectangles = BigEndianBitConverter.ToUInt32(rreHeader, 0);

                // Read subrectangles
                int subrectSize = a_bytesPerPixel + 8;
                byte[] rreSubrectangles = new byte[subrectSize * numberOfSubrectangles];
                await readAsyncAndStore(rreSubrectangles, 0, rreSubrectangles.Length);
                break;

            case VncEnum.EncodeType.Hextile:
                int xTileCount = (int)Math.Ceiling(w / 16.0);
                int yTileCount = (int)Math.Ceiling(h / 16.0);

                // Loop for each tile
                int index = 0;
                for (int ytile = 0; ytile < yTileCount; ++ytile, ++index)
                {
                    int tileHeight = ((ytile == yTileCount - 1) && ((h % 16) != 0)) ? h % 16 : 16;
                    for (int xtile = 0; xtile < xTileCount; ++xtile, ++index)
                    {
                        int tileWidth = ((xtile == xTileCount - 1) && ((w % 16) != 0)) ? w % 16 : 16;

                        // Read header
                        byte[] subencodingMaskBuffer = new byte[1];
                        await readAsyncAndStore(subencodingMaskBuffer, 0, subencodingMaskBuffer.Length);
                        var subencodingMask = new VncEncodeHextileSubencodingMask(subencodingMaskBuffer[0]);

                        if (subencodingMask.Raw)
                        {
                            // Read raw pixel data
                            byte[] tilesRawPixel = new byte[tileWidth * tileHeight * a_bytesPerPixel];
                            await readAsyncAndStore(tilesRawPixel, 0, tilesRawPixel.Length);
                        }
                        else
                        {
                            // Read BackgroundSpecified if needed
                            if (subencodingMask.BackgroundSpecified)
                            {
                                byte[] backgroundPixelBuffer = new byte[a_bytesPerPixel];
                                await readAsyncAndStore(backgroundPixelBuffer, 0, backgroundPixelBuffer.Length);
                            }

                            // Read ForegroundSpecified if needed
                            if (subencodingMask.ForegroundSpecified)
                            {
                                byte[] foregroundPixelBuffer = new byte[a_bytesPerPixel];
                                await readAsyncAndStore(foregroundPixelBuffer, 0, foregroundPixelBuffer.Length);
                            }

                            // Subrects
                            if (subencodingMask.AnySubrects)
                            {
                                // Read number-of-subrectangles
                                byte[] numberOfSubrectanglesHextileBuffer = new byte[1];
                                await readAsyncAndStore(numberOfSubrectanglesHextileBuffer, 0, numberOfSubrectanglesHextileBuffer.Length);
                                byte numberOfSubrectanglesHextile = numberOfSubrectanglesHextileBuffer[0];

                                // Read subrectangle
                                if (subencodingMask.SubrectsColored)
                                {
                                    for (int i = 0; i < numberOfSubrectanglesHextile; ++i)
                                    {
                                        byte[] hextileSubrectBuffer = new byte[a_bytesPerPixel + 2];
                                        await readAsyncAndStore(hextileSubrectBuffer, 0, hextileSubrectBuffer.Length);
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < numberOfSubrectanglesHextile; ++i)
                                    {
                                        byte[] hextileSubrectBuffer = new byte[2];
                                        await readAsyncAndStore(hextileSubrectBuffer, 0, hextileSubrectBuffer.Length);
                                    }
                                }
                            }
                        }
                    }
                }

                break;

            case VncEnum.EncodeType.ZRLE:
                // Read length
                byte[] zrleLengthBuffer = new byte[4];
                await readAsyncAndStore(zrleLengthBuffer, 0, zrleLengthBuffer.Length);
                UInt32 zrleLength = BigEndianBitConverter.ToUInt32(zrleLengthBuffer, 0);

                // Read zlib data
                byte[] zrleZlibData = new byte[zrleLength];
                await readAsyncAndStore(zrleZlibData, 0, zrleZlibData.Length);
                readDataList.Add(zrleZlibData);

                break;

            case VncEnum.EncodeType.Cursor:
            case VncEnum.EncodeType.DesktopSize:
            default:
                throw new NotSupportedException($"Encode type ({encodeType}) is Not supported.");
            }

            return readDataList;
        }
    }
}
