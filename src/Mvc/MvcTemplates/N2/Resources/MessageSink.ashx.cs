using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Web.Messaging;
using System.IO;

namespace N2.Management.Resources
{
	/// <summary>
	/// Summary description for MessageSink
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