using System;
using N2.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Templates.UI.Parts
{
    public partial class Teaser : Web.UI.TemplateUserControl<ContentItem, Templates.Items.Teaser>
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (N2.Web.UI.WebControls.ControlPanel.GetState(Page.GetEngine()) == ControlPanelState.Editing)
                hl.NavigateUrl = "";
        }
    }
}
