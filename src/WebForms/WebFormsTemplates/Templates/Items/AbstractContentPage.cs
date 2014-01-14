using N2.Details;
using N2.Integrity;
using System;
using N2.Definitions;
using N2.Web.UI;

namespace N2.Templates.Items
{
    /// <summary>
    /// A page item with a convenient set of properties defined by default.
    /// </summary>
    [WithEditableName("Name", 20, ContainerName = Tabs.Content),
        WithEditablePublishedRange("Published Between", 30, ContainerName = Tabs.Advanced, BetweenText = " and ")]
    [AvailableZone("Right", Zones.Right),
        AvailableZone("Recursive Right", Zones.RecursiveRight),
        AvailableZone("Left", Zones.Left),
        AvailableZone("Recursive Left", Zones.RecursiveLeft),
        AvailableZone("Content", Zones.Content),
        AvailableZone("Recursive Above", Zones.RecursiveAbove),
        AvailableZone("Recursive Below", Zones.RecursiveBelow)]
    [RestrictParents(typeof(IStructuralPage))]
    [TabContainer(Tabs.Seo, "SEO", Tabs.SeoIndex)]
    public abstract class AbstractContentPage : AbstractPage, IContentPage
    {
        [EditableFreeTextArea("Text", 100, ContainerName = Tabs.Content)]
        [DisplayableTokens]
        public virtual string Text
        {
            get { return (string) (GetDetail("Text") ?? string.Empty); }
            set { SetDetail("Text", value, string.Empty); }
        }

        [EditableCheckBox("Visible", 40, ContainerName = Tabs.Advanced)]
        public override bool Visible
        {
            get { return base.Visible; }
            set { base.Visible = value; }
        }

        [EditableText(Title = "Page title", ContainerName = Tabs.Seo, HelpTitle = "Displayed in the browser title area and on external search results")]
        public virtual string HeadTitle
        {
            get { return GetDetail("HeadTitle", Title); }
            set { SetDetail("HeadTitle", value, ""); }
        }

        [EditableMetaTag(Title = "Keywords", ContainerName = Tabs.Seo, HelpTitle = "Keywords used to search engine to categorize this page.")]
        public virtual string Keywords
        {
            get { return GetDetail("Keywords", ""); }
            set { SetDetail("Keywords", value, ""); }
        }

        [EditableMetaTag(Title = "Description", ContainerName = Tabs.Seo, HelpTitle = "Description used by search engine to describe this page")]
        public virtual string Description
        {
            get { return GetDetail("Description", ""); }
            set { SetDetail("Description", value, ""); }
        }
    }
}
