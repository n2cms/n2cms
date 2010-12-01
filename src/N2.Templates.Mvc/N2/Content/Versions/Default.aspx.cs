using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using N2.Web;
using System.Web.Security;
using N2.Security;

namespace N2.Edit.Versions
{
    [ToolbarPlugin("", "versions", "{ManagementUrl}/Content/Versions/Default.aspx?selected={selected}", ToolbarArea.Preview, Targets.Preview, "{ManagementUrl}/Resources/icons/book_previous.png", 90, 
        ToolTip = "versions", 
        GlobalResourceClassName = "Toolbar")]
	[ControlPanelPendingVersion("There is a newer unpublished version of this item.", 200)]
	public partial class Default : Web.EditPage
	{
		ContentItem publishedItem;

		Persistence.IPersister persister;
		Persistence.IVersionManager versioner;

		protected override void OnInit(EventArgs e)
		{
            Page.Title = string.Format("{0}: {1}", GetLocalResourceString("VersionsPage.Title"), Selection.SelectedItem.Title);

			persister = Engine.Persister;
			versioner = Engine.Resolve<Persistence.IVersionManager>();

			bool isVersionable = versioner.IsVersionable(SelectedItem);
            cvVersionable.IsValid = isVersionable;

            publishedItem = Selection.SelectedItem.VersionOf ?? SelectedItem;

			base.OnInit(e);
		}

		protected void gvHistory_RowCommand(object sender, GridViewCommandEventArgs e)
		{
            ContentItem currentVersion = Selection.SelectedItem;
			int id = Convert.ToInt32(e.CommandArgument);
			if (currentVersion.ID == id)
			{
				// do nothing
			}
			else if (e.CommandName == "Publish")
			{
				N2.ContentItem versionToRestored = Engine.Persister.Get(id);
				ContentItem unpublishedVersion = versioner.ReplaceVersion(currentVersion, versionToRestored, true);

				currentVersion.SavedBy = User.Identity.Name;
				currentVersion.VersionIndex = unpublishedVersion.VersionIndex + 1;
				persister.Save(currentVersion);

				Refresh(currentVersion, ToolbarArea.Both);
				DataBind();
			}
			else if (e.CommandName == "Delete")
			{
				ContentItem item = Engine.Persister.Get(id);
				persister.Delete(item);
			}
		}

		protected void gvHistory_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{

		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			IList<ContentItem> versions = versioner.GetVersionsOf(publishedItem);

			gvHistory.DataSource = versions;
			gvHistory.DataBind();
		}

		protected override string GetPreviewUrl(ContentItem item)
		{
			if (item.VersionOf == null)
				return item.Url;

			return Url.Parse(item.FindPath(PathData.DefaultAction).RewrittenUrl)
				.AppendQuery("preview", item.ID)
				.AppendQuery("original", item.VersionOf.ID);
		}

		protected bool IsVisible(object dataItem)
		{
			return Engine.SecurityManager.IsAuthorized(User, dataItem as ContentItem, Permission.Publish)
				&& !IsPublished(dataItem);
		}

		protected bool IsPublished(object dataItem)
		{
			return publishedItem == dataItem;
		}
	}
}
