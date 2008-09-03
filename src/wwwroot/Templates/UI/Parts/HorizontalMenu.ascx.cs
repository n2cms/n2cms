using N2.Resources;

namespace N2.Templates.UI.Parts
{
    public partial class HorizontalMenu : Web.UI.TemplateUserControl<Templates.Items.AbstractContentPage, Templates.Items.HorizontalMenu>
    {
        protected override void OnInit(System.EventArgs e)
        {
            if (CurrentItem.ExpandingMenu)
            { 
                Resources.Register.JQuery(Page);
                Resources.Register.JavaScript(Page, "~/Js/n2menu.js");
                Resources.Register.JavaScript(Page, "jQuery('.horizontalMenu > ul').n2menu();", ScriptOptions.DocumentReady);
            }
            base.OnInit(e);
        }
    }
}