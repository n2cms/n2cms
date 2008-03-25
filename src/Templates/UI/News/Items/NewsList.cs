using N2.Collections;
using N2.Details;
using N2.Integrity;
using N2.Templates.Items;

namespace N2.Templates.News.Items
{
	[Definition("News List", "NewsList", "A news list box that can be displayed in a column.", "", 160)]
	[AllowedZones(Zones.RecursiveRight, Zones.Right, Zones.Left)]
	public class NewsList : SidebarItem
	{
		[EditableLink("News container", 100)]
		public virtual NewsContainer Container
		{
			get { return (NewsContainer) GetDetail("Container"); }
			set { SetDetail("Container", value); }
		}


		[EditableTextBox("Max news", 120)]
		public virtual int MaxNews
		{
			get { return (int) (GetDetail("MaxNews") ?? 3); }
			set { SetDetail("MaxNews", value, 3); }
		}


		public override string TemplateUrl
		{
			get { return "~/News/UI/NewsList.ascx"; }
		}

		public override string IconUrl
		{
			get { return "~/News/UI/Img/newspaper_go.png"; }
		}

		public virtual void Filter(ItemList items)
		{
			TypeFilter.Filter(items, typeof (News));
			CountFilter.Filter(items, 0, MaxNews);
		}
	}
}