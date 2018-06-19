using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace VncLibrary
{
    public class VncPixelGetter8bits : IVncPixelGetter
    {
        #region for IDisposable
        bool m_disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool a_disposing)
        {
            if (m_disposed)
            {
                return;
            }

            if (a_disposing)
            {
                m_colorMap?.Dispose();
            }
            m_disposed = true;
        }
        #endregion

        private MatOfByte3 m_colorMap;
        public VncPixelGetter8bits(PixelFormat a_pixelFormat)
        {
            m_colorMap = VncColorLookupTableFactory.GetColorLookupTable(a_pixelFormat);
        }
        public Vec3b GetPixelVec3b(byte[] a_value, int a_offset)
        {
            return m_colorMap.At<Vec3b>(a_value[a_offset]);
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
                m_colorMap = new MatOfByte3(256, 1);
            }

            // Set new color
            int index = a_colorMap.FirstColor;
            foreach (var rgb in a_colorMap.GetColors())
            {
                m_colorMap.Set<Vec3b>(index, 0, new Vec3b((byte)rgb.R, (byte)rgb.G, (byte)rgb.B));
            }
        }
    }
}
