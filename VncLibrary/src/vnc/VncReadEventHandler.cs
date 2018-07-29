using System;

namespace VncLibrary
{
    public class VncReadEventArgs : EventArgs
    {
        public byte[] BodyRaw
        {
            get;
            private set;
        }
        public VncReadMessageBody Body
        {
            get;
            private set;
        }
        public VncReadEventArgs(byte[] a_bodyRaw, VncReadMessageBody a_body)
        {
            BodyRaw = a_bodyRaw;
            Body    = a_body;
        }
    }
    public delegate void VncReadEventHandler(IVncClient a_sender, VncReadEventArgs a_eventArgs);
}
