using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Web.Drawing;
using System;

namespace N2.Details
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DisplayableImageAttribute : AbstractDisplayableAttribute, IWritingDisplayable, IDisplayable
	{
		private string alt = string.Empty;

		public string Alt
		{
			get { return alt; }
			set { alt = value; }
		}

		/// <summary>The image size to display by default if available.</summary>
		public string PreferredSize { get; set; }

		public override Control AddTo(ContentItem item, string detailName, Control container)
		{
			return AddImage(container, item, detailName, PreferredSize, CssClass, Alt);
		}

		/// <summary>Adds an image control to the container.</summary>
		/// <param name="container">The containing control.</param>
		/// <param name="item">The item containing image informatin.</param>
		/// <param name="detailName">The detail name on the item.</param>
		/// <param name="cssClass">The css class to applky to the image element.</param>
		/// <param name="altText">Alt alternative text to apply to the image element.</param>
		/// <returns>An image control.</returns>
		public static Control AddImage(Control container, ContentItem item, string detailName, string preferredSize, string cssClass, string altText)
		{
			string imageUrl = item[detailName] as string;
			if (!string.IsNullOrEmpty(imageUrl))
			{
				Image image = new Image();
				bool preferredSizeExists;
				image.ImageUrl = ImagesUtility.GetExistingImagePath(imageUrl, preferredSize, out preferredSizeExists);
				image.AlternateText = item.GetDetail(detailName + "_AlternateText", altText);
				image.CssClass = item.GetDetail(detailName + "_CssClass", cssClass);
				if (preferredSizeExists)
					image.CssClass += " " + preferredSize;

				container.Controls.Add(image);
				return image;
			}
			return null;
		}

		/// <summary>Writes an image html to the given writer.</summary>
		/// <param name="item">The item containing the data.</param>
		/// <param name="detailName">The name of the property to write.</param>
		/// <param name="writer">The writer to write to.</param>
		public static void WriteImage(ContentItem item, string detailName, string preferredSize, string alt, string cssClass, System.IO.TextWriter writer)
		{
			string imageUrl = item[detailName] as string;
			if (string.IsNullOrEmpty(imageUrl))
				return;

			TagBuilder tb = new TagBuilder("img");
			bool preferredSizeExists;
			tb.Attributes["src"] = ImagesUtility.GetExistingImagePath(imageUrl, preferredSize, out preferredSizeExists);
			tb.Attributes["alt"] = item.GetDetail(detailName + "_AlternateText", alt);
			cssClass = item.GetDetail(detailName + "_CssClass", cssClass);
			if (preferredSizeExists)
			{
				if(string.IsNullOrEmpty(cssClass))
					cssClass = preferredSize;
				else
					cssClass += " " + preferredSize;
			}

			if (!string.IsNullOrEmpty(cssClass))
				tb.AddCssClass(cssClass);

			writer.Write(tb.ToString(TagRenderMode.SelfClosing));
		}

		#region IWritingDisplayable Members

		public override void Write(ContentItem item, string detailName, System.IO.TextWriter writer)
		{
			DisplayableImageAttribute.WriteImage(item, detailName, PreferredSize, alt, CssClass, writer);
		}

		#endregion
	}
}
