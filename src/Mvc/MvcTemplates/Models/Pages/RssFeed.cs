using System.Collections.Generic;
using System.Linq;
using N2.Collections;
using N2.Details;
using N2.Integrity;
using N2.Templates.Mvc.Services;
using N2.Web;
using N2.Web.Mvc;
using N2.Definitions;
using N2.Edit;
using N2.Engine;
using N2.Persistence;

namespace N2.Templates.Mvc.Models.Pages
{
    [Adapts(typeof(RssFeed))]
    public class RssFeedNodeAdapter : NodeAdapter
    {
        public override string GetPreviewUrl(ContentItem item, bool allowDraft)
        {
            return new Url(base.GetPreviewUrl(item)).AppendSegment("Preview");;
        }
    }

    [PageDefinition("Feed",
        Description = "An RSS feed that outputs an xml with the latest feeds.",
        SortOrder = 260,
        IconClass = "fa fa-rss")]
    [RestrictParents(typeof (IStructuralPage))]
    [WithEditableTitle("Title", 10),
     WithEditableName("Name", 20)]
    public class RssFeed : ContentPageBase, IFeed, Engine.IInjectable<IRepository<ContentItem>>
    {
        private IRepository<ContentItem> repository;
        [EditableLink("Feed root", 90, ContainerName = Tabs.Content)]
        public virtual ContentItem FeedRoot
        {
            get { return (ContentItem) GetDetail("FeedRoot"); }
            set { SetDetail("FeedRoot", value); }
        }

        [EditableNumber("Number of items", 100, ContainerName = Tabs.Content)]
        public virtual int NumberOfItems
        {
            get { return (int) (GetDetail("NumberOfItems") ?? 10); }
            set { SetDetail("NumberOfItems", value, 10); }
        }

        [EditableText("Tagline", 110, ContainerName = Tabs.Content)]
        public virtual string Tagline
        {
            get { return (string) (GetDetail("Tagline") ?? string.Empty); }
            set { SetDetail("Tagline", value, string.Empty); }
        }

        [EditableText("Author", 120, ContainerName = Tabs.Content)]
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
