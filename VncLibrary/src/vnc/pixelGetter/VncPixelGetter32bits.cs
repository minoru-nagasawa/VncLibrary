using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace VncLibrary
{
    public class VncPixelGetter32bits : IVncPixelGetter
    {
        private PixelFormat m_pixelFormat;

        public VncPixelGetter32bits(PixelFormat a_pixelFormat)
        {
            m_pixelFormat = a_pixelFormat;
        }

        public Vec3b GetPixelVec3b(byte[] a_value, int a_offset)
        {
            //                     |----------------------|
            //                     | IsLittleEndian       |
            //                     |----------------------|
            //                     | false     | true     |
            // --------------------|-----------|----------|
            // BigEndianFlag false | Convert   | Use as it|
            //               ------|-----------|----------|
            //               true  | Use as it | Convert  |
            // --------------------|-----------|----------|

            // Endian Convert
            UInt32 value;
            if (m_pixelFormat.BigEndianFlag == BitConverter.IsLittleEndian)
            {
                byte[] tmp = new byte[4] { a_value[a_offset + 3],
                                           a_value[a_offset + 2],
                                           a_value[a_offset + 1],
                                           a_value[a_offset    ] };

                value = BitConverter.ToUInt32(tmp, 0);
            }
            else
            {
                value = BitConverter.ToUInt32(a_value, a_offset);
            }

            UInt32 r = (value >> m_pixelFormat.RedShift)   & m_pixelFormat.RedMax;
            UInt32 g = (value >> m_pixelFormat.GreenShift) & m_pixelFormat.GreenMax;
            UInt32 b = (value >> m_pixelFormat.BlueShift)  & m_pixelFormat.BlueMax;

            return new Vec3b((byte)b, (byte)g, (byte)r);
        }
        public int GetPixelByteSize()
        {
            return 4;
        }
        public void SetColorMap(VncSetColorMapEntriesBody a_colorMap)
        {
            throw new NotSupportedException($"VncPixelGetter32bits doe's not support SetColorMap.");
        }
    }
}
