using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Messaging
{
    public interface IReceiver
    {
        string MessageType { get; }
        void Receive(Envelope message);
    }
}
