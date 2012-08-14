using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Web.Messaging
{
	public abstract class ReceiverBase<TMessage> : IReceiver where TMessage : class, new ()
	{
		public string MessageType
		{
			get { return typeof(TMessage).Name; }
		}

		void IReceiver.Receive(Envelope message)
		{
			Receive(message.DeserializeMessage<TMessage>());
		}

		protected abstract void Receive(TMessage message);
	}
}
