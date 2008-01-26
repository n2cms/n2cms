using System;
using System.Web.UI.WebControls;

namespace N2.TemplateWeb.Domain
{
    //	N2.Details.WithEditable("Title", typeof(TextBox), "Text", 10, "Title", Focus=true),
	//	N2.Details.WithEditable("Name", typeof(N2.Web.UI.WebControls.NameEditor), "Text", 20, "Name"),
	[N2.Details.WithEditableTitle(ContainerName = "default", HelpTitle = "This text is displayed in the menu and header on the page.", HelpText = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Duis lacus nibh, tempor at, pretium id, tincidunt a, tellus. Proin neque arcu, dictum id, auctor id, feugiat a, orci. Donec posuere nisi sit amet mauris. Cras nec ante nec diam eleifend suscipit. Sed lobortis vehicula mauris.")]
	[N2.Details.WithEditableName(ContainerName = "default", HelpTitle = "This text appears as an url segment in the url to this page.")]
    [N2.Details.WithEditable("Sort order of this page or at a pretty long label", typeof(TextBox), "Text", 30, "SortOrder", ContainerName = "default")]
    [N2.Web.UI.TabPanel("links", "Links", 210)]
    [N2.Web.UI.TabPanel("default", "Default", 200)]
    [N2.Web.UI.TabPanel("special", "Special stuff", 220)]
    public abstract class AbstractCustomItem : N2.ContentItem
    {
		public AbstractCustomItem()
		{
			newedDate = DateTime.Now;
		}
		DateTime newedDate;
		public virtual DateTime NewedDate { get{return newedDate;} }
    }
}
