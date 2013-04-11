using N2.Configuration;
using N2.Engine;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace N2.Persistence.Search
{
	[Service]
	[Service(typeof(IIndexer), Replaces = typeof(EmptyIndexer), Configuration = "remote")]
	public class IndexerClient : IIndexer
	{
		private readonly Engine.Logger<IndexerClient> logger;
		private string serverUrl;

		public IndexerClient(DatabaseSection config)
		{
			serverUrl = config.Search.Server.Url.Replace("://*", "://localhost");
		}

		public void Update(IndexableDocument document)
		{
			Request("PUT", "items/" + document.ID + "/", new JavaScriptSerializer().Serialize(document));
		}

		public void Delete(int itemID)
		{
			Request("DELETE", "items/" + itemID + "/", "");
		}

		public IndexStatistics GetStatistics()
		{
			var response = Request("GET", "index/statistics", "");
			return new JavaScriptSerializer().Deserialize<IndexStatistics>(response);
		}

		public void Clear()
		{
			var response = Request("POST", "index/clear", "");
		}

		public void Optimize()
		{
			var response = Request("POST", "index/optimize", "");
		}

		public void Unlock()
		{
			var response = Request("POST", "index/unlock", "");
		}

		private string Request(string httpMethod, string path, string requestBody)
		{
			HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(serverUrl + path);
			hwr.Method = httpMethod;
			hwr.ContentType = "application/json";
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
