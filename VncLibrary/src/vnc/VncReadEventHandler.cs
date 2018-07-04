using System;

namespace VncLibrary
{
    public class VncReadEventArgs : EventArgs
    {
        public VncEnum.MessageTypeServerToClient MessageType
        {
            get;
            private set;
        }
        public VncReadMessageBody Body
        {
            get;
            private set;
        }
        public VncReadEventArgs(VncEnum.MessageTypeServerToClient a_type, VncReadMessageBody a_body)
        {
            MessageType   = a_type;
            Body          = a_body;
        }
    }
    public delegate void VncReadEventHandler(IVncClient a_sender, VncReadEventArgs a_eventArgs);
}
