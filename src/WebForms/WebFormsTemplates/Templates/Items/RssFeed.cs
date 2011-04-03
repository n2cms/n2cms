using System.Collections.Generic;
using N2.Collections;
using N2.Details;
using N2.Integrity;
using N2.Templates.Services;
using N2.Web;
using N2.Definitions;

namespace N2.Templates.Items
{
    [PageDefinition("Feed", 
		Description = "An RSS feed that outputs an xml with the latest feeds.",
		SortOrder = 260,
		IconUrl = "~/Templates/UI/Img/feed.png")]
    [RestrictParents(typeof (IStructuralPage))]
    [WithEditableTitle("Title", 10),
     WithEditableName("Name", 20)]
	[ConventionTemplate("Feed")]
    public class RssFeed : AbstractContentPage, IFeed, INode
    {
        [EditableLink("Feed root", 90)]
        public virtual ContentItem FeedRoot
        {
            get { return (ContentItem)GetDetail("FeedRoot"); }
            set { SetDetail("FeedRoot", value); }
        }

        [EditableText("Number of items", 100)]
        public virtual int NumberOfItems
        {
            get { return (int) (GetDetail("NumberOfItems") ?? 10); }
            set { SetDetail("NumberOfItems", value, 10); }
        }

        [EditableText("Tagline", 110)]
        public virtual string Tagline
        {
            get { return (string) (GetDetail("Tagline") ?? string.Empty); }
            set { SetDetail("Tagline", value, string.Empty); }
        }

        [EditableText("Author", 120)]
        public virtual string Author
        {
            get { return (string) (GetDetail("Author") ?? string.Empty); }
            set { SetDetail("Author", value, string.Empty); }
        }

        [EditableCheckBox("Visible", 30)]
        public override bool Visible
        {
            get { return base.Visible; }
            set { base.Visible = value; }
        }

        public override string Url
        {
            get { return base.Url + "?hungry=yes"; }
        }

        public string PreviewUrl
        {
			get { return base.FindPath(PathData.DefaultAction).RewrittenUrl; }
        }

        public virtual IEnumerable<ISyndicatable> GetItems()
        {
			var query = N2.Find.Items
                .Where.Detail(SyndicatableDefinitionAppender.SyndicatableDetailName).Eq(true);
			if (FeedRoot != null)
				query = query.And.AncestralTrail.Like(Utility.GetTrail(FeedRoot) + "%");

			foreach (ISyndicatable item in query
				.Filters(new TypeFilter(typeof(ISyndicatable)), new AccessFilter())
                .MaxResults(NumberOfItems)
                .OrderBy.Published.Desc
                .Select())
            {
                yield return item;
            }
        }

        private ItemFilter[] GetFilters()
        {
            ItemFilter[] filters;
            if(FeedRoot != null)
                filters = new ItemFilter[] { new TypeFilter(typeof(ISyndicatable)), new AccessFilter(), new ParentFilter(FeedRoot) };
            else
                filters = new ItemFilter[] {  };
            return filters;
        }
    }
}