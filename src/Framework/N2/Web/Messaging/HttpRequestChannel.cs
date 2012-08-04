using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using N2.Engine;

namespace N2.Web.Messaging
{
	[Service(typeof(IChannel))]
	public class HttpRequestChannel : IChannel
	{
		Logger<HttpRequestChannel> logger;

		public void Send(string targetAddress, string messageJson)
		{
			var request = (HttpWebRequest)WebRequest.Create(targetAddress);
			request.ContentType = "application/json";
			request.Method = "POST";
			request.UserAgent = "N2 Message Channel 1.0";

			using (var s = request.GetRequestStream())
			using (var sw = new StreamWriter(s))
			{
				sw.Write(messageJson);
				s.Flush();
			}

			try
			{
				var response = (HttpWebResponse)request.GetResponse();
				if (response.StatusCode != HttpStatusCode.OK)
				{
					logger.ErrorFormat("Failed sending message to {0}, with error {0}", targetAddress, response.StatusDescription);
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex);
			}
		}


		public void Receive(string messageJson)
		{
			if (Received != null)
				Received(this, new ReceivedEventArgs { Message = messageJson });
		}

		public event EventHandler<ReceivedEventArgs> Received;
	}
}
