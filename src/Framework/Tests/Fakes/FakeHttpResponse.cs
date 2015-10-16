using System.IO;
using System.Web;

namespace N2.Tests.Fakes
{
	public class FakeHttpResponse : HttpResponseBase
	{
		public override string ApplyAppPathModifier(string virtualPath)
		{
			return virtualPath;
		}

		public TextWriter output;
		public override TextWriter Output
		{
			get { return output ?? (output = new StringWriter()); }
		}

		public override string Status { get; set; }

		public override int StatusCode { get; set; }

		public override string ContentType { get; set; }

		public override void Write(string s)
		{
			Output.Write(s);
		}

		public override HttpCachePolicyBase Cache
		{
			get
			{
				return new FakeHttpCachePolicy();
			}
		}
	}
}
