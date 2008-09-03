using System.Collections.Generic;
using N2.Collections;
using N2.Details;
using N2.Integrity;
using N2.Templates.Items;
using N2.Templates.Rss;
using N2.Templates.Syndication;

namespace N2.Templates.Items
{
    [Definition("Feed", "RssFeed", "An RSS feed that outputs an xml with the latest feeds.", "", 260)]
    [RestrictParents(typeof (IStructuralPage))]
    [WithEditableTitle("Title", 10),
     WithEditableName("Name", 20)]
    public class RssFeed : AbstractContentPage, IFeed, INode
    {
        [EditableLink("Feed root", 90)]
        public virtual ContentItem FeedRoot
        {
            get { return (ContentItem)GetDetail("FeedRoot"); }
            set { SetDetail("FeedRoot", value); }
        }

        [EditableTextBox("Number of items", 100)]
        public virtual int NumberOfItems
        {
            get { return (int) (GetDetail("NumberOfItems") ?? 10); }
            set { SetDetail("NumberOfItems", value, 10); }
        }

        [EditableTextBox("Tagline", 110)]
        public virtual string Tagline
        {
            get { return (string) (GetDetail("Tagline") ?? string.Empty); }
            set { SetDetail("Tagline", value, string.Empty); }
        }

        [EditableTextBox("Author", 120)]
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
            get { return base.RewrittenUrl; }
        }

        public virtual IEnumerable<ISyndicatable> GetItems()
        {
            foreach (ISyndicatable item in N2.Find.Items
                .Where.Detail(SyndicatableDefinitionAppender.SyndicatableDetailName).Eq(true)
                .Filters(GetFilters())
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
                filters = new ItemFilter[] { new TypeFilter(typeof(ISyndicatable)), new AccessFilter() };
            return filters;
        }

        protected override string IconName
        {
            get { return "feed"; }
        }

        protected override string TemplateName
        {
            get { return "Feed"; }
        }
    }
}