using OpenCvSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace VncLibrary
{
    static public class VncColorLookupTableFactory
    {
        static private ConcurrentDictionary<Tuple<int, bool>, Vec3b[]> s_colorMap = new ConcurrentDictionary<Tuple<int, bool>, Vec3b[]>();

        static public Vec3b[] GetColorLookupTable(PixelFormat a_pixelFormat)
        {
            if (a_pixelFormat.BytesPerPixel != 1
            &&  a_pixelFormat.BytesPerPixel != 2)
            {
                throw new NotSupportedException($"Bytes-per-pixel ({a_pixelFormat.BytesPerPixel}) is Not supported.");
            }
            if (!a_pixelFormat.TrueColorFlag)
            {
                return new Vec3b[0];
            }

            Vec3b[] colorMap;
            var key = new Tuple<int, bool>(a_pixelFormat.BytesPerPixel, a_pixelFormat.BigEndianFlag);
            if (s_colorMap.TryGetValue(key, out colorMap))
            {
                return colorMap;
            }

            if (a_pixelFormat.BytesPerPixel == 1)
            {
                int size = 0xFF;
                colorMap = new Vec3b[size + 1];
                for (int i = 0; i <= size; ++i)
                {
                    int r = ((i >> a_pixelFormat.RedShift)   & a_pixelFormat.RedMax  ) * 0xFF / a_pixelFormat.RedMax;
                    int g = ((i >> a_pixelFormat.GreenShift) & a_pixelFormat.GreenMax) * 0xFF / a_pixelFormat.GreenMax;
                    int b = ((i >> a_pixelFormat.BlueShift)  & a_pixelFormat.BlueMax ) * 0xFF / a_pixelFormat.BlueMax;
                    colorMap[i] = new Vec3b((byte)b, (byte)g, (byte)r);
                }

                s_colorMap.TryAdd(key, colorMap);
                return colorMap;
            }
            else if (a_pixelFormat.BytesPerPixel == 2)
            {
                int size = 0xFF;
                colorMap = new Vec3b[(size + 1) * (size + 1)];
                if (a_pixelFormat.BigEndianFlag)
                {
                    int index = 0;
                    for (int i = 0; i <= size; ++i)
                    {
                        for (int j = 0; j <= size; ++j)
                        {
                            int value = (i << 8) | j;
                            int r = ((value >> a_pixelFormat.RedShift)   & a_pixelFormat.RedMax)   * 0xFF / a_pixelFormat.RedMax;
                            int g = ((value >> a_pixelFormat.GreenShift) & a_pixelFormat.GreenMax) * 0xFF / a_pixelFormat.GreenMax;
                            int b = ((value >> a_pixelFormat.BlueShift)  & a_pixelFormat.BlueMax)  * 0xFF / a_pixelFormat.BlueMax;
                            colorMap[index++] = new Vec3b((byte)b, (byte)g, (byte)r);
                        }
                    }
                }
                else
                {
                    int index = 0;
                    for (int i = 0; i <= size; ++i)
                    {
                        for (int j = 0; j <= size; ++j)
                        {
                            int value = i | (j << 8);
                            int r = ((value >> a_pixelFormat.RedShift)   & a_pixelFormat.RedMax)   * 0xFF / a_pixelFormat.RedMax;
                            int g = ((value >> a_pixelFormat.GreenShift) & a_pixelFormat.GreenMax) * 0xFF / a_pixelFormat.GreenMax;
                            int b = ((value >> a_pixelFormat.BlueShift)  & a_pixelFormat.BlueMax)  * 0xFF / a_pixelFormat.BlueMax;
                            colorMap[index++] = new Vec3b((byte)b, (byte)g, (byte)r);
                        }
                    }
                }

                s_colorMap.TryAdd(key, colorMap);
                return colorMap;
            }

            throw new NotSupportedException($"Bytes-per-pixel ({a_pixelFormat.BytesPerPixel}) is Not supported.");
        }
    }
}
