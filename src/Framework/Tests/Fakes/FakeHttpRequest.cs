using System;
using System.Collections.Specialized;
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
        public override string PathInfo
        {
            get { return ""; }
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
    }
}
