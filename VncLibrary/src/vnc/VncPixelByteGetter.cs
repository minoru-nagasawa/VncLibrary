using System;
using System.Collections.Generic;
using System.Text;

namespace VncLibrary
{
    public static class VncPixelByteGetter
    {
        public static byte[] GetPixelByte(byte a_bytesPerPixel, byte[] a_value, int a_startIndex)
        {
            if (a_bytesPerPixel == 1)
            {
                return new byte[1] { a_value[a_startIndex] };
            }
            else if (a_bytesPerPixel == 2)
            {
                return new byte[2] { a_value[a_startIndex], a_value[a_startIndex + 1] };
            }
            else if (a_bytesPerPixel == 4)
            {
                return new byte[4] { a_value[a_startIndex],
                                     a_value[a_startIndex + 1],
                                     a_value[a_startIndex + 2],
                                     a_value[a_startIndex + 3] };
            }

            throw new NotSupportedException($"Bytes-per-pixel ({a_bytesPerPixel}) is Not supported.");
        }
    }
}
