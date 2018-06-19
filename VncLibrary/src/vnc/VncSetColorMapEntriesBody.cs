using System;
using System.Collections.Generic;
using System.Text;

namespace VncLibrary
{
    public class VncSetColorMapEntriesBody
    {
        public struct RGB
        {
            public UInt16 R { get; private set; }
            public UInt16 G { get; private set; }
            public UInt16 B { get; private set; }
            public RGB(UInt16 a_r, UInt16 a_g, UInt16 a_b)
            {
                R = a_r;
                G = a_g;
                B = a_b;
            }
        }

        public int FirstColor     { get; private set; }
        public int NumberOfColors { get; private set; }

        private List<RGB> m_colors;

        public VncSetColorMapEntriesBody(byte[] a_body)
        {
            // message-type(1) - padding(1) - first-color(2) - number-of-colors(2)
            FirstColor     = BigEndianBitConverter.ToUInt16(a_body, 2);
            NumberOfColors = BigEndianBitConverter.ToUInt16(a_body, 4);

            // Read colors
            m_colors = new List<RGB>();
            for (int i = 0; i < NumberOfColors; ++i)
            {
                UInt16 r = BigEndianBitConverter.ToUInt16(a_body, 6);
                UInt16 g = BigEndianBitConverter.ToUInt16(a_body, 8);
                UInt16 b = BigEndianBitConverter.ToUInt16(a_body, 10);
                m_colors.Add(new RGB(r, g, b));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_colors"></param>
        /// <remarks>This system don't use. It is for testing.</remarks>
        public VncSetColorMapEntriesBody(int a_firstColor, List<RGB> a_colors)
        {
            FirstColor = a_firstColor;
            m_colors   = a_colors;
        }

        public IEnumerable<RGB> GetColors()
        {
            foreach (var v in m_colors)
            {
                yield return v;
            }
        }
    }
}
