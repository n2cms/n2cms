using N2.Definitions;
using N2.Edit.Trash;
using N2.Installation;
using N2.Persistence.Search;

namespace N2.Edit.Wizard.Items
{
	[PageDefinition("Wizard Container",
		IconUrl = "{ManagementUrl}/Resources/icons/wand.png",
		InstallerVisibility = InstallerHint.NeverRootOrStartPage,
		AuthorizedRoles = new string[0])]
	[Throwable(AllowInTrash.No)]
	[Indexable(IsIndexable = false)]
	public class Wonderland : ContentItem, ISystemNode
	{
		public override bool IsPage
		{
			get { return false; }
		}
	}
}
