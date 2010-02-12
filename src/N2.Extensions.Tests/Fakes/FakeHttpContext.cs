using System.Web;
using N2.Web;

namespace N2.Extensions.Tests.Fakes
{
	public class FakeHttpContext : HttpContextBase
	{
		public FakeHttpContext()
		{
			request = new FakeHttpRequest();
			response = new FakeHttpResponse();
		}
		public FakeHttpContext(Url url)
			: this()
		{
			request.appRelativeCurrentExecutionFilePath = "~" + url.Path;
			foreach (var q in url.GetQueries())
				request.query[q.Key] = q.Value;
			request.rawUrl = url.PathAndQuery;
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
	}
}