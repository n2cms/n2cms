using N2;
using N2.Web;
using N2.Details;

namespace N2.Addons.UITests.Items
{
	[Definition]
	[WithEditableTitle, WithEditableName]
	public class AdaptiveItemPage : ContentItem
	{
		[EditableFreeTextArea("Text", 100)]
		public virtual string Text
		{
			get { return GetDetail("Text", ""); }
			set { SetDetail("Text", value, ""); }
		}

		public override TemplateData FindTemplate(string remainingUrl)
		{
			TemplateData data = base.FindTemplate(remainingUrl);
			if(data.CurrentItem != null && data.CurrentItem != this)
				return data;

			if(string.IsNullOrEmpty(remainingUrl))
				return new TemplateData(this, "~/Addons/UITests/UI/AdaptiveItem.aspx");

			return new TemplateData(this, "~/Addons/UITests/UI/" + remainingUrl + ".aspx");
		}
	}
}
