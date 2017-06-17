using System.Web;
using N2.Web;
using System.Security.Principal;
using System.Collections;

namespace N2.Tests.Fakes
{
    public class FakeHttpContext : HttpContextBase
    {
        public FakeHttpContext()
        {
            request = new FakeHttpRequest();
            response = new FakeHttpResponse();
            server = new FakeHttpServerUtility();
            session = new FakeHttpSessionState();
        }
        public FakeHttpContext(Url url)
            : this()
        {
            request.appRelativeCurrentExecutionFilePath = "~" + url.Path;
            foreach (var q in url.GetQueries())
                request.query[q.Key] = q.Value;
            request.rawUrl = url.PathAndQuery;
			if (url.Path.IndexOf(".ashx/") > 0)
				request.pathInfo = url.Path.Substring(url.Path.IndexOf(".ashx/") + 5);
        }
        public Hashtable items = new Hashtable();
        public override System.Collections.IDictionary Items
        {
            get { return items; }
        }
        public FakeHttpRequest request;
        public override HttpRequestBase Request
        {
            get { return request; }
        }
        public FakeHttpResponse response;
        public override HttpResponseBase Response
        {
            get { return response; }
        }
        public FakeHttpServerUtility server;
        public override HttpServerUtilityBase Server
        {
            get { return server; }
        }

        public FakeHttpSessionState session;
        public override HttpSessionStateBase Session
        {
            get { return session; }
        }

        public override void RewritePath(string path)
        {
            RewritePath(path, "", "");
        }
        public override void RewritePath(string path, bool rebaseClientPath)
        {
            RewritePath(path, "", "");
        }
        public override void RewritePath(string filePath, string pathInfo, string queryString)
        {
            request.SetQuery(queryString);
            request.rawUrl = filePath;
            request.appRelativeCurrentExecutionFilePath = Url.ToRelative(filePath);
        }
        private System.Security.Principal.IPrincipal user = new GenericPrincipal(new GenericIdentity(""), new string[0]);
        public override System.Security.Principal.IPrincipal User
        {
            get { return user; }
            set{ user = value; }
        }

        public override System.Web.Caching.Cache Cache
        {
            get { return HttpRuntime.Cache; }
        }

		public override object GetService(System.Type serviceType)
		{
			return null;
		}
    }
}
