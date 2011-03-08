using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;

namespace N2.Web.Mvc.Html
{
	public static class RenderExtensions
	{
		public static DisplayRenderer<DisplayableImageAttribute> Image(this RenderHelper render, string detailName)
		{
			return render.Displayable<DisplayableImageAttribute>(detailName);
		}
		public static DisplayRenderer<DisplayableImageAttribute> Image(this RenderHelper render, string detailName, string preferredSize)
		{
			return render.Displayable(new DisplayableImageAttribute { Name = detailName, PreferredSize = preferredSize });
		}
	}
}