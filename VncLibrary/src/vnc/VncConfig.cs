using System;
using System.Collections.Generic;
using System.Text;

namespace VncLibrary
{
    public class VncConfig
    {
        public string Address
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public VncEnum.Version ForceVersion
        {
            get;
            set;
        }

        public VncEnum.EncodeType[] Encodings
        {
            get;
            set;
        }

        public bool IsColourSpecified
        {
            get;
            set;
        }

        public PixelFormat SpecifiedColour
        {
            get;
            set;
        }

        public bool IsSendKeyboard
        {
            get;
            set;
        }

        public bool IsSendPointer
        {
            get;
            set;
        }

        public VncConfig(string a_address, int a_port, string a_password, VncEnum.Version a_forceVersion, VncEnum.EncodeType[] a_encodings, bool a_isColourSpecified, PixelFormat a_specifiedColour, bool a_isSendKeyboard, bool a_isSendPointer)
        {
            Address  = a_address;
            Port     = a_port;
            Password = a_password;
            ForceVersion = a_forceVersion;
            Encodings    = a_encodings;
            IsColourSpecified = a_isColourSpecified;
            SpecifiedColour   = a_specifiedColour;
            IsSendKeyboard    = a_isSendKeyboard;
            IsSendPointer     = a_isSendPointer;
        }
    }
}
