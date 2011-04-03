using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web.UI;
using N2.Details;

namespace N2.Tests.Edit.Items
{
    [FieldSetContainer("hiddenFromEditors", "Naughty naughty", 100, AuthorizedRoles = new string[] { "Administrators" })]
    public class ItemWithSecuredContainer : ContentItem
    {
        [EditableText("Hidden text", 10, ContainerName = "hiddenFromEditors")]
        public string HiddenText { get; set; }
    }
}
