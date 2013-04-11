using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using System.Diagnostics;
using System.Web.Script.Serialization;
using System.Net;
using N2.Configuration;
using System.IO;
using N2.Web;

namespace N2.Persistence.Search
{
	[Service(typeof(IContentSearcher), Replaces = typeof(FindingContentSearcher), Configuration = "remote")]
	public class SearcherClient : IContentSearcher
	{
		private readonly Logger<SearcherClient> logger;
		private IPersister persister;
		private string serverUrl;

		public SearcherClient(IPersister persister, DatabaseSection config)
		{
			this.persister = persister;
			serverUrl = config.Search.Server.Url.Replace("://*", "://localhost");
		}

		public Result<ContentItem> Search(N2.Persistence.Search.Query query)
		{
			if (!query.IsValid())
				return Result<ContentItem>.Empty;

			var result = Request("POST", "items", query.ToJson());

			var lightweightResult = new JavaScriptSerializer().Deserialize<Result<LightweightHitData>>(result);

			return new Result<ContentItem>
			{
				Count = lightweightResult.Count,
				Total = lightweightResult.Total,
				Hits = lightweightResult.Hits.Select(h =>
					new LazyHit<ContentItem>
					{
						Score = h.Score,
						Title = h.Title,
						Url = h.Url,
						ID = h.Content.ID,
						ContentAccessor = persister.Get
					}).ToList()
			};
		}

		private string Request(string httpMethod, string path, string requestBody)
		{
			HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(serverUrl + path);
			hwr.Method = httpMethod;
			hwr.ContentType = "application/json";
			using (var s = hwr.GetRequestStream())
			using (var tw = new StreamWriter(s))
			{
				tw.Write(requestBody);
			}

			using (var wr = hwr.GetResponse())
			using (var s = wr.GetResponseStream())
			using (var sr = new StreamReader(s))
			{
				return sr.ReadToEnd();
			}
		}
	}
}
