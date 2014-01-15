using System;
using System.Collections.Generic;
using N2.Definitions;
using N2.Integrity;

namespace N2.Templates.UI.Parts
{
    [Obsolete]
    public partial class ControlPanel : Web.UI.TemplateUserControl<ContentItem, Templates.Items.ControlPanel>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if(N2.Context.Current.SecurityManager.IsEditor(Page.User))
                DataBind();
            else
                Visible = false;
        }
    }
}
