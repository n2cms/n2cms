using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Messaging
{
    public interface IChannel
    {
        void Send(string targetAddress, string message);
        void Receive(string message);
        event EventHandler<ReceivedEventArgs> Received;
    }
}
