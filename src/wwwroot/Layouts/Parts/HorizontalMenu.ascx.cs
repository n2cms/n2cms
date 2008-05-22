using N2.Resources;

namespace N2.Templates.UI.Layouts.Parts
{
	public partial class HorizontalMenu : Web.UI.TemplateUserControl<Templates.Items.AbstractContentPage, Items.LayoutParts.HorizontalMenu>
	{
		protected override void OnInit(System.EventArgs e)
		{
			if (CurrentItem.ExpandingMenu)
			{ 
				Register.JQuery(Page);
				Register.JavaScript(Page, "~/Js/n2menu.js");
				Register.JavaScript(Page, "$('.horizontalMenu > ul').n2menu();", ScriptOptions.DocumentReady);
			}
			base.OnInit(e);
		}
	}
}