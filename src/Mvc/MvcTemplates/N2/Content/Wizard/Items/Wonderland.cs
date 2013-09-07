using N2.Definitions;
using N2.Edit.Trash;
using N2.Installation;
using N2.Persistence.Search;
using N2.Integrity;
using N2.Management;

namespace N2.Edit.Wizard.Items
{
	[PartDefinition("Wizard Container",
		IconClass = "n2-icon-magic",
		AuthorizedRoles = new string[0])]
	[Throwable(AllowInTrash.No)]
	[Indexable(IsIndexable = false)]
	[RestrictParents(typeof(IRootPage))]
	public class Wonderland : ManagementItem, ISystemNode
	{
	}
}
