using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace N2.Web.Messaging
{
	public static class MessagingExtensions
	{
		public static void Broadcast<TMessage>(this ISender sender, TMessage simpleObject) where TMessage : class, new()
		{
			sender.Broadcast(typeof(TMessage).Name, new JavaScriptSerializer().Serialize(simpleObject));
		}

		public static void Send<TMessage>(this ISender sender, string targetName, TMessage simpleObject) where TMessage : class, new()
		{
			sender.Send(targetName, typeof(TMessage).Name, new JavaScriptSerializer().Serialize(simpleObject));
		}
	}
}
