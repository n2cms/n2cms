using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace N2.Web.UI.WebControls
{
	public class Box : PlaceHolder
	{
		public Box()
		{
			CssClass = "box";
			InnerCssClass = "box-inner";
			HeadingCssClass = "box-heading";
			RenderInnerContainer = true;
		}

		public string HeadingText { get; set; }
		public string HeadingCssClass { get; set; }
		public string CssClass { get; set; }
		public string InnerCssClass { get; set; }
		public bool RenderInnerContainer { get; set; }

		public override void RenderControl(System.Web.UI.HtmlTextWriter writer)
		{
			writer.Write("<div id=\"" + ClientID + "\" class=\"" + CssClass + "\">");
			if (!string.IsNullOrEmpty(HeadingText))
				writer.Write("<h4 class=\"" + HeadingCssClass + "\">" + HeadingText + "</h4>");
			if(RenderInnerContainer)
				writer.Write("<div class=\"" + InnerCssClass + "\">");

			base.RenderControl(writer);

			if(RenderInnerContainer)
				writer.Write("</div>");
			writer.Write("</div>");
		}
	}
}
