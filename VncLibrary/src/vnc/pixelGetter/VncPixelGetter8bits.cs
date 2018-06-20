using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace VncLibrary
{
    public class VncPixelGetter8bits : IVncPixelGetter
    {
        private Vec3b[] m_colorMap;
        public VncPixelGetter8bits(PixelFormat a_pixelFormat)
        {
            m_colorMap = VncColorLookupTableFactory.GetColorLookupTable(a_pixelFormat);
        }
        public Vec3b GetPixelVec3b(byte[] a_value, int a_offset)
        {
            return m_colorMap[a_value[a_offset]];
        }
        public int GetPixelByteSize()
        {
            return 1;
        }
        public void SetColorMap(VncSetColorMapEntriesBody a_colorMap)
        {
            // If TrueColorFlag is false, m_colorMap is null at the first.
            if (m_colorMap == null)
            {
                m_colorMap = new Vec3b[0xFF + 1];
            }

            // Set new color
            int index = a_colorMap.FirstColor;
            foreach (var rgb in a_colorMap.GetColors())
            {
                m_colorMap[index] = new Vec3b((byte)rgb.R, (byte)rgb.G, (byte)rgb.B);
            }
        }
    }
}
