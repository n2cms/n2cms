using N2.Definitions;
using N2.Edit.Trash;
using N2.Installation;

namespace N2.Edit.Wizard.Items
{
	[PageDefinition("Wizard Container",
		IconUrl = "{ManagementUrl}/Resources/icons/wand.png",
		InstallerVisibility = InstallerHint.NeverRootOrStartPage)]
	[ItemAuthorizedRoles(Roles = new string[0])]
    [NotThrowable]
	public class Wonderland : ContentItem, ISystemNode
	{
		public override bool IsPage
		{
			get { return false; }
		}
	}
}
