using System;
using N2.Definitions;
using N2.Details;
using N2.Integrity;
using N2.Web.UI;
using System.Web.UI.WebControls;
using N2.Persistence;

namespace N2.Templates.Mvc.Models.Pages
{
    /// <summary>
    /// A page item with a convenient set of properties defined by default.
    /// </summary>
    [WithEditableName("Name", 14, ContainerName = Tabs.Details),
     WithEditablePublishedRange("Published Between", 16, ContainerName = Tabs.Details, BetweenText = " and ")]
    [AvailableZone("Right", Zones.Right), // declaring zones in this way allows managers to add parts from the management UI (not only drag-and-drop)
     AvailableZone("Recursive Right", Zones.RecursiveRight),
     AvailableZone("Left", Zones.Left),
     AvailableZone("Recursive Left", Zones.RecursiveLeft),
     AvailableZone("Content", Zones.Content),
     AvailableZone("Recursive Above", Zones.RecursiveAbove),
     AvailableZone("Recursive Below", Zones.RecursiveBelow)]
    [RestrictParents(typeof (IStructuralPage))]
    [Separator("TitleSeparator", 16, ContainerName = Tabs.Details)]
    public abstract class ContentPageBase : PageBase, ICommentable, IContentPage, IUrlSource
    {
        // editables

        [Persistable]
        [EditableSummary("Summary", 90, ContainerName = Tabs.Content, Source = "Text", HelpTitle = "Text summary used for representing this page in listings", HelpText = "The summary is automaticaly generated from the first lines of text. Optionally it can be overridden with a custom text.")]
        public virtual string Summary { get; set; }

        [EditableFreeTextArea("Text", 100, ContainerName = Tabs.Content)]
        [DisplayableTokens]
        public virtual string Text { get; set; }

        [EditableCheckBox("Visible", 12, ContainerName = Tabs.Details)]
        public override bool Visible
        {
            get { return base.Visible; }
            set { base.Visible = value; }
        }

        [EditableDirectUrl("Direct URL", 15, Placeholder = "e.g. /news", ContainerName = Tabs.Details)]
        public virtual string DirectUrl { get; set; }
    }

    [Obsolete("Use ContentPageBase and [PageDefinition]")]
    public abstract class AbstractContentPage : ContentPageBase
    {
    }
}
