using N2.Configuration;
using N2.Engine;
using N2.Persistence.Search;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace N2.Search.Remote.Server
{
	public class IndexerServer : IDisposable
	{
		private Logger<IndexerServer> logger;
		private HttpListener listener;
		private IIndexer indexer;
		private ILightweightSearcher searcher;

		public string UriPrefix { get; private set; }

		public IndexerServer(string uriPrefix, IIndexer indexer, ILightweightSearcher searcher)
		{
			Init(uriPrefix, indexer, searcher);
		}

		public IndexerServer(DatabaseSection config)
		{
			if (config == null)
				config = new DatabaseSection();

			var accessor = new LuceneAccesor(new ThreadContext(), config);
			var indexer = new LuceneIndexer(accessor);
			var searcher = new LuceneLightweightSearcher(accessor);
			Init(config.Search.Server.Url, indexer, searcher);
		}

		public IndexerServer()
			: this ((DatabaseSection)ConfigurationManager.GetSection("n2/database"))
		{
		}

		private void Init(string uriPrefix, IIndexer indexer, ILightweightSearcher searcher)
		{
			listener = new HttpListener();
			listener.Prefixes.Add(uriPrefix);

			UriPrefix = uriPrefix;

			this.indexer = indexer;
			this.searcher = searcher;
		}

		public void Start()
		{
			listener.Start();
			listener.BeginGetContext(new AsyncCallback(GetContextCallback), null);
		}

		public void Stop()
		{
			listener.Stop();
		}

		void GetContextCallback(IAsyncResult result)
		{
			if (listener.IsListening)
				listener.BeginGetContext(new AsyncCallback(GetContextCallback), null);
			else
				return;

			HttpListenerContext context = listener.EndGetContext(result);

			try
			{
				logger.DebugFormat("Handling {0} {1} ({2})", context.Request.HttpMethod, context.Request.RawUrl, context.Request.ContentLength64);
				Handle(context);
			}
			catch (Exception ex)
			{
				logger.Error("Error handling " + context.Request.HttpMethod + " " + context.Request.RawUrl + " (" + context.Request.ContentLength64 + ")", ex);
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			}
			finally
			{
				logger.DebugFormat("Ending {0} {1} ({2})", context.Request.HttpMethod, context.Request.RawUrl, context.Request.ContentLength64);
				context.Response.Close();
			}
		}

		private void Handle(HttpListenerContext context)
		{
			context.Response.StatusCode = (int)HttpStatusCode.OK;
			switch (context.Request.HttpMethod)
			{
				case "GET":
					HandleQuery(context);
					break;
				case "PUT":
					HandleReindex(context.Request);
					break;
				case "POST":
					HandleCommand(context);
					break;
				case "DELETE":
					HandleDelete(context.Request);
					break;
				default:
					context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
					break;
			}
		}

		private void HandleQuery(HttpListenerContext context)
		{
			Url url = context.Request.RawUrl;
			if (url.Path == "/index/statistics")
			{
				using (var sw = new StreamWriter(context.Response.OutputStream))
				{
					var json = indexer.GetStatistics().ToJson();
					sw.Write(json);
				}
			}
			if (url.Path == "/items")
			{
				var queryString = context.Request.QueryString;
				var q = Query.For(queryString["q"]);
				if (queryString["below"] != null)
					q.Below(queryString["below"]);
				if (queryString["onlyPages"] != null)
					q.Pages(Convert.ToBoolean(queryString["onlyPages"]));
				if (queryString["orderBy"] != null)
					q.OrderBy(queryString["orderBy"], "true".Equals(queryString["orderByDescending"], StringComparison.InvariantCultureIgnoreCase));
				if (queryString["language"] != null)
					q.Language(queryString["language"]);
				if (queryString["skip"] != null)
					q.Skip(int.Parse("skip"));
				if (queryString["take"] != null)
					q.Take(int.Parse(queryString["take"]));
				if (queryString["type"] != null)
					q.OfType(queryString["type"].Split(','));

				var result = searcher.Search(q);
				WriteQueryResult(result, context.Response);
			}
		}

		private void HandleCommand(HttpListenerContext context)
		{
			Url url = context.Request.RawUrl;

			if (url.Path == "/items")
			{
				var result = GetQueryResult(context.Request);
				WriteQueryResult(result, context.Response);
			}
			if (url.Path == "/index/clear")
			{
				indexer.Clear();
			}
			if (url.Path == "/index/optimize")
			{
				indexer.Optimize();
			}
			if (url.Path == "/index/unlock")
			{
				indexer.Unlock();
			}
		}

		private void HandleReindex(HttpListenerRequest request)
		{
			using (var sr = new StreamReader(request.InputStream))
			{
				var document = new JavaScriptSerializer().Deserialize<IndexableDocument>(sr.ReadToEnd());
				indexer.Update(document);
			}
		}

		private void HandleDelete(HttpListenerRequest request)
		{
			var url = request.RawUrl.ToUrl();
			indexer.Delete(int.Parse(url.Segments.Last()));
		}

		private object GetQueryResult(HttpListenerRequest request)
		{
			using (var sr = new StreamReader(request.InputStream))
			{
				var json = sr.ReadToEnd();
				var query = new JavaScriptSerializer().Deserialize<Query>(json);
				return searcher.Search(query);
			}
		}

		private void WriteQueryResult(object result, HttpListenerResponse response)
		{
			using (var sw = new StreamWriter(response.OutputStream))
			{
				var json = result.ToJson();
				sw.Write(json);
			}
		}

		void IDisposable.Dispose()
		{
			Stop();
		}
	}
}
