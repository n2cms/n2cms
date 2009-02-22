using System.Web;

namespace N2.Extensions.Tests.Fakes
{
	public class FakeHttpResponse : HttpResponseBase
	{
		public override string ApplyAppPathModifier(string virtualPath)
		{
			return virtualPath;
		}
	}
}