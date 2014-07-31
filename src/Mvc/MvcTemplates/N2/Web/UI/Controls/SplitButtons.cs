using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace N2.Edit.Web.UI.Controls
{
    public class SplitButtons : HtmlContainerControl
    {
		public SplitButtons()
			: base("li")
		{
			Attributes["class"] = "dropdown splitbutton";
		}

		protected override void Render(HtmlTextWriter writer)
		{
			var visibleControls = Controls.OfType<Control>().Where(c => c.Visible).Where(c => !(c is LiteralControl)).ToList();
			if (visibleControls.Count > 1)
				RenderSplitButtons(visibleControls, writer);
			else
				base.Render(writer);
		}

		private void RenderSplitButtons(List<Control> visibleControls, HtmlTextWriter writer)
		{
			writer.Write("<li class=\"dropdown splitbutton\">");
				visibleControls[0].RenderControl(writer);
				writer.Write("<a href=\"#\" class=\"dropdown-toggle\" data-toggle=\"dropdown\"><span class=\"caret\"></span></a>");
				writer.Write("<ul class=\"dropdown-menu\">");
				for (int i = 1; i < visibleControls.Count; i++)
				{
					writer.Write("<li>");
						visibleControls[i].RenderControl(writer);
					writer.Write("</li>");
				}
				writer.Write("</ul>");
			writer.Write("</li>");
		}
		//  <li class="dropdown splitbutton">
			//<a href="#" class="dropdown-toggle" data-toggle="dropdown"><span class="caret"></span></a>

		
			//<ul class="dropdown-menu">
			//	<li>
    }
}
