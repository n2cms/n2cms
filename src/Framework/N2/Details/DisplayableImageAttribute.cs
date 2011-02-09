using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;

namespace N2.Details
{
	public class DisplayableImageAttribute : AbstractDisplayableAttribute, IWritingDisplayable
	{
		private string alt = string.Empty;

		public string Alt
		{
			get { return alt; }
			set { alt = value; }
		}

		public override Control AddTo(ContentItem item, string detailName, Control container)
		{
			return AddImage(container, item, detailName, CssClass, Alt);
		}

		/// <summary>Adds an image control to the container.</summary>
		/// <param name="container">The containing control.</param>
		/// <param name="item">The item containing image informatin.</param>
		/// <param name="detailName">The detail name on the item.</param>
		/// <param name="cssClass">The css class to applky to the image element.</param>
		/// <param name="altText">Alt alternative text to apply to the image element.</param>
		/// <returns>An image control.</returns>
		public static Control AddImage(Control container, ContentItem item, string detailName, string cssClass, string altText)
		{
			string imageUrl = item[detailName] as string;
			if (!string.IsNullOrEmpty(imageUrl))
			{
				Image image = new Image();
				image.ImageUrl = N2.Web.Url.ToAbsolute(imageUrl);
				image.Attributes["alt"] = altText;
				image.CssClass = cssClass;
				container.Controls.Add(image);
				return image;
			}
			return null;
		}

		/// <summary>Writes an image html to the given writer.</summary>
		/// <param name="item">The item containing the data.</param>
		/// <param name="propertyName">The name of the property to write.</param>
		/// <param name="writer">The writer to write to.</param>
		public static void WriteImage(ContentItem item, string propertyName, string alt, string cssClass, System.IO.TextWriter writer)
		{
			string imageUrl = item[propertyName] as string;
			if (string.IsNullOrEmpty(imageUrl))
				return;

			TagBuilder tb = new TagBuilder("img");
			tb.Attributes["src"] = N2.Web.Url.ToAbsolute(imageUrl);
			tb.Attributes["alt"] = item[propertyName + "_Alt"] as string ?? alt;
			cssClass = item[propertyName + "_CssClass"] as string ?? cssClass;
			if (!string.IsNullOrEmpty(cssClass))
				tb.AddCssClass(cssClass);

			writer.Write(tb.ToString());
		}

		#region IWritingDisplayable Members

		public void Write(ContentItem item, string propertyName, System.IO.TextWriter writer)
		{
			DisplayableImageAttribute.WriteImage(item, propertyName, alt, CssClass, writer);
		}

		#endregion
	}
}
