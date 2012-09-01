using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using N2.Web.Messaging;

namespace N2.Management.Resources
{
	/// <summary>
	/// The messag sink is the default handler on the receiving side for messages sent through <see cref="ISender"/>.
	/// </summary>
	public class MessageSink : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			using (var s = context.Request.InputStream)
			using (var sr = new StreamReader(s))
			{
				Context.Current.Resolve<IChannel>().Receive(sr.ReadToEnd());
			}
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}