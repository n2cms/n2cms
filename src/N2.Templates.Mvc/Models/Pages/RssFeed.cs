using System.Collections.Generic;
using System.Linq;
using N2.Collections;
using N2.Details;
using N2.Integrity;
using N2.Templates.Mvc.Services;
using N2.Web;
using N2.Web.Mvc;
using N2.Definitions;

namespace N2.Templates.Mvc.Models.Pages
{
	[PageDefinition("Feed",
		Description = "An RSS feed that outputs an xml with the latest feeds.",
		SortOrder = 260,
		IconUrl = "~/Content/Img/feed.png")]
	[RestrictParents(typeof (IStructuralPage))]
	[WithEditableTitle("Title", 10),
	 WithEditableName("Name", 20)]
	public class RssFeed : AbstractContentPage, IFeed, INode
	{
		[EditableLink("Feed root", 90, ContainerName = Tabs.Content)]
		public virtual ContentItem FeedRoot
		{
			get { return (ContentItem) GetDetail("FeedRoot"); }
			set { SetDetail("FeedRoot", value); }
		}

		[EditableTextBox("Number of items", 100, ContainerName = Tabs.Content)]
		public virtual int NumberOfItems
		{
			get { return (int) (GetDetail("NumberOfItems") ?? 10); }
			set { SetDetail("NumberOfItems", value, 10); }
		}

		[EditableTextBox("Tagline", 110, ContainerName = Tabs.Content)]
		public virtual string Tagline
		{
			get { return (string) (GetDetail("Tagline") ?? string.Empty); }
			set { SetDetail("Tagline", value, string.Empty); }
		}

		[EditableTextBox("Author", 120, ContainerName = Tabs.Content)]
		public virtual string Author
		{
			get { return (string) (GetDetail("Author") ?? string.Empty); }
			set { SetDetail("Author", value, string.Empty); }
		}

		[EditableCheckBox("Visible", 30, ContainerName = Tabs.Advanced)]
		public override bool Visible
		{
			get { return base.Visible; }
			set { base.Visible = value; }
		}

		public override string Url
		{
			get { return base.Url + "?hungry=yes"; }
		}

		public virtual IEnumerable<ISyndicatable> GetItems()
		{
			foreach (ISyndicatable item in N2.Find.Items
				.Where.Detail(SyndicatableDefinitionAppender.SyndicatableDetailName).Eq(true)
				.Filters(GetFilters())
				.OrderBy.Published.Desc
				.Select().Take(NumberOfItems))
			{
				yield return item;
			}
		}

		private ItemFilter[] GetFilters()
		{
			ItemFilter[] filters;
			if (FeedRoot != null)
				filters = new ItemFilter[] {new TypeFilter(typeof (ISyndicatable)), new AccessFilter(), new ParentFilter(FeedRoot)};
			else
				filters = new ItemFilter[] {new TypeFilter(typeof (ISyndicatable)), new AccessFilter()};
			return filters;
		}
	}
}