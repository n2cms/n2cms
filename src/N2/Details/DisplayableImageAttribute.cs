using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Details
{
	public class DisplayableImageAttribute : AbstractDisplayableAttribute
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
				image.ImageUrl = container.ResolveUrl(imageUrl);
				image.Attributes["alt"] = altText;
				image.CssClass = cssClass;
				container.Controls.Add(image);
				return image;
			}
			return null;
		}
	}
}
