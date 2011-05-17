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
			return CreateDisplayRenderer<T>(new Rendering.RenderingContext
			{
				Content = Content,
				Displayable = displayable,
				Html = Html,
				PropertyName = displayable.Name
			});
		}

		protected virtual DisplayRenderer<T> CreateDisplayRenderer<T>(Rendering.RenderingContext context) where T : IWritingDisplayable
		{
			return new DisplayRenderer<T>(context);
		}
	}
}
