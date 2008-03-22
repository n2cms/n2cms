using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;

namespace N2.Edit
{
	[NavigationPlugIn("Delete", "delete", "../delete.aspx?selected={selected}&alert=true", "preview", "~/edit/img/ico/delete.gif", 30, GlobalResourceClassName="Navigation")]
	[ToolbarPlugIn("", "delete", "delete.aspx?selected={selected}", ToolbarArea.Preview, "preview", "~/Edit/Img/Ico/delete.gif", 60, ToolTip = "delete", GlobalResourceClassName = "Toolbar")]
	public partial class Delete : Web.EditPage
    {
        protected void Page_Init(object sender, EventArgs e)
        {
			hlCancel.NavigateUrl = SelectedItem.Url;
			itemsToDelete.CurrentData = SelectedItem;
            itemsToDelete.DataBind();
			referencingItems.Item = SelectedItem;
			referencingItems.DataBind();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
			if (N2.Context.UrlParser.IsRootOrStartPage(SelectedItem))
			{
				cvDelete.IsValid = false;
				this.btnDelete.Enabled = false;
			}
			else
			{

				if (!IsPostBack && Request["alert"] != null && Boolean.Parse(Request["alert"]))
				{
					RegisterConfirmAlert();
				}
			}
			this.Title = string.Format(GetLocalResourceString("DeletePage.TitleFormat"), 
				SelectedItem.Title);
        }

		private void RegisterConfirmAlert()
		{
			string message = string.Format(GetLocalResourceString("confirm.message"), this.SelectedItem.Title, this.SelectedItem.Url);
			ClientScript.RegisterClientScriptBlock(typeof(Delete), "confirm",
				string.Format(@"$(document).ready( function() {{
	if(confirm('{0}')){{
		{1};
	}}else{{
		window.location='{2}';
	}}
}});", message, ClientScript.GetPostBackClientHyperlink(btnDelete, string.Empty), SelectedItem.Url), true);
		}

        protected void OnDeleteClick(object sender, EventArgs e)
        {
            ContentItem parent = this.SelectedItem.Parent;
			N2.Context.Persister.Delete(this.SelectedItem);

			if (parent != null)
				Refresh(parent, ToolbarArea.Both);
			else
				Refresh(N2.Context.UrlParser.StartPage, ToolbarArea.Both);
        }
    }
}