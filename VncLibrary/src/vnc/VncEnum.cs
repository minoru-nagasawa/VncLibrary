using System;
using System.Collections.Generic;
using System.Text;

namespace VncLibrary
{
    public class VncEnum
    {
        public enum Version
        {
            None,
            Version33,
            Version37,
            Version38
        }

        public enum SecurityType
        {
            Invalid = 0,
            None    = 1,
            VNCAuthentication = 2,
            RA2     = 5,
            RA2ne   = 6,
            Tight   = 16,
            Ultra   = 17,
            TLS     = 18,
            VcNCrypt   = 19,
            GTKVNCSASL = 20,
            MD5     = 21
        }

        public enum SecurityResult
        {
            OK = 0,
            Failed = 1
        }

        public enum SharedFlag
        {
            Exclusive = 0,
            Share     = 1,
        }

        public enum MessageTypeClientToServer
        {
            SetPixelFormat  = 0,
            SetEncodings    = 2,
            FramebufferUpdateRequest = 3,
            KeyEvent        = 4,
            PointerEvent    = 5,
            ClientCutText   = 6,

            // Not supported
            VMWare127       = 127,
            Cloin_Dean_xvp  = 250,
            Pierre_Ossman_SetDesktopSize = 251,
            Tight           = 252,
            Gii             = 253,
            VMWare254       = 254,
            Authony_Liguori = 255,
        }

        public enum MessageTypeServerToClient
        {
            FramebufferUpdate  = 0,
            SetColorMapEntries = 1,
            Bell               = 2,
            ServerCutText      = 3,

            // Not supported
            VMWare127          = 127,
            Cloin_Dean_xvp     = 250,
            Tight              = 252,
            Gii                = 253,
            VMWare254          = 254,
            Authony_Liguori    = 255,
        }

        public enum FramebufferUpdateRequestIncremental
        {
            UpdateAll   = 0,
            Incremental = 1
        }

        public enum EncodeType
        {
            // Not supported
            Cursor      = -239,
            DesktopSize = -223,
            // Supported
            Raw         = 0,
            CopyRect    = 1,
            RRE         = 2,
            Hextile     = 5,
            ZRLE        = 16,
        }
    }
}
