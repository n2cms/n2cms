using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web.Messaging;
using NUnit.Framework;
using Shouldly;
using System.Web.Script.Serialization;

namespace N2.Tests.Web.Messaging
{
    [TestFixture]
    public class MessagingTests
    {
        private FakeReceiver receiver;
        private FakeChannel channel;
        private MessageBroker broker;
        private N2.Configuration.HostSection config;
        public class TestMessage
        {
            public string Text { get; set; }
        }

        class FakeChannel : IChannel
        {
            public string LastTargetAddress;
            public string LastMessageJson;
            public string LastReceivedMessageJson;

            public void Send(string targetAddress, string messageJson)
            {
                LastTargetAddress = targetAddress;
                LastMessageJson = messageJson;
            }

            public void Receive(string messageJson)
            {
                LastReceivedMessageJson = messageJson;
                if (Received != null)
                    Received(this, new ReceivedEventArgs { Message = messageJson });
            }

            public event EventHandler<ReceivedEventArgs> Received;
        }

        public class FakeReceiver : ReceiverBase<TestMessage> 
        {
            public TestMessage LastMessage;
            protected override void Receive(TestMessage message)
            {
                LastMessage = message;
            }
        }

        [SetUp]
        public void SetUp()
        {
            receiver = new FakeReceiver();
            channel = new FakeChannel();
            config = new N2.Configuration.HostSection();
            config.Messaging.Targets.Add(new N2.Configuration.MessageTargetElement { Name = "Remote", Address = "http://nowhere/" });
            broker = new MessageBroker(channel, new Fakes.FakeWorker(), new[] { receiver }, config);
        }

        [Test]
        public void SentMessage_CanBeReceived()
        {
            broker.Broadcast(new TestMessage { Text = "Hello World" });

            // simulate message going over http
            broker.Receive(channel.LastMessageJson);

            receiver.LastMessage.Text.ShouldBe("Hello World");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void NoSecret_IsNotAllowed()
        {
            var msg = Envelope.Create<TestMessage>(new TestMessage { Text = "Hello World" });
            msg.SenderName = "Unknown";
            broker.Receive(new JavaScriptSerializer().Serialize(msg));
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void InvalidSecret_IsNotAllowed()
        {
            var msg = Envelope.Create<TestMessage>(new TestMessage { Text = "Hello World" });
            msg.SenderName = "Unknown";
            msg.Secret = "some ticket";
            broker.Receive(new JavaScriptSerializer().Serialize(msg));
        }
    }
}
