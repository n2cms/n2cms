using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using N2.Details;
using System.Web.Mvc;

namespace N2.Web.Mvc
{
	public class RenderHelper
	{
		public HtmlHelper Html { get; set; }
		public ContentItem Content { get; set; }

		public DisplayRenderer<T> Displayable<T>(string detailName) where T : IWritingDisplayable, new()
		{
			return Displayable(new T() { Name = detailName });
		}

		public DisplayRenderer<T> Displayable<T>(T displayable) where T : IWritingDisplayable
		{
			return new DisplayRenderer<T>(new Rendering.RenderingContext
			{
				Content = Content,
				Displayable = displayable,
				Html = Html,
				PropertyName = displayable.Name
			});
		}
	}

	//public class RendererAdapter : IHtmlString
	//{
	//    public RendererAdapter()
	//    {
	//    }

	//    public ContentItem Content { get; set; }
	//    public string DetailName { get; set; }
	//    public IWritingDisplayable Displayable { get; set; }

	//    #region IHtmlString Members

	//    public string ToHtmlString()
	//    {
	//        using (var tw = new StringWriter())
	//        {
	//            Displayable.Write(Content, DetailName, tw);
	//            return tw.ToHtmlString();
	//        }
	//    }

	//    #endregion
	//}
}
