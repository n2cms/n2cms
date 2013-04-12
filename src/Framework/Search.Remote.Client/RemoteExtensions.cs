using N2.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace N2.Search.Remote.Client
{
	public class RemoteExtensions
	{
		public static string RequestJson(string httpMethod, string url, string requestBody, int timeout = 15000)
		{
			HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(url);
			hwr.Method = httpMethod;
			hwr.ContentType = "application/json";
			hwr.Timeout = timeout;

			if (string.IsNullOrEmpty(requestBody))
				hwr.ContentLength = 0;
			else
			{
				using (var s = hwr.GetRequestStream())
				using (var tw = new StreamWriter(s))
				{
					tw.Write(requestBody);
				}
			}

			using (var wr = hwr.GetResponse())
			{
				if (wr.ContentLength == 0)
					return "";

				using (var s = wr.GetResponseStream())
				using (var sr = new StreamReader(s))
				{
					return sr.ReadToEnd();
				}
			}
		}
	}
}
