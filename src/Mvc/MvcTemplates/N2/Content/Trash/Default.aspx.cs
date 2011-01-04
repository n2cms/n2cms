using System;
using System.Web;
using System.Web.UI.WebControls;
using N2.Web;
using N2.Web.UI;

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
			else
			{
				RegisterRefreshNavigationScript(CurrentItem);
			}
		}

		protected void btnClear_Click(object sender, EventArgs e)
		{
			foreach (ContentItem child in this.CurrentItem.GetChildren())
			{
				Engine.Persister.Delete(child);
			}
			this.DataBind();
			RegisterRefreshNavigationScript(this.CurrentItem);
		}

		#region RegisterRefreshNavigationScript
		private const string refreshScriptFormat = @"n2ctx.refresh({{ navigationUrl: '{1}', path: '{4}'}});";

		protected virtual void RegisterRefreshNavigationScript(ContentItem item)
		{
			string script = string.Format(refreshScriptFormat,
				VirtualPathUtility.ToAbsolute(Engine.ManagementPaths.ResolveResourceUrl("{ManagementUrl}/Content/Default.aspx")), // 0
				GetNavigationUrl(item), // 1
				item.ID, // 2
				item.FindPath(PathData.DefaultAction).RewrittenUrl, // 3
				item.Path // 4
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
