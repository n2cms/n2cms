using System.Text.RegularExpressions;
using N2.Details;
using N2.Templates.Mvc.Services;
using N2.Persistence.Serialization;
using N2.Web.Mvc;
using N2.Definitions;
using N2.Persistence;

namespace N2.Templates.Mvc.Models.Pages
{
    /// <summary>
    /// A page containing textual information.
    /// </summary>
    [PageDefinition("Text Page",
        Description = "A simple text page. It displays a vertical menu, the content and provides a sidebar column",
        SortOrder = 20)]
    public class TextPage : ContentPageBase, IStructuralPage, ISyndicatable
    {
        [EditableMedia("Image", 90, ContainerName = Tabs.Content, CssClass = "main", PreferredSize = "original")]
        public virtual string Image { get; set; }

        [EditableMedia("PDF", 92, ContainerName = Tabs.Content, CssClass = "main", Extensions = ".pdf")]
        public virtual string Pdf { get; set; }

        [Persistable(PersistAs = PropertyPersistenceLocation.Detail)]
        public virtual bool Syndicate { get; set; }
    }
}
