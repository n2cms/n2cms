//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using N2.Engine;

//namespace N2.Web.Messaging
//{
//    [Service(typeof(ISender))]
//    public class Sender : ISender
//    {
//        private MessageBroker broker;
//        public Sender(MessageBroker broker)
//        {
//            this.broker = broker;
//        }

//        public string Name
//        {
//            get { return broker.Name; }
//        }

//        public IEnumerable<MessageTarget> Targets 
//        { 
//            get { return broker.Targets; } 
//        }

//        public void Broadcast(string messageType, string message)
//        {
//            broker.Broadcast(messageType, message);
//        }

//        public void Send(string targetName, string messageType, string message)
//        {
//            broker.Send(targetName, messageType, message);
//        }
//    }
//}
