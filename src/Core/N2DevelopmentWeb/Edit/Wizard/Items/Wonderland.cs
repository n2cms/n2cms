using N2.Definitions;

namespace N2.Edit.Wizard.Items
{
	[Definition("Wizard container", "Wonderland")]
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
