using N2.Configuration;
using N2.Engine;
using N2.Persistence.Search;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace N2.Search.Remote.Server
{
    public class IndexerServer : IDisposable
    {
        class InstanceServices
        {
            public IIndexer indexer;
            public ILightweightSearcher searcher;
        }

        private Logger<IndexerServer> logger;
        private HttpListener listener;
        Dictionary<string, InstanceServices> instances = new Dictionary<string, InstanceServices>(StringComparer.InvariantCultureIgnoreCase);
        
        private string sharedSecret;
        private string indexPath;

        public string UriPrefix { get; private set; }

        public IndexerServer(string uriPrefix, string sharedSecret, string indexPath)
        {
            Init(uriPrefix, sharedSecret, indexPath);
        }

        public IndexerServer(DatabaseSection config)
        {
            if (config == null)
                config = new DatabaseSection();

            Init(config.Search.Server.UrlPrefix, config.Search.Server.SharedSecret, config.Search.IndexPath);
        }

        public IndexerServer()
            : this ((DatabaseSection)ConfigurationManager.GetSection("n2/database"))
        {
        }

        private void Init(string uriPrefix, string sharedSecret, string indexPath)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(uriPrefix);

            UriPrefix = uriPrefix;
            this.sharedSecret = sharedSecret;
            if (!indexPath.Contains(":\\"))
                indexPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, indexPath.Trim('~', '/').Replace('/', '\\'));
            this.indexPath = indexPath;
        }
        
        private InstanceServices CreateServices(string instanceName)
        {
            var path = Path.Combine(indexPath, instanceName);
            var accessor = new LuceneAccesor(path);
            var indexer = new LuceneIndexer(accessor);
            var searcher = new LuceneLightweightSearcher(accessor);
            return new InstanceServices { indexer = indexer, searcher = searcher };
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

                if (!string.IsNullOrEmpty(sharedSecret))
                {
                    string sentSecret = GetSecret(context);
                    if (sentSecret != sharedSecret)
                    {
                        logger.WarnFormat("Invalid shared secret {0} {1}", context.Request.HttpMethod, context.Request.RawUrl);
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return;
                    }
                }

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

        private static string GetSecret(HttpListenerContext context)
        {
            var cookie = context.Request.Cookies["Secret"];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                return cookie.Value;
            }
            if (context.Request.QueryString["secret"] != null)
            {
                return context.Request.QueryString["secret"];
            }
            return "";
        }

        private void Handle(HttpListenerContext context)
        {
            if (context.Request.RawUrl.ToUrl().Segments.Length < 1)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            switch (context.Request.HttpMethod)
            {
                case "GET":
                    HandleGet(context);
                    break;
                case "PUT":
                    HandlePut(context);
                    break;
                case "POST":
                    HandlePost(context);
                    break;
                case "DELETE":
                    HandleDelete(context);
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                    break;
            }
        }

        private void HandleGet(HttpListenerContext context)
        {
            var segments = context.Request.RawUrl.ToUrl().Segments;
            if (segments.Length > 1 && segments[1] == "statistics")
            {
                using (var sw = new StreamWriter(context.Response.OutputStream))
                {
                    var json = ResolveServices(context).indexer.GetStatistics().ToJson();
                    sw.Write(json);
                }
            }
            if (segments.Length == 1)
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

                var result = ResolveServices(context).searcher.Search(q);
                WriteQueryResult(result, context.Response);
            }
        }

        private InstanceServices ResolveServices(HttpListenerContext context)
        {
            var instanceName = context.Request.RawUrl.ToUrl().Segments[0];
            InstanceServices services;
            if (instances.TryGetValue(instanceName, out services))
                return services;
            instances[instanceName] = services = CreateServices(instanceName);
            return services;
        }

        private void HandlePost(HttpListenerContext context)
        {
            var segments = context.Request.RawUrl.ToUrl().Segments;
            
            if (segments.Length == 1)
            {
                var result = GetQueryResult(context);
                WriteQueryResult(result, context.Response);
            }
            if (segments.Length > 1 && segments[1] == "clear")
            {
                ResolveServices(context).indexer.Clear();
            }
            if (segments.Length > 1 && segments[1] == "optimize")
            {
                ResolveServices(context).indexer.Optimize();
            }
            if (segments.Length > 1 && segments[1] == "unlock")
            {
                ResolveServices(context).indexer.Unlock();
            }
        }

        private void HandlePut(HttpListenerContext context)
        {
            using (var sr = new StreamReader(context.Request.InputStream))
            {
                var document = new JavaScriptSerializer().Deserialize<IndexableDocument>(sr.ReadToEnd());
                ResolveServices(context).indexer.Update(document);
            }
        }

        private void HandleDelete(HttpListenerContext context)
        {
            var segments = context.Request.RawUrl.ToUrl().Segments;
            if (segments.Length > 1)
                ResolveServices(context).indexer.Delete(int.Parse(segments[1]));
        }

        private object GetQueryResult(HttpListenerContext context)
        {
            using (var sr = new StreamReader(context.Request.InputStream))
            {
                var json = sr.ReadToEnd();
                var query = new JavaScriptSerializer().Deserialize<Query>(json);
                return ResolveServices(context).searcher.Search(query);
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

		public Result<LightweightHitData> Search(string instanceName, string query)
		{
			var q = Query.For(query);
			return CreateServices(instanceName).searcher.Search(q);
		}

		internal IndexStatistics Statistics(string instanceName)
		{
			return CreateServices(instanceName).indexer.GetStatistics();
		}
	}
}
