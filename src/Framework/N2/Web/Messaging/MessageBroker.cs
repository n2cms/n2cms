using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using System.Web.Security;
using System.Web.Script.Serialization;
using N2.Plugin;

namespace N2.Web.Messaging
{
    //[Service]
    [Service(typeof(ISender))]
    public class MessageBroker : ISender, IAutoStart
    {
        Logger<MessageBroker> logger;
        private readonly IWorker worker;
        readonly IChannel channel;
        readonly JavaScriptSerializer serializer = new JavaScriptSerializer();
        private readonly ILookup<string, IReceiver> receivers;

        public MessageBroker(IChannel channel, IWorker worker, IReceiver[] receivers, Configuration.HostSection config)
        {
            this.channel = channel;
            this.worker = worker;
            this.receivers = receivers.ToLookup(r => r.MessageType);

            this.Name = config.Messaging.SenderName;
            this.Enabled = config.Messaging.Enabled;
            this.Async = config.Messaging.Async;
            this.OnlyFromMachineNamed = config.Messaging.Targets.OnlyFromMachineNamed;
            this.SharedSecret = config.Messaging.Targets.SharedSecret;
            this.Targets = config.Messaging.Targets.AllElements.Select(te => new MessageTarget { Name = te.Name, Address = te.Address, ExceptFromMachineNamed = te.ExceptFromMachineNamed }).ToArray();
        }

        public string Name { get; private set; }
        public bool Enabled { get; set; }
        public bool Async { get; set; }
        public string OnlyFromMachineNamed { get; set; }
        public string SharedSecret { get; set; }
        public IEnumerable<MessageTarget> Targets { get; set; }

        public virtual void Broadcast(string messageType, string message)
        {
            logger.InfoFormat("Broadcasting message {0}", message);

            foreach (var target in Targets)
            {
                if (!string.IsNullOrEmpty(target.ExceptFromMachineNamed) && target.ExceptFromMachineNamed == Environment.MachineName)
                {
                    logger.DebugFormat("Not sending message due to except on machine named {0} != {1}", target.ExceptFromMachineNamed, Environment.MachineName);
                    continue;
                }

                if (!string.IsNullOrEmpty(OnlyFromMachineNamed) && OnlyFromMachineNamed != Environment.MachineName)
                {
                    logger.DebugFormat("Not sending message due only on machine named {0} != {1}", OnlyFromMachineNamed, Environment.MachineName);
                    return;
                }

                Send(target, messageType, message);
            }
        }

        public virtual void Send(string targetName, string messageType, string message)
        {
            logger.InfoFormat("Sending message {0} to {1}", message, targetName);

            var target = Targets.FirstOrDefault(t => t.Name == targetName);
            if (target != null)
                Send(target, messageType, message);
            else
                logger.WarnFormat("Unable to find target with name {0} for delivery of message {1}", targetName, message);
        }

        protected void Send(MessageTarget target, string messageType, string message)
        {
            var envelope = GetEnvelope(messageType, message);
            var messageJson = serializer.Serialize(envelope);

            if (Async)
                worker.DoWork(() => channel.Send(target.Address, messageJson));
            else
                channel.Send(target.Address, messageJson);
        }

        protected Envelope GetEnvelope(string messageType, string message)
        {
            return new Envelope(messageType, message)
            {
                SentDate = Utility.CurrentTime(),
                SenderName = Name,
                Secret = GetSecret(),
                MessageType = messageType
            };
        }

        private string GetSecret()
        {
            var secret = string.IsNullOrEmpty(SharedSecret)
                ? FormsAuthentication.Encrypt(new FormsAuthenticationTicket(1, "messaging", N2.Utility.CurrentTime(), N2.Utility.CurrentTime().AddYears(1), false, "authorize me"))
                : SharedSecret;
            return secret;
        }

        public virtual void Receive(string envelopeJson)
        {
            var envelope = serializer.Deserialize<Envelope>(envelopeJson);
            envelope.ReceivedDate = Utility.CurrentTime();

            if (!IsValidSecret(envelope.Secret))
                throw new ArgumentException("Invalid secret, doesn't match the configured secret");
            
            foreach (var receiver in receivers[envelope.MessageType])
            {
                receiver.Receive(envelope);
            }
        }

        private bool IsValidSecret(string secret)
        {
            if (string.IsNullOrEmpty(SharedSecret))
            {
                var ticket = FormsAuthentication.Decrypt(secret);
                return ticket.UserData == "authorize me";
            }
            return secret == SharedSecret;
        }

        public void Start()
        {
            channel.Received += channel_Received;
        }

        void channel_Received(object sender, ReceivedEventArgs e)
        {
            Receive(e.Message);
        }

        public void Stop()
        {
            channel.Received -= channel_Received;
        }
    }
}
