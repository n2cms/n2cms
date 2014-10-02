using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Edit.Web.UI.Controls
{
	public class ButtonGroup : Control
	{
		public string CssClass { get; set; }

		protected override void Render(HtmlTextWriter writer)
		{
			var visibleChildren = Controls.OfType<WebControl>().Where(c => c.Visible).ToList();

			if (visibleChildren.Count == 0)
			{
				writer.Write("<!-- no visible controls -->");
				return;
			}

			writer.Write("<div class='btn-group dropup'>");
			
			visibleChildren[0].CssClass += " btn " + CssClass;
			visibleChildren[0].RenderControl(writer);
			
			if (visibleChildren.Count > 1)
			{
				writer.Write("<a href='#' class='btn " + CssClass + " dropdown-toggle' data-toggle='dropdown'><span class='caret'></span></a>");
				writer.Write("<ul class='dropdown-menu'>");
				for (int i = 1; i < visibleChildren.Count; i++)
				{
					writer.Write("<li>");
					visibleChildren[i].RenderControl(writer);
					writer.Write("</li>");					
				}
				writer.Write("</ul>");
			}
			writer.Write("</div>");
		}
	}
}