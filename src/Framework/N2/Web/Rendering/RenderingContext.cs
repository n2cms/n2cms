using System.IO;
using System.Web.Mvc;
using N2.Details;

namespace N2.Web.Rendering
{
	public class RenderingContext
	{
		public HtmlHelper Html { get; set; }
		public ContentItem Content { get; set; }
		public string PropertyName { get; set; }
		public IDisplayable Displayable { get; set; }
	}
}
