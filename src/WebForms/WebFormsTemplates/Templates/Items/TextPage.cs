using N2.Details;
using N2.Persistence.Serialization;
using N2.Web;
using N2.Definitions;
using N2.Persistence;

namespace N2.Templates.Items
{
    /// <summary>
    /// A page containing textual information.
    /// </summary>
    [PageDefinition("Text Page", 
        Description = "A simple text page. It displays a vertical menu, the content and provides a sidebar column", 
        SortOrder = 20)]
    [ConventionTemplate("Text")]
    [Template("Wide", "~/Templates/UI/Views/WideText.aspx", 
        SelectableAsDefault = true,
        TemplateTitle = "Wide Text Page",
        TemplateDescription = "A simple Text page without sidebar column")]
    [WithEditableTemplateSelection(ContainerName = Tabs.Advanced)]
    public class TextPage : AbstractContentPage, IStructuralPage, ISyndicatable
    {
        [FileAttachment, EditableFileUploadAttribute("Image", 90, ContainerName = Tabs.Content, CssClass = "main")]
        public virtual string Image
        {
            get { return (string)(GetDetail("Image") ?? string.Empty); }
            set { SetDetail("Image", value, string.Empty); }
        }

        public string Summary
        {
            get { return Utility.ExtractFirstSentences(Text, 250); }
        }

        [Persistable(PersistAs = PropertyPersistenceLocation.Detail)]
        public virtual bool Syndicate { get; set; }
    }
}
