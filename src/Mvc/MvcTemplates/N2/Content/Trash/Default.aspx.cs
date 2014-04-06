using System;
using System.Configuration;
using System.Web.UI.WebControls;
using N2.Web.UI;
using N2.Management.Content.Trash;
using N2.Web;

namespace N2.Edit.Trash
{
    public partial class Default : N2.Edit.Web.EditPage, IContentTemplate, IItemContainer
    {
        protected ITrashHandler Trash
        {
            get { return Engine.Resolve<ITrashHandler>(); }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            N2.Resources.Register.JQueryPlugins(Page);

            this.hlCancel.NavigateUrl = Engine.UrlParser.StartPage.Url;
            this.cvRestore.IsValid = true;

            var status = Engine.Resolve<AsyncTrashPurger>().Status;

            if (status.IsRunning)
            {
                btnClear.Enabled = false;
                hlRunning.NavigateUrl = Request.RawUrl;
                hlRunning.Visible = status.IsRunning;
                var format = status.Progress.Deleted > 0
                    ? GetLocalResourceString("hlRunning", "A delete task is in progress. Deleted {0} out of {1} items below '{2}'. Click to refresh.")
                    : GetLocalResourceString("hlDeleting", "Deleting {1} items below '{2}'. Click to refresh.");
                hlRunning.Text = string.Format(format, status.Progress.Deleted, status.Progress.Total, status.Title);
            }
            else if (Convert.ToBoolean(Request["showStatus"]))
                RegisterRefreshNavigationScript(CurrentItem);
            else
                this.btnClear.Enabled = CurrentItem.Children.Count > 0;
        }

        protected void gvTrash_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int itemIndex = Convert.ToInt32(e.CommandArgument);
            int itemID = Convert.ToInt32(gvTrash.DataKeys[itemIndex].Value);
            ContentItem item = Engine.Persister.Get(itemID);

            if (e.CommandName == "Restore")
            {
                try
                {
                    Trash.Restore(item);
                    this.gvTrash.DataBind();
                }
                catch (N2.Integrity.NameOccupiedException)
                {
                    cvRestore.IsValid = false;
                }
                RegisterRefreshNavigationScript(item);
            }
            else if (e.CommandName == "Purge")
            {
                if (Trash.TrashContainer != null && Trash.TrashContainer.AsyncTrashPurging)
                {
                    Engine.Resolve<AsyncTrashPurger>().BeginPurge(item.ID);
                    Response.Redirect(Request.RawUrl.ToUrl().SetQueryParameter("showStatus", "true"));
                }
                else
                    Engine.Persister.Delete(item);

            }
            else
            {
                RegisterRefreshNavigationScript(CurrentItem);
            }
        }

	    protected string HtmlEncode(String value)
	    {
		    return Sanitizer.Encode(value);
	    }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            if (Trash.TrashContainer != null && Trash.TrashContainer.AsyncTrashPurging)
            {
                Engine.Resolve<AsyncTrashPurger>().BeginPurgeAll();
                Response.Redirect(Request.RawUrl.ToUrl().SetQueryParameter("showStatus", "true"));
            }
            else
            {
                Trash.PurgeAll();
                this.DataBind();
                RegisterRefreshNavigationScript(this.CurrentItem);
            }
        }

        #region RegisterRefreshNavigationScript
        private const string refreshScriptFormat = @"n2ctx.refresh({{ id: {0}, path: '{1}', navigationUrl: '{2}', permission: '{3}'}});";

        protected virtual void RegisterRefreshNavigationScript(ContentItem item)
        {
            string script = string.Format(refreshScriptFormat,
                item.ID, // 0
                item.Path, // 1
                GetNavigationUrl(item), // 2
                Engine.GetContentAdapter<NodeAdapter>(item).GetMaximumPermission(item) // 3
                );

            ClientScript.RegisterClientScriptBlock(
                typeof(Default),
                "AddRefreshEditScript",
                script, true);
        }
        #endregion


        #region IContentTemplate Members

        public ContentItem currentItem;
        public ContentItem CurrentItem 
        {
            get { return currentItem ?? (currentItem = Engine.UrlParser.Parse(Request.RawUrl)); }
            set { currentItem = value; }
        }

        #endregion
    }
}
