using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace VncLibrary
{
    public class VncPixelGetter16bits : IVncPixelGetter
    {
        private const int SIZE = 0xFF + 1;
        private Vec3b[] m_colorMap;
        public VncPixelGetter16bits(PixelFormat a_pixelFormat)
        {
            m_colorMap = VncColorLookupTableFactory.GetColorLookupTable(a_pixelFormat);
        }
        public Vec3b GetPixelVec3b(byte[] a_value, int a_offset)
        {
            return m_colorMap[a_value[a_offset] * SIZE + a_value[a_offset + 1]];
        }
        public int GetPixelByteSize()
        {
            return 2;
        }
        public void SetColorMap(VncSetColorMapEntriesBody a_colorMap)
        {
            // If TrueColorFlag is false, m_colorMap is null at the first.
            if (m_colorMap == null)
            {
                m_colorMap = new Vec3b[SIZE * SIZE];
            }

            // Set new color
            int index = a_colorMap.FirstColor;
            foreach (var rgb in a_colorMap.GetColors())
            {
                int x = index >> 8;
                int y = index & 0xFF;
                m_colorMap[x * SIZE + y] = new Vec3b((byte)rgb.R, (byte)rgb.G, (byte)rgb.B);
                ++index;
            }
        }
    }
}
