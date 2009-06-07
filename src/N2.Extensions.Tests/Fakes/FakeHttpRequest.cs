using System.Collections.Specialized;
using System.Web;

namespace N2.Extensions.Tests.Fakes
{
	public class FakeHttpRequest : HttpRequestBase
	{
		public string appRelativeCurrentExecutionFilePath;
		public override string AppRelativeCurrentExecutionFilePath
		{
			get { return appRelativeCurrentExecutionFilePath; }
		}
		public string rawUrl;
		public override string RawUrl
		{
			get { return rawUrl; }
		}
		public StringDictionary query = new StringDictionary();

		public override string this[string key]
		{
			get { return query[key]; }
		}
		public override string ApplicationPath
		{
			get { return "/"; }
		}

		public NameValueCollection serverVariables = new NameValueCollection();
		public override NameValueCollection ServerVariables
		{
			get
			{
				return serverVariables;
			}
		}
	}
}