using System;
using N2.Web;
using N2.Web.UI.WebControls;
using N2.Security;

namespace N2.Edit
{
    [NavigationLinkPlugin("Delete", "delete", "../delete.aspx?selected={selected}&alert=true", Targets.Preview, "~/edit/img/ico/png/cross.png", 30, GlobalResourceClassName = "Navigation")]
    [ToolbarPlugin("DEL", "delete", "delete.aspx?selected={selected}", ToolbarArea.Operations, Targets.Preview, "~/Edit/Img/Ico/png/cross.png", 60, ToolTip = "delete", GlobalResourceClassName = "Toolbar")]
    [ControlPanelLink("cpDelete", "~/Edit/Img/Ico/png/cross.png", "~/Edit/Delete.aspx?selected={Selected.Path}", "Delete this page", 60, ControlPanelState.Visible)]
	public partial class Delete : Web.EditPage
    {
        protected override void OnInit(EventArgs e)
        {
            hlCancel.NavigateUrl = CancelUrl();

            itemsToDelete.CurrentItem = Selection.SelectedItem;
            itemsToDelete.DataBind();
            this.hlReferencingItems.NavigateUrl = "Dependencies.aspx?selected=" + Selection.SelectedItem.Path + "&returnUrl=" + Server.HtmlEncode(Request.RawUrl);

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (N2.Context.UrlParser.IsRootOrStartPage(Selection.SelectedItem))
            {
                cvDelete.IsValid = false;
                this.btnDelete.Enabled = false;
                hlCancel.Focus();
            }
            else
            {
				try
				{
					EnsureAuthorization(Permission.Publish);
				}
				catch (Exception ex)
				{
					cvDelete.IsValid = false;
					cvDelete.ErrorMessage = ex.Message;
					this.btnDelete.Enabled = false;
					return;
				}

                btnDelete.Focus();
            }
            this.Title = string.Format(GetLocalResourceString("DeletePage.TitleFormat"),
                Selection.SelectedItem.Title);

            base.OnLoad(e);
        }

		private void RegisterConfirmAlert()
		{
            string message = string.Format(GetLocalResourceString("confirm.message"), Selection.SelectedItem.Title, Selection.SelectedItem.Url);
			ClientScript.RegisterClientScriptBlock(typeof(Delete), "confirm",
                string.Format(@"jQuery(document).ready( function() {{
	if(confirm('{0}')){{
		{1};
	}}else{{
		window.location='{2}';
	}}
}});", message, ClientScript.GetPostBackClientHyperlink(btnDelete, string.Empty), Selection.SelectedItem.Url), true);
		}

        protected void OnDeleteClick(object sender, EventArgs e)
        {
            ContentItem parent = Selection.SelectedItem.Parent;
            try
            {
                N2.Context.Persister.Delete(Selection.SelectedItem);

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
