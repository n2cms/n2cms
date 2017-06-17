using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Web.UI;

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

			var engine = Page.GetEngine();
			writer.Write("<div class='btn-group" + (engine.Config.Sections.Management.IsToolbarOnBottom ? " dropup" : "") + "'");
			writer.Write(">");

			AddOnClientClickBehavior(visibleChildren[0]);
			visibleChildren[0].CssClass += " btn " + CssClass;
			visibleChildren[0].RenderControl(writer);
			
			if (visibleChildren.Count > 1)
			{
				writer.Write("<a href='#' class='btn " + CssClass + " dropdown-toggle' data-toggle='dropdown'><span class='caret'></span></a>");
				writer.Write("<ul class='dropdown-menu'>");
				for (int i = 1; i < visibleChildren.Count; i++)
				{
					writer.Write("<li>");
					AddOnClientClickBehavior(visibleChildren[i]);
					visibleChildren[i].RenderControl(writer);
					writer.Write("</li>");					
				}
				writer.Write("</ul>");
			}
			writer.Write("</div>");
		}

		private void AddOnClientClickBehavior(WebControl actionControl)
		{
			if (!string.IsNullOrEmpty(OnClientClick))
				actionControl.Attributes.Add("onclick", OnClientClick);
		}

		public string OnClientClick { get; set; }
	}
}