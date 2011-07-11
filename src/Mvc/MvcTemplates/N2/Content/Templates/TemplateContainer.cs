using N2.Definitions;
using N2.Edit.Trash;
using N2.Integrity;
using N2.Persistence.Search;

namespace N2.Management.Content.Templates
{
	[PartDefinition("Template Container",
		TemplateUrl = "{ManagementUrl}/Empty.aspx",
		IconUrl = "{ManagementUrl}/Resources/icons/page_white_swoosh.png",
		AuthorizedRoles = new string[0])]
	[AllowedChildren(typeof(ContentItem))]
	[Throwable(AllowInTrash.No)]
	[Indexable(IsIndexable = false)]
	public class TemplateContainer : ContentItem, ISystemNode
	{
	}
}
