using N2.Definitions;
using N2.Edit.Trash;
using N2.Installation;
using N2.Persistence.Search;
using N2.Integrity;

namespace N2.Edit.Wizard.Items
{
	[PageDefinition("Wizard Container",
		IconUrl = "{ManagementUrl}/Resources/icons/wand.png",
		InstallerVisibility = InstallerHint.NeverRootOrStartPage,
		AuthorizedRoles = new string[0])]
	[Throwable(AllowInTrash.No)]
	[Indexable(IsIndexable = false)]
	[RestrictParents(typeof(IRootPage))]
	public class Wonderland : ContentItem, ISystemNode
	{
		public override bool IsPage
		{
			get { return false; }
		}
	}
}
