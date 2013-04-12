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
using N2.Search.Remote.Client;
using N2.Persistence.Search;

namespace N2.Search.Remote.Client
{
	[Service]
	[Service(typeof(IIndexer), Replaces = typeof(EmptyIndexer), Configuration = "remote")]
	public class IndexerClient : IIndexer
	{
		private readonly Engine.Logger<IndexerClient> logger;
		private string serverUrl;
		private int timeout;
		private string sharedSecret;

		public IndexerClient(DatabaseSection config)
		{
			serverUrl = config.Search.Client.Url;
			timeout = config.Search.Client.IndexTimeout;
			sharedSecret = config.Search.Client.SharedSecret;
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

		private string Request(string httpMethod, string relativePath, string requestBody)
		{
			logger.Debug(httpMethod + " " + serverUrl + relativePath + " (" + requestBody.Length + ")");

			return RemoteExtensions.RequestJson(httpMethod, serverUrl + relativePath, requestBody, timeout: timeout, sharedSecret: sharedSecret);
		}
	}
}
