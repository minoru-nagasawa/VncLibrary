using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace VncLibrary
{
    public class VncPixelGetterCPIXEL : IVncPixelGetter
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
            }
            m_disposed = true;
        }
        #endregion

        private PixelFormat m_pixelFormat;

        public VncPixelGetterCPIXEL(PixelFormat a_pixelFormat)
        {
            m_pixelFormat = a_pixelFormat;
        }

        public Vec3b GetPixelVec3b(byte[] a_value, int a_offset)
        {
            UInt32 value;
            if (m_pixelFormat.BigEndianFlag)
            {
                value = (UInt32) ((a_value[a_offset    ] << 16) |
                                  (a_value[a_offset + 1] <<  8) |
                                  (a_value[a_offset + 2]));
            }
            else
            {
                value = (UInt32) ((a_value[a_offset    ])       |
                                  (a_value[a_offset + 1] <<  8) |
                                  (a_value[a_offset + 2] << 16));
            }

            UInt32 r = (value >> m_pixelFormat.RedShift)   & m_pixelFormat.RedMax;
            UInt32 g = (value >> m_pixelFormat.GreenShift) & m_pixelFormat.GreenMax;
            UInt32 b = (value >> m_pixelFormat.BlueShift)  & m_pixelFormat.BlueMax;

            return new Vec3b((byte)b, (byte)g, (byte)r);
        }
        public int GetPixelByteSize()
        {
            return 3;
        }
        public void SetColorMap(VncSetColorMapEntriesBody a_colorMap)
        {
            throw new NotSupportedException($"VncPixelGetterCPIXEL doe's not support SetColorMap.");
        }
    }
}
