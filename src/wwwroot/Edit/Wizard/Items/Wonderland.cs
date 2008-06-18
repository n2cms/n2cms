using N2.Definitions;
using N2.Installation;
using N2.Definitions.Edit.Trash;

namespace N2.Edit.Wizard.Items
{
	[Definition("Wizard Container", "Wonderland", Installer = InstallerHint.NeverRootOrStartPage)]
	[ItemAuthorizedRoles(Roles = new string[0])]
    [NotThrowable]
    public class Wonderland : ContentItem
	{
		public override bool IsPage
		{
			get { return false; }
		}

		public override string IconUrl
		{
			get { return "~/Edit/Wizard/Img/wand.png"; }
		}
	}
}
