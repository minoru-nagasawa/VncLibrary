using System;

namespace VncLibrary
{
    public class VncCauseEventArgs : EventArgs
    {
        public Exception Cause
        {
            get;
            private set;
        }
        public VncCauseEventArgs(Exception a_ex)
        {
            Cause = a_ex;
        }
    }

    public delegate void VncCauseEventHandler(object sender, VncCauseEventArgs e);
}
