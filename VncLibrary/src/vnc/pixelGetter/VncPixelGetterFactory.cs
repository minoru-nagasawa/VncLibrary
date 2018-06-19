using System;
using System.Collections.Generic;
using System.Text;

namespace VncLibrary
{
    static public class VncPixelGetterFactory
    {
        static public IVncPixelGetter CreateVncPixelGetter(PixelFormat a_pixelFormat, VncEnum.EncodeType a_encodeType)
        {
            if (a_pixelFormat.BytesPerPixel == 1)
            {
                return new VncPixelGetter8bits(a_pixelFormat);
            }
            else if (a_pixelFormat.BytesPerPixel == 2)
            {
                return new VncPixelGetter16bits(a_pixelFormat);
            }
            else if (a_pixelFormat.BytesPerPixel == 4)
            {
                if (a_encodeType == VncEnum.EncodeType.ZRLE)
                {
                    return new VncPixelGetterCPIXEL(a_pixelFormat);
                }
                else
                {
                    return new VncPixelGetter32bits(a_pixelFormat);
                }
            }

            throw new NotSupportedException($"Bytes-per-pixel ({a_pixelFormat.BytesPerPixel}) is Not supported.");
        }
    }
}
