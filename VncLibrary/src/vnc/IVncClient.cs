using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VncLibrary
{
    public interface IVncClient : IDisposable
    {
        bool Connecting     { get; }
        bool Connected      { get; }
        bool Disconnected   { get; }
        VncConfig ClientConfig { get; }
        VncServerInitBody ServerInitBody { get; }

        event VncSimpleEventHandler ConnectedEvent;
        event VncReadEventHandler   ReadEvent;
        event VncCauseEventHandler  ConnectFailedEvent;
        event VncCauseEventHandler  DisconnectedEvent;
        void DisconnectVnc();
        bool ConnectVnc();
        void WriteFramebufferUpdateRequest();
        VncReadMessageBody ReadServerMessage();
        byte[] CreateCanvasImage();
    }
}
