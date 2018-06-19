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

        public VncConfig(string a_address, int a_port, string a_password, VncEnum.Version a_forceVersion)
        {
            Address  = a_address;
            Port     = a_port;
            Password = a_password;
            ForceVersion = a_forceVersion;
        }
    }
}
