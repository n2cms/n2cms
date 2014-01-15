using System;

namespace N2.Web
{
    public class ErrorEventArgs : EventArgs
    {
        public Exception Error { get; set; }
    }
}
