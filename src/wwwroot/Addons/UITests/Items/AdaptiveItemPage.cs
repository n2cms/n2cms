using N2;
using N2.Web;

namespace N2.Addons.UITests.Items
{
	[Definition]
	public class AdaptiveItemPage : ContentItem
	{
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
