using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Web.UI;
using N2.Definitions;
using N2.Persistence;

namespace N2.Addons.UITests.UI
{
    public partial class ContentCreator : ContentUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ddlType.DataSource = Engine.Definitions.GetAllowedChildren(CurrentPage, null)
                .WhereAuthorized(Engine.SecurityManager, Page.User, CurrentPage);
            ddlType.DataBind();
        }

        protected void OnCreateCommand(object sender, CommandEventArgs e)
        {
            ItemDefinition definition = Engine.Definitions.GetDefinition(ddlType.SelectedValue);
            CreateChildren(CurrentPage, definition.ItemType, int.Parse(txtDepth.Text));
            Engine.Persister.Save(CurrentPage);
        }

        private void CreateChildren(ContentItem parent, Type type, int depth)
        {
            if(depth == 0) return;

            int width = int.Parse(txtWidth.Text);
            for (int i = 1; i <= width; i++)
            {
                ContentItem item = Engine.Resolve<ContentActivator>().CreateInstance(type, parent);
                item.Name = txtName.Text + i;
                item.Title = txtName.Text + " " + i;
                item.AddTo(parent);
                CreateChildren(item, type, depth - 1);
            }
        }
    }
}
