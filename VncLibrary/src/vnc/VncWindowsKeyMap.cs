using System;
using System.Collections.Generic;
using System.Text;

namespace VncLibrary
{
    /// <summary>
    /// </summary>
    /// <seealso cref="http://www-higashi.ist.osaka-u.ac.jp/~k-maeda/vcpp/com3-2keycodes.html"/>
    public static class VncWindowsKeyMap
    {
        const uint VK_LBUTTON    = 0x01;
        const uint VK_RBUTTON    = 0x02;
        const uint VK_CANCEL     = 0x03;
        const uint VK_MBUTTON    = 0x04;
        const uint VK_BACK       = 0x08;
        const uint VK_TAB        = 0x09;
        const uint VK_CLEAR      = 0x0C;
        const uint VK_RETURN     = 0x0D;
        const uint VK_SHIFT      = 0x10;
        const uint VK_CONTROL    = 0x11;
        const uint VK_MENU       = 0x12;
        const uint VK_PAUSE      = 0x13;
        const uint VK_CAPITAL    = 0x14;
        const uint VK_KANA       = 0x15;
        const uint VK_KANJI      = 0x19;
        const uint VK_ESCAPE     = 0x1B;
        const uint VK_CONVERT    = 0x1C;
        const uint VK_NONCONVERT = 0x1D;
        const uint VK_SPACE      = 0x20;
        const uint VK_PRIOR      = 0x21;
        const uint VK_NEXT       = 0x22;
        const uint VK_END        = 0x23;
        const uint VK_HOME       = 0x24;
        const uint VK_LEFT       = 0x25;
        const uint VK_UP         = 0x26;
        const uint VK_RIGHT      = 0x27;
        const uint VK_DOWN       = 0x28;
        const uint VK_SELECT     = 0x29;
        const uint VK_PRINT      = 0x2A;
        const uint VK_EXECUTE    = 0x2B;
        const uint VK_SNAPSHOT   = 0x2C;
        const uint VK_INSERT     = 0x2D;
        const uint VK_DELETE     = 0x2E;
        const uint VK_HELP       = 0x2F;
        const uint VK_NUMPAD0    = 0x60;
        const uint VK_NUMPAD1    = 0x61;
        const uint VK_NUMPAD2    = 0x62;
        const uint VK_NUMPAD3    = 0x63;
        const uint VK_NUMPAD4    = 0x64;
        const uint VK_NUMPAD5    = 0x65;
        const uint VK_NUMPAD6    = 0x66;
        const uint VK_NUMPAD7    = 0x67;
        const uint VK_NUMPAD8    = 0x68;
        const uint VK_NUMPAD9    = 0x69;
        const uint VK_MULTIPLY   = 0x6A;
        const uint VK_ADD        = 0x6B;
        const uint VK_SEPARATOR  = 0x6C;
        const uint VK_SUBTRACT   = 0x6D;
        const uint VK_DECIMAL    = 0x6E;
        const uint VK_DIVIDE     = 0x6F;
        const uint VK_F1         = 0x70;
        const uint VK_F2         = 0x71;
        const uint VK_F3         = 0x72;
        const uint VK_F4         = 0x73;
        const uint VK_F5         = 0x74;
        const uint VK_F6         = 0x75;
        const uint VK_F7         = 0x76;
        const uint VK_F8         = 0x77;
        const uint VK_F9         = 0x78;
        const uint VK_F10        = 0x79;
        const uint VK_F11        = 0x7A;
        const uint VK_F12        = 0x7B;
        const uint VK_F13        = 0x7C;
        const uint VK_F14        = 0x7D;
        const uint VK_F15        = 0x7E;
        const uint VK_F16        = 0x7F;
        const uint VK_F17        = 0x80;
        const uint VK_F18        = 0x81;
        const uint VK_F19        = 0x82;
        const uint VK_F20        = 0x83;
        const uint VK_F21        = 0x84;
        const uint VK_F22        = 0x85;
        const uint VK_F23        = 0x86;
        const uint VK_F24        = 0x87;
        const uint VK_NUMLOCK    = 0x90;
        const uint VK_SCROLL     = 0x91;

        private static Dictionary<uint, uint> s_map;
        static VncWindowsKeyMap()
        {
            s_map = new Dictionary<uint, uint>()
            {
                // {VK_LBUTTON     , },
                // {VK_RBUTTON     , },
                // {VK_CANCEL      , },
                // {VK_MBUTTON     , },
                {VK_BACK        , 0xFF08},
                {VK_TAB         , 0xFF09},
                //{VK_CLEAR       , },
                {VK_RETURN      , 0xFF0D},
                {VK_SHIFT       , 0xFFE1},
                {VK_CONTROL     , 0xFFE3},
                {VK_MENU        , 0xFFE9},
                //{VK_PAUSE       , },
                //{VK_CAPITAL     , },
                //{VK_KANA        , },
                //{VK_KANJI       , },
                {VK_ESCAPE      , 0xFF1B},
                //{VK_CONVERT     , },
                //{VK_NONCONVERT  , },
                //{VK_SPACE       , },
                {VK_PRIOR       , 0xFF55}, // Page Up
                {VK_NEXT        , 0xFF56}, // Page Down
                {VK_END         , 0xFF57},
                {VK_HOME        , 0xFF50},
                {VK_LEFT        , 0xFF51},
                {VK_UP          , 0xFF52},
                {VK_RIGHT       , 0xFF53},
                {VK_DOWN        , 0xFF54},
                //{VK_SELECT      , },
                //{VK_PRINT       , },
                //{VK_EXECUTE     , },
                //{VK_SNAPSHOT    , },
                {VK_INSERT      , 0xFF63},
                {VK_DELETE      , 0xFFFF},
                //{VK_HELP        , },
                //{VK_NUMPAD0     , },
                //{VK_NUMPAD1     , },
                //{VK_NUMPAD2     , },
                //{VK_NUMPAD3     , },
                //{VK_NUMPAD4     , },
                //{VK_NUMPAD5     , },
                //{VK_NUMPAD6     , },
                //{VK_NUMPAD7     , },
                //{VK_NUMPAD8     , },
                //{VK_NUMPAD9     , },
                //{VK_MULTIPLY    , },
                //{VK_ADD         , },
                //{VK_SEPARATOR   , },
                //{VK_SUBTRACT    , },
                //{VK_DECIMAL     , },
                //{VK_DIVIDE      , },
                {VK_F1          , 0xFFBE},
                {VK_F2          , 0xFFBF},
                {VK_F3          , 0xFFC0},
                {VK_F4          , 0xFFC1},
                {VK_F5          , 0xFFC2},
                {VK_F6          , 0xFFC3},
                {VK_F7          , 0xFFC4},
                {VK_F8          , 0xFFC5},
                {VK_F9          , 0xFFC6},
                {VK_F10         , 0xFFC7},
                {VK_F11         , 0xFFC8},
                {VK_F12         , 0xFFC9},
                //{VK_F13         , },
                //{VK_F14         , },
                //{VK_F15         , },
                //{VK_F16         , },
                //{VK_F17         , },
                //{VK_F18         , },
                //{VK_F19         , },
                //{VK_F20         , },
                //{VK_F21         , },
                //{VK_F22         , },
                //{VK_F23         , },
                //{VK_F24         , },
                //{VK_NUMLOCK     , },
                //{VK_SCROLL      , },
            };
        }

        public static uint GetVncKey(uint a_windowsKey)
        {
            uint key = 0;
            s_map.TryGetValue(a_windowsKey, out key);
            return key;
        }
    }
}
