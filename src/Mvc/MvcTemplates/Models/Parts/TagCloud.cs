using System;
using System.Collections.Generic;
using System.Linq;
using N2.Collections;
using N2.Details;
using N2.Integrity;
using N2.Templates.Mvc.Models.Pages;
using System.Web.UI.WebControls;

namespace N2.Templates.Mvc.Models.Parts
{
    [PartDefinition("Tag cloud",
        Description = "Tag based navigation.",
        SortOrder = 160,
        IconUrl = "{ManagementUrl}/Resources/Icons/tag_blue.png")]
    [WithEditableTitle("Title", 10, Required = false)]
    public class TagCloud : SidebarItem
    {
        [EditableLink("News container", 100)]
        public virtual NewsContainer Container { get; set; }
    }
}
