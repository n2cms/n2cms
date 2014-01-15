using N2.Details;
using N2.Web.UI;

namespace N2.Tests.Edit.Items
{
    [FieldSetContainer("hiddenFromEditors", "Naughty naughty", 100, AuthorizedRoles = new string[] { "Administrators" })]
    public class ItemWithSecuredContainer : ContentItem
    {
        [EditableText("Hidden text", 10, ContainerName = "hiddenFromEditors")]
        public string HiddenText { get; set; }
    }
}
