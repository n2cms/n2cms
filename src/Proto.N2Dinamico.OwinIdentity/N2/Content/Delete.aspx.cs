using System;
using N2.Persistence.Finder;
using N2.Security;
using N2.Web;
using N2.Web.UI.WebControls;
using N2.Edit.Activity;
using N2.Management.Activity;
using N2.Persistence;
using N2.Edit.Versioning;
using System.Linq;

namespace N2.Edit
{
	[NavigationLinkPlugin("Delete", "delete", "{ManagementUrl}/Content/delete.aspx?{Selection.SelectedQueryKey}={selected}&alert=true", Targets.Preview, "{ManagementUrl}/Resources/icons/cross.png", 30, 
		GlobalResourceClassName = "Navigation",
		RequiredPermission = Permission.Publish,
		IconClass = "n2-icon-trash",
		Legacy = true)]
	[ToolbarPlugin("", "delete_tool", "{ManagementUrl}/Content/delete.aspx?{Selection.SelectedQueryKey}={selected}", ToolbarArea.Operations, Targets.Preview, "{ManagementUrl}/Resources/icons/cross.png", 60, 
		ToolTip = "delete",
		GlobalResourceClassName = "Toolbar",
		RequiredPermission = Permission.Publish,
		Legacy = true)]
	[ControlPanelDiscardPreviewOrDelete]
	public partial class Delete : Web.EditPage
    {
		protected ContentItem selectedItem;

        protected override void OnInit(EventArgs e)
        {
			selectedItem = Selection.ParseSelectionFromRequest();

			if (selectedItem != null)
			{
				itemsToDelete.CurrentItem = selectedItem;
				itemsToDelete.DataBind();

				ParameterCollection query = Parameter.Equal("State", ContentState.Deleted);
				if (selectedItem.ID != 0)
					query &= (Parameter.Like(LinkTracker.Tracker.LinkDetailName, selectedItem.Url).Detail() | Parameter.Equal(null, selectedItem).Detail());
				else
					query &= Parameter.Like(LinkTracker.Tracker.LinkDetailName, selectedItem.Url).Detail();
				var count = Engine.Persister.Repository.Count(query);

				//var q = Engine.Resolve<IItemFinder>().Where.State.NotEq(ContentState.Deleted);

				//q = q.And.OpenBracket()
				//	.Detail(LinkTracker.Tracker.LinkDetailName).Like(selectedItem.Url);
				//if (selectedItem.ID != 0)
				//	q = q.Or.Detail().Eq(selectedItem);
				//q = q.CloseBracket();

				//int count = q.Count();
				if (count > 0)
				{
					chkAllow.Text += " (" + count + ")";
					rptReferencing.DataSource = Engine.Persister.Repository.Find(query.Take(10)).Where(Content.Is.Distinct());
						//q.MaxResults(10).Filters(N2.Content.Is.Distinct()).Select();
					rptReferencing.DataBind();
					hlReferencingItems.Visible = (count > 10);
				}
				else
					referencingItems.Visible = false;

				this.hlReferencingItems.NavigateUrl = "Dependencies.aspx?" + SelectionUtility.SelectedQueryKey + "=" + selectedItem.Path + "&returnUrl=" + Server.HtmlEncode(Request.RawUrl);
			}

            base.OnInit(e);
        }

		protected void chkAllow_OnCheckedChanged(object sender, EventArgs e)
		{
			btnDelete.Enabled = chkAllow.Checked;
		}

        protected override void OnLoad(EventArgs e)
        {
			if (selectedItem == null)
			{
				this.Title = GetLocalResourceString("DeletePage.NotFound", "Delete \"Not Found\"");

				cvRemoved.IsValid = false;
				this.btnDelete.Enabled = false;
				hlCancel.Focus();

				referencingItems.Visible = false;
				affectedItems.Visible = false;
			}
            else if (Engine.UrlParser.IsRootOrStartPage(selectedItem))
            {
                cvDelete.IsValid = false;
                this.btnDelete.Enabled = false;
                hlCancel.Focus();

				this.Title = string.Format(GetLocalResourceString("DeletePage.TitleFormat", "Delete \"{0}\""), selectedItem.Title);

				referencingItems.Visible = false;
				affectedItems.Visible = false;
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

				this.Title = string.Format(GetLocalResourceString("DeletePage.TitleFormat", "Delete \"{0}\""), selectedItem.Title);

                btnDelete.Focus();
            }

            base.OnLoad(e);
        }

		private void RegisterConfirmAlert()
		{
            string message = string.Format(GetLocalResourceString("confirm.message", "Delete {0} ({1})?"), selectedItem.Title, selectedItem.Url);
			ClientScript.RegisterClientScriptBlock(typeof(Delete), "confirm",
                string.Format(@"jQuery(document).ready( function() {{
	if(confirm('{0}')){{
		{1};
	}}else{{
		window.location='{2}';
	}}
}});", message, ClientScript.GetPostBackClientHyperlink(btnDelete, string.Empty), selectedItem.Url), true);
		}

        protected void OnDeleteClick(object sender, EventArgs e)
        {
			var item = selectedItem;
            var parent = item.Parent;
			try
            {
				if (!item.IsPage)
				{
					// it's a published part, create a version of it's page and remove the part from it.
					var page = Find.ClosestPage(item);
					if (page != null)
					{
						var versions = Engine.Resolve<IVersionManager>();

						if (!page.VersionOf.HasValue)
						{
							page = versions.AddVersion(page, asPreviousVersion: false);
						}

						var partVersion = page.FindPartVersion(item);
						partVersion.AddTo(null);
						
						versions.UpdateVersion(page);
						parent = page;
					}
				}
				else
				{
					Engine.Persister.Delete(selectedItem);
				}

                if (parent != null)
                    Refresh(parent, ToolbarArea.Both);
                else
                    Refresh(N2.Context.UrlParser.StartPage, ToolbarArea.Both);

				ppPermitted.Visible = false;

				Engine.AddActivity(new ManagementActivity { Operation = "Delete", PerformedBy = User.Identity.Name, Path = selectedItem.Path, ID = selectedItem.ID });
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
