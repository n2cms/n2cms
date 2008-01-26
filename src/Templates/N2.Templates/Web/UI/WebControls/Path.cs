using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using N2.Web.UI;

namespace N2.Templates.Web.UI.WebControls
{
	public class Path : Control
	{

		public string SeparatorText
		{
			get { return (string)(ViewState["SeparatorText"] ?? " / "); }
			set { ViewState["SeparatorText"] = value; }
		}

		protected override void CreateChildControls()
		{
			PrependAnchor(Find.CurrentPage, "current");

			foreach (ContentItem parent in Find.EnumerateParents(Find.CurrentPage, Find.StartPage))
			{
				Controls.AddAt(0, new LiteralControl(SeparatorText));
				PrependAnchor(parent, null);
			}
						
			base.CreateChildControls();
		}

		private void PrependAnchor(ContentItem item, string cssClass)
		{
			Control anchor = N2.Web.Link.To(item).Class(cssClass).ToControl();
			Controls.AddAt(0, anchor);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.Write("<div class='path'>");
			base.Render(writer);
			writer.Write("</div>");
		}
	}
}
