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

		public DisplayRenderer<T> Displayable<T>(string detailName) where T : IDisplayable, new()
		{
			return Displayable(new T() { Name = detailName });
		}

		public DisplayRenderer<T> Displayable<T>(T displayable) where T : IDisplayable
		{
			return CreateDisplayRenderer<T>(new Rendering.RenderingContext
			{
				Content = Content,
				Displayable = displayable,
				Html = Html,
				PropertyName = displayable.Name
			});
		}

		protected virtual DisplayRenderer<T> CreateDisplayRenderer<T>(Rendering.RenderingContext context) where T : IDisplayable
		{
			return new DisplayRenderer<T>(context);
		}
	}
}
