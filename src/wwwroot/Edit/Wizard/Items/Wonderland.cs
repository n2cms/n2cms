using N2.Definitions;
using N2.Installation;

namespace N2.Edit.Wizard.Items
{
	[Definition("Wizard container", "Wonderland", Installer = InstallerHint.NeverRootOrStartPage)]
	[ItemAuthorizedRoles(Roles = new string[0])]
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
