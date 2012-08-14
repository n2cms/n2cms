using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit.Web;
using N2.Web.Messaging;
using N2.Engine;
using N2.Web;
using System.Web.Routing;

namespace N2.Management.Users
{
	public class ChatMessage
	{
		public string User { get; set; }
		public string Text { get; set; }
		public DateTime ReceivedDate { get; set; }
	}

	[Service]
	public class MessageBoard
	{
		public MessageBoard()
		{
			Messages = new Queue<ChatMessage>();
		}
		public Queue<ChatMessage> Messages { get; set; }

		public void Truncate(TimeSpan messagesOlderThan)
		{
			lock (this)
			{
				while (Messages.Count > 0 && Utility.CurrentTime().Subtract(Messages.Peek().ReceivedDate) > messagesOlderThan)
				{
					Messages.Dequeue();
				}
			}
		}

		public void Enqueue(ChatMessage message)
		{
			lock (this)
			{
				Messages.Enqueue(message);
			}

			Truncate(TimeSpan.FromHours(1));
		}
	}

	[Receiver]
	public class ChatMessageReceiver : ReceiverBase<ChatMessage>
	{
		private MessageBoard log;

		public ChatMessageReceiver(MessageBoard log)
		{
			this.log = log;
		}
		protected override void Receive(ChatMessage message)
		{
			message.ReceivedDate = Utility.CurrentTime();
			log.Enqueue(message);
		}
	}


	public partial class Activity : EditPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			gvMessages.DataSource = Engine.Resolve<MessageBoard>().Messages;
			gvMessages.DataBind();
		}

		protected void OnSendCommand(object sender, CommandEventArgs args)
		{
			var messageSender = Engine.Resolve<ISender>();
			var message = new ChatMessage { User = User.Identity.Name, Text = txtMessage.Text };
			messageSender.Broadcast(message);

			gvMessages.DataSource = Engine.Resolve<MessageBoard>().Messages.Union(new [] { message });
			gvMessages.DataBind();

		}
	}
}