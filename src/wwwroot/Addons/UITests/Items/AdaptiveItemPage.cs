using N2;
using N2.Integrity;
using N2.Web;
using N2.Details;
using N2.Edit.Trash;
using N2.Templates.Items;

namespace N2.Addons.UITests.Items
{

	[RestrictParents(typeof(AdaptiveItemPage))]
	public class AbstractItem : ContentItem
	{
		public override string TemplateUrl
		{
			get { return "~/Addons/UITests/UI/Item.ascx"; }
		}
		public string TypeName
		{
			get { return GetType().Name; }
		}
	}
	[AllowedZones(AllowedZones.All)]
	public class ItemAll : AbstractItem
	{
	}
	[AllowedZones(AllowedZones.AllNamed)]
	public class ItemAllNamed : AbstractItem
	{
	}
	[AllowedZones(AllowedZones.None)]
	public class ItemNone : AbstractItem
	{
	}
	[RestrictParents(typeof(AdaptiveItemPage), typeof(ItemAllNamed))]
	[AllowedZones("Left", "TestZone", "ItemAllNamed")]
	public class ItemSpecifiedZones : AbstractItem
	{
	}

	[Definition("Multiple Tests Page", SortOrder = 10000)]
	[WithEditableTitle, WithEditableName]
	[NotThrowable]
	[RestrictParents(typeof(AbstractContentPage))]
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
