using System;
using N2.Persistence.Finder;
using N2.Security;
using N2.Web;
using N2.Web.UI.WebControls;
using N2.Edit.Activity;
using N2.Management.Activity;

namespace N2.Edit
{
	[NavigationLinkPlugin("Delete", "delete", "{ManagementUrl}/Content/delete.aspx?{Selection.SelectedQueryKey}={selected}&alert=true", Targets.Preview, "{ManagementUrl}/Resources/icons/cross.png", 30, 
		GlobalResourceClassName = "Navigation",
		RequiredPermission = Permission.Publish)]
	[ToolbarPlugin("DEL", "delete", "{ManagementUrl}/Content/delete.aspx?{Selection.SelectedQueryKey}={selected}", ToolbarArea.Operations, Targets.Preview, "{ManagementUrl}/Resources/icons/cross.png", 60, ToolTip = "delete",
		GlobalResourceClassName = "Toolbar",
		RequiredPermission = Permission.Publish)]
	[ControlPanelLink("cpDelete", "{ManagementUrl}/Resources/icons/cross.png", "{ManagementUrl}/Content/Delete.aspx?{Selection.SelectedQueryKey}={Selected.Path}", "Delete this page", 60, ControlPanelState.Visible,
		RequiredPermission = Permission.Publish)]
	public partial class Delete : Web.EditPage
    {
        protected override void OnInit(EventArgs e)
        {
            itemsToDelete.CurrentItem = Selection.SelectedItem;
            itemsToDelete.DataBind();

			var q = Engine.Resolve<IItemFinder>()
				.Where.Detail(LinkTracker.Tracker.LinkDetailName)
				.Like(Selection.SelectedItem.Url)
				.And.State.NotEq(ContentState.Deleted);
			if (Selection.SelectedItem.ID != 0)
				q = q.Or.Detail().Eq(Selection.SelectedItem);

			int count = q.Count();
			if (count > 0)
			{
				chkAllow.Text += " (" + count + ")";
				rptReferencing.DataSource = q.MaxResults(10).Filters(N2.Content.Is.Distinct()).Select();
				rptReferencing.DataBind();
				hlReferencingItems.Visible = (count > 10);
			}
			else
				referencingItems.Visible = false;

			this.hlReferencingItems.NavigateUrl = "Dependencies.aspx?" + SelectionUtility.SelectedQueryKey + "=" + Selection.SelectedItem.Path + "&returnUrl=" + Server.HtmlEncode(Request.RawUrl);

            base.OnInit(e);
        }

		protected void chkAllow_OnCheckedChanged(object sender, EventArgs e)
		{
			btnDelete.Enabled = chkAllow.Checked;
		}

        protected override void OnLoad(EventArgs e)
        {
            if (Engine.UrlParser.IsRootOrStartPage(Selection.SelectedItem))
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
            this.Title = string.Format(GetLocalResourceString("DeletePage.TitleFormat", "Delete \"{0}\""),
                Selection.SelectedItem.Title);

            base.OnLoad(e);
        }

		private void RegisterConfirmAlert()
		{
            string message = string.Format(GetLocalResourceString("confirm.message", "Delete {0} ({1})?"), Selection.SelectedItem.Title, Selection.SelectedItem.Url);
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

				ppPermitted.Visible = false;

				Engine.AddActivity(new ManagementActivity { Operation = "Delete", PerformedBy = User.Identity.Name, Path = Selection.SelectedItem.Path, ID = Selection.SelectedItem.ID });
            }
            catch (Exception ex)
            {
                Engine.Resolve<IErrorNotifier>().Notify(ex);
                cvException.IsValid = false;
                cvException.Text = ex.Message;
            }
        }
    }
}
