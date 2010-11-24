using System.Web;
using System.IO;

namespace N2.Extensions.Tests.Fakes
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
	}
}