using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace N2.Web.Messaging
{
    public class Envelope
    {
        public Envelope()
        {
        }

        public Envelope(string messageType, string message)
        {
            MessageType = messageType;
            Message = message;
        }

        public static Envelope Create<TMessage>(TMessage simpleObject) where TMessage : class, new()
        {
            return new Envelope(typeof(TMessage).Name, new JavaScriptSerializer().Serialize(simpleObject));
        }

        public string MessageType { get; set; }
        public string Message { get; set; }
        public string SenderName { get; set; }
        public string Secret { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime? ReceivedDate { get; set; }

        public TMessage DeserializeMessage<TMessage>() where TMessage : class, new()
        {
            return new JavaScriptSerializer().Deserialize<TMessage>(Message);
        }
    }
}
