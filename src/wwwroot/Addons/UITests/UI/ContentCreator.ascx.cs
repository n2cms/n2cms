using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Web.UI;
using N2.Definitions;

namespace N2.Addons.UITests.UI
{
	public partial class ContentCreator : ContentUserControl
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			ItemDefinition pageDefinition = Engine.Definitions.GetDefinition(CurrentPage.GetContentType());
			ddlType.DataSource = Engine.Definitions.GetAllowedChildren(pageDefinition, null, Page.User);
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
				ContentItem item = Engine.Definitions.CreateInstance(type, parent);
				item.Name = txtName.Text + i;
				item.Title = txtName.Text + " " + i;
				item.AddTo(parent);
				CreateChildren(item, type, depth - 1);
			}
		}
	}
}