using System;
using System.Web;
using System.Web.UI.WebControls;
using N2.Web;

namespace N2.Edit.Trash
{
	public partial class Default : N2.Web.UI.ContentPage<TrashContainerItem>
	{
		protected ITrashHandler Trash
		{
			get { return N2.Context.Current.Resolve<ITrashHandler>(); }
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.hlCancel.NavigateUrl = N2.Context.UrlParser.StartPage.Url;
			this.cvRestore.IsValid = true;
			this.btnClear.Enabled = CurrentItem.Children.Count > 0;
		}

		protected void gvTrash_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			int itemIndex = Convert.ToInt32(e.CommandArgument);
			int itemID = Convert.ToInt32(gvTrash.DataKeys[itemIndex].Value);
			ContentItem item = N2.Context.Persister.Get(itemID);

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
			else
			{
				RegisterRefreshNavigationScript(CurrentItem);
			}
		}

		protected void btnClear_Click(object sender, EventArgs e)
		{
			foreach (ContentItem child in this.CurrentItem.GetChildren())
			{
				N2.Context.Persister.Delete(child);
			}
			this.DataBind();
			RegisterRefreshNavigationScript(this.CurrentItem);
		}

		#region RegisterRefreshNavigationScript
		private const string refreshScriptFormat = @"if(window.n2ctx){{window.n2ctx.setupToolbar('{3}');window.n2ctx.refreshNavigation('{1}');}}";

		protected string GetNavigationUrl(ContentItem selectedItem)
		{
			return N2.Context.Current.EditManager.GetNavigationUrl(selectedItem);
		}

		protected virtual void RegisterRefreshNavigationScript(ContentItem item)
		{
			string script = string.Format(refreshScriptFormat,
				VirtualPathUtility.ToAbsolute("~/N2/Content/Default.aspx"), // 0
				GetNavigationUrl(item), // 1
				item.ID, // 2
				item.FindPath(PathData.DefaultAction).RewrittenUrl // 3
				);

			ClientScript.RegisterClientScriptBlock(
				typeof(Default),
				"AddRefreshEditScript",
				script, true);
		}
		#endregion

	}
}
