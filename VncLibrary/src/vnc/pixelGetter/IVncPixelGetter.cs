using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace VncLibrary
{
    public interface IVncPixelGetter
    {
        Vec3b GetPixelVec3b(byte[] a_value, int a_offset);
        int   GetPixelByteSize();
        void  SetColorMap(VncSetColorMapEntriesBody a_colorMap);
    }
}
