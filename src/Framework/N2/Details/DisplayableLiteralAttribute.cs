using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace N2.Details
{
	/// <summary>Associate a property/detail with a literal used for presentation.</summary>
	public class DisplayableLiteralAttribute : AbstractDisplayableAttribute, IWritingDisplayable
	{
		public DisplayableLiteralAttribute()
		{
		}

		public override System.Web.UI.Control AddTo(ContentItem item, string detailName, System.Web.UI.Control container)
		{
			object value = item[detailName];
			if(value == null)
				return null;

			if (string.IsNullOrEmpty(CssClass))
			{
				var literal = new LiteralControl(value.ToString());
				container.Controls.Add(literal);
				return literal;
			}
			else
			{
				var span = new HtmlGenericControl("span");
				span.InnerHtml = value.ToString();
				span.Attributes["class"] = CssClass;
				container.Controls.Add(span);
				return span;
			}
		}

		#region IWritingDisplayable Members

		public void Write(ContentItem item, string propertyName, System.IO.TextWriter writer)
		{
			object value = item[propertyName];
			if (value == null)
				return;
			else if (string.IsNullOrEmpty(CssClass))
				writer.Write(value);
			else
				writer.Write("<span class=\"" + CssClass + "\">" + value + "</span>");
		}

		#endregion
	}
}
