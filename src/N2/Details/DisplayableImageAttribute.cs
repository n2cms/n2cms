using System.Web.UI;

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
			return EditableImageAttribute.AddImage(container, item, detailName, CssClass, Alt);
		}
	}
}
