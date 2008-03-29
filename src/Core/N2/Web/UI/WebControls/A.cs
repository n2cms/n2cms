using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace N2.Web.UI.WebControls
{
	public class A : HtmlAnchor
	{
		public A()
		{
		}
		public A(ILink destination)
		{
			Destination = destination;
		}
		public A(string href, string target, string title, string contents, string cssClass)
		{
			HRef = href;
			Target = target;
			Title = title;
			Contents = contents;
			if(!string.IsNullOrEmpty(cssClass))
				Attributes["class"] = cssClass;
		}

		public ILink Destination
		{
			set
			{
				HRef = value.Url;
				Target = value.Target;
				Title = value.ToolTip;
				Contents = value.Contents;
			}
		}

		public string Contents
		{
			set
			{
				Controls.Clear();
				Controls.Add(new LiteralControl(value));
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (string.IsNullOrEmpty(HRef))
			{
				writer.Write("<span");
				RenderAttributes(writer);
				writer.Write(">");
				RenderChildren(writer);
				writer.Write("</span>");

				//HtmlGenericControl span = new HtmlGenericControl("span");
				//if (!string.IsNullOrEmpty(Title))
				//    span.Attributes["title"] = Title;
				//foreach (KeyValuePair<string, string> pair in Attributes)
				//{
				//    span.Attributes[pair.Key] = pair.Value;
				//}
				//foreach (Control c in Controls)
				//{
				//    span.Controls.Add(c);
				//}
				//return span;
			}
			else
			{
				base.Render(writer);
			}
		}
	}
}
