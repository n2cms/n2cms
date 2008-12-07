using N2;
using N2.Web;
using N2.Details;

namespace N2.Addons.UITests.Items
{
	[Definition("Multiple Tests Page", SortOrder = 10000)]
	[WithEditableTitle, WithEditableName]
	public class AdaptiveItemPage : ContentItem
	{
		[EditableFreeTextArea("Text", 100)]
		public virtual string Text
		{
			get { return GetDetail("Text", ""); }
			set { SetDetail("Text", value, ""); }
		}

		public override PathData FindPath(string remainingUrl)
		{
			PathData data = base.FindPath(remainingUrl);
			if(data.CurrentItem != null && data.CurrentItem != this)
				return data;

			if(string.IsNullOrEmpty(remainingUrl))
				return new PathData(this, "~/Addons/UITests/UI/AdaptiveItem.aspx");

			return new PathData(this, "~/Addons/UITests/UI/" + remainingUrl + ".aspx");
		}
	}
}
