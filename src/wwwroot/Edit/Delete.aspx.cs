using System;
using N2.Web;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
    [NavigationLinkPlugin("Delete", "delete", "../delete.aspx?selected={selected}&alert=true", Targets.Preview, "~/edit/img/ico/delete.gif", 30, GlobalResourceClassName = "Navigation")]
	[ToolbarPlugin("", "delete", "delete.aspx?selected={selected}", ToolbarArea.Preview, Targets.Preview, "~/Edit/Img/Ico/delete.gif", 60, ToolTip = "delete", GlobalResourceClassName = "Toolbar")]
	[ControlPanelLink("cpDelete", "~/Edit/Img/Ico/delete.gif", "~/Edit/Delete.aspx?selected={Selected.Path}", "Delete this page", 60, ControlPanelState.Visible)]
	public partial class Delete : Web.EditPage
    {
        protected override void OnInit(EventArgs e)
        {
            hlCancel.NavigateUrl = CancelUrl();

            itemsToDelete.CurrentItem = SelectedItem;
            itemsToDelete.DataBind();
            this.hlReferencingItems.NavigateUrl = "Dependencies.aspx?selected=" + SelectedItem.Path + "&returnUrl=" + Server.HtmlEncode(Request.RawUrl);

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
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

            base.OnLoad(e);
        }

		private void RegisterConfirmAlert()
		{
			string message = string.Format(GetLocalResourceString("confirm.message"), this.SelectedItem.Title, this.SelectedItem.Url);
			ClientScript.RegisterClientScriptBlock(typeof(Delete), "confirm",
                string.Format(@"jQuery(document).ready( function() {{
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
            try
            {
                N2.Context.Persister.Delete(this.SelectedItem);

                if (parent != null)
                    Refresh(parent, ToolbarArea.Both);
                else
                    Refresh(N2.Context.UrlParser.StartPage, ToolbarArea.Both);
            }
            catch (Exception ex)
            {
                Engine.Resolve<IErrorHandler>().Notify(ex);
                cvException.IsValid = false;
                cvException.Text = ex.Message;
            }
        }
    }
}
