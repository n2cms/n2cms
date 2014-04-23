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
using N2.Persistence.Search;
using N2.Persistence;

namespace N2.Search.Remote.Client
{
	[Service(typeof(IContentSearcher), Replaces = typeof(FindingContentSearcher), Configuration = "RemoteServer")]
	[Service(typeof(ILightweightSearcher), Replaces = typeof(FindingContentSearcher), Configuration = "RemoteServer")]
    public class SearcherClient : IContentSearcher, ILightweightSearcher
    {
        private readonly Logger<SearcherClient> logger;
        private IPersister persister;
        private string serverUrl;
        private int timeout;
        private string sharedSecret;
        private string instanceName;

        public SearcherClient(IPersister persister, DatabaseSection config)
        {
            this.persister = persister;
            serverUrl = config.Search.Client.Url;
            timeout = config.Search.Client.SearchTimeout;
            sharedSecret = config.Search.Client.SharedSecret;
            instanceName = config.Search.Client.InstanceName;
        }

        Result<LightweightHitData> ISearcher<LightweightHitData>.Search(Query query)
        {
            if (!query.IsValid())
                return Result<LightweightHitData>.Empty;

            var result = Request("POST", "items", query.ToJson());

            return new JavaScriptSerializer().Deserialize<Result<LightweightHitData>>(result);
        }

        public Result<ContentItem> Search(N2.Persistence.Search.Query query)
        {
            if (!query.IsValid())
                return Result<ContentItem>.Empty;

            var result = Request("POST", instanceName, query.ToJson());

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

        private string Request(string httpMethod, string relativePath, string requestBody)
        {
            logger.Debug(httpMethod + " " + serverUrl + relativePath + " (" + requestBody.Length + ")");

            return RemoteExtensions.RequestJson(httpMethod, serverUrl + relativePath, requestBody, timeout: timeout, sharedSecret: sharedSecret);
        }
    }
}
