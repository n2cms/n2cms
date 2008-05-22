using N2.Integrity;

namespace N2.Edit.Wizard.Items
{
	[Item("Template container", "TemplateContainer")]
	[AllowedParents(AllowedTypes.None)]
	public class TemplateContainer : ContentItem
	{
		public override bool IsPage
		{
			get { return false; }
		}
		public override string IconUrl
		{
			get { return "~/Edit/Wizard/Img/bricks.png"; }
		}
	}
}
