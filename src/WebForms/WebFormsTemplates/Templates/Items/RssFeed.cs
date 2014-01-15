using System.Collections.Generic;
using N2.Collections;
using N2.Details;
using N2.Integrity;
using N2.Templates.Services;
using N2.Web;
using N2.Definitions;
using N2.Engine;
using N2.Edit;
using N2.Persistence;
using System.Linq;

namespace N2.Templates.Items
{

    [Adapts(typeof(RssFeed))]
    public class RssFeedNodeAdapter : NodeAdapter
    {
        public override string GetPreviewUrl(ContentItem item, bool allowDraft)
        {
            return item.FindPath(PathData.DefaultAction).GetRewrittenUrl();
        }
    }

    [PageDefinition("Feed", 
        Description = "An RSS feed that outputs an xml with the latest feeds.",
        SortOrder = 260,
        IconUrl = "~/Templates/UI/Img/feed.png")]
    [RestrictParents(typeof (IStructuralPage))]
    [WithEditableTitle("Title", 10),
     WithEditableName("Name", 20)]
    [ConventionTemplate("Feed")]
    public class RssFeed : AbstractContentPage, IFeed, Engine.IInjectable<IRepository<ContentItem>>
    {
        private IRepository<ContentItem> repository;
        [EditableLink("Feed root", 90)]
        public virtual ContentItem FeedRoot
        {
            get { return (ContentItem)GetDetail("FeedRoot"); }
            set { SetDetail("FeedRoot", value); }
        }

        [EditableNumber("Number of items", 100)]
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

        public virtual IEnumerable<ISyndicatable> GetItems()
        {
            ParameterCollection query = Parameter.Equal(SyndicatableDefinitionAppender.SyndicatableDetailName, true).Detail();
            if (FeedRoot != null)
                query &= Parameter.Below(FeedRoot);
            
            foreach (ISyndicatable item in repository.Find(query.Take(NumberOfItems).OrderBy("Published DESC"))
                .Where(new TypeFilter(typeof(ISyndicatable)) & new AccessFilter())
                .OfType<ISyndicatable>())
            {
                yield return item;
            }
        }

        public void Set(IRepository<ContentItem> dependency)
        {
            repository = dependency;
        }
    }
}
