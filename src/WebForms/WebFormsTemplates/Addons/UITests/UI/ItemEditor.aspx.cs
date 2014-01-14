using System;
using N2.Templates.Web.UI;
using N2.Edit.Workflow;

namespace N2.Addons.UITests.UI
{
    public partial class ItemEditor : TemplatePage<Items.AdaptiveItemPage>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Request["new"] == "true")
            {
                ie.Initialize(Engine.Definitions.GetDefinition(CurrentItem).Discriminator, null, CurrentItem.Parent);
            }
            else
            {
                ie.CurrentItem = CurrentPage;
            }
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            Engine.Resolve<CommandDispatcher>().Publish(ie.CreateCommandContext());
            Response.Redirect(ie.CurrentItem.Url);
        }
    }
}
