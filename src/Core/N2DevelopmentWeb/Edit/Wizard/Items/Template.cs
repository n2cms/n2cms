using N2.Integrity;

namespace N2.Edit.Wizard.Items
{
	[Item("Template", "Template")]
	[AllowedChildren(typeof(ContentItem))]
	[AllowedParents(typeof(TemplateContainer))]
	public class Template : ContentItem
	{
		public override bool IsPage
		{
			get { return false; }
		}
		public override string IconUrl
		{
			get { return "~/Edit/Wizard/Img/brick.png"; }
		}
	}
}
