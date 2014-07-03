using N2.Definitions;
using N2.Edit.Trash;
using N2.Integrity;
using N2.Persistence.Search;

namespace N2.Management.Content.Templates
{
    [PartDefinition("Template Container",
        TemplateUrl = "{ManagementUrl}/Empty.aspx",
        IconClass = "fa fa-plus-sqare-o",
        AuthorizedRoles = new string[0])]
    [AllowedChildren(typeof(ContentItem))]
    [Throwable(AllowInTrash.No)]
    [Indexable(IsIndexable = false)]
    [RestrictParents(typeof(IRootPage))]
	[Versionable(AllowVersions.No)]
    public class TemplateContainer : ContentItem, ISystemNode
    {
    }
}
