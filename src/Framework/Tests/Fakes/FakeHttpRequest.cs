using N2.Web;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;

namespace N2.Tests.Fakes
{
    public class FakeHttpRequest : HttpRequestBase
    {
        public string appRelativeCurrentExecutionFilePath = "~/";
        public NameValueCollection query = new NameValueCollection();
        public string rawUrl = "/";

        public override System.Uri Url
        {
            get { return new Uri("http://localhost" + rawUrl, UriKind.RelativeOrAbsolute); }
        }
        public override string AppRelativeCurrentExecutionFilePath
        {
            get { return appRelativeCurrentExecutionFilePath; }
        }
        public override string RawUrl
        {
            get { return rawUrl; }
        }
        public override string this[string key]
        {
            get { return query[key]; }
        }
        public override string ApplicationPath
        {
            get { return "/"; }
        }
		public string pathInfo = "";
		public override string PathInfo
		{
			get { return pathInfo; }
		}
        public override NameValueCollection QueryString
        {
            get { return query; }
        }

        public override string PhysicalPath
        {
            get { return MapPath(appRelativeCurrentExecutionFilePath); }
        }

        public override string MapPath(string virtualPath)
        {
            return Environment.CurrentDirectory + virtualPath.Replace('/', '\\').Trim('~');
        }

        public NameValueCollection serverVariables = new NameValueCollection();
        public override NameValueCollection ServerVariables
        {
            get { return serverVariables; }
        }

        public override void ValidateInput()
        {
        }

        public void SetQuery(string queryString)
        {
            query = new System.Collections.Specialized.NameValueCollection();
            foreach (var kvp in N2.Web.Url.ParseQueryString(queryString))
                query[kvp.Key] = kvp.Value;
        }

		public string input;
		public override System.IO.Stream InputStream
		{
			get 
			{ 
				var ms = new MemoryStream();
				var sw = new StreamWriter(ms);
				sw.Write(input);
				sw.Flush();
				ms.Position = 0;
				return ms; 
			}
		}

		public int contentLength = 0;
		public override int ContentLength
		{
			get { return contentLength; }
		}

		public string httpMethod = "GET";
		public override string HttpMethod
		{
			get { return httpMethod; }
		}

		string contentType;
		public override string ContentType
		{
			get { return contentType; }
			set { contentType = value; }
		}

		public void CreatePost(Url url, string contentType, string input, NameValueCollection queries = null, string pathInfo = null)
		{
			httpMethod = "POST";
			rawUrl = url;
			this.pathInfo = pathInfo;
			query = queries ?? url.GetQueryNameValues();
			ContentType = contentType;
			this.input = input;
			contentLength = input.Length;

		}
	}
}
