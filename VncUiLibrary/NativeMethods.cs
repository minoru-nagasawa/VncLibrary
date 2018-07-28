using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VncUiLibrary
{
    internal class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);
    }
}
