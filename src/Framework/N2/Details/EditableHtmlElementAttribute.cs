using System.IO;

namespace N2.Details
{
	public class EditableHtmlElementAttribute  : EditableTextAttribute
	{
		public EditableHtmlElementAttribute()
			: this(null)
		{
		}

		/// <summary>Initializes a new instance of the EditableHtmlElementAttribute class.</summary>
		/// <param name="title">The label displayed to editors</param>
		/// <param name="sortOrder">The order of this editor</param>
		public EditableHtmlElementAttribute(string title, string tagName = "span", int sortOrder = 0)
			: base(title, sortOrder)
		{
			TagName = tagName;
		}

		public string CssClass { get; set; }

		public string TagName { get; set; }

		public string Attributes { get; set; }

		public override void Write(ContentItem item, string propertyName, TextWriter writer)
		{
			writer.Write("<");
			writer.Write(TagName);
			if (!string.IsNullOrEmpty(CssClass))
			{
				writer.Write(" class=\"");
				writer.Write(CssClass);
				writer.Write("\"");
			}
			if (!string.IsNullOrEmpty(Attributes))
			{
				writer.Write(" ");
				writer.Write(Attributes);
			}
			writer.Write(">");

			base.Write(item, propertyName, writer);

			writer.Write("</");
			writer.Write(TagName);
			writer.Write(">");
		}
	}
}