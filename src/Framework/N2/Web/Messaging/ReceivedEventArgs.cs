using System;

namespace N2.Web.Messaging
{
    public class ReceivedEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
}
