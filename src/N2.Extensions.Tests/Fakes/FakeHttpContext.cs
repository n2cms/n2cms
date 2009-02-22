using System.Web;

namespace N2.Extensions.Tests.Fakes
{
	public class FakeHttpContext : HttpContextBase
	{
		public FakeHttpContext()
		{
			request = new FakeHttpRequest();
			response = new FakeHttpResponse();
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