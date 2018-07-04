using System;
using System.Collections.Generic;
using System.Text;

namespace VncLibrary
{
    public class VncReadMessageBody
    {
        public VncEnum.MessageTypeServerToClient MessageType
        {
            get;
            private set;
        }

        public List<VncEncodeAbstract> EncodeList
        {
            get;
            private set;
        }

        public VncSetColorMapEntriesBody ColorMap
        {
            get;
            private set;
        }

        public VncReadMessageBody(VncEnum.MessageTypeServerToClient a_messageType, List<VncEncodeAbstract> a_encodeList, VncSetColorMapEntriesBody a_colorMap)
        {
            MessageType = a_messageType;
            EncodeList  = a_encodeList;
            ColorMap    = a_colorMap;
        }
    }
}
