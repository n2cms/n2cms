using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using N2.Edit.Workflow;
using N2.Security;
using N2.Web;

namespace N2.Edit.Versions
{
	[ToolbarPlugin("VERS", "versions", "{ManagementUrl}/Content/Versions/Default.aspx?{Selection.SelectedQueryKey}={selected}", ToolbarArea.Options, Targets.Preview, "{ManagementUrl}/Resources/icons/book_previous.png", 90, 
        ToolTip = "versions", 
        GlobalResourceClassName = "Toolbar",
		RequiredPermission = Permission.Publish)]
	[ControlPanelPendingVersion("There is a newer unpublished version of this item.", 200)]
	public partial class Default : Web.EditPage
	{
		ContentItem publishedItem;

		Persistence.IPersister persister;
		Persistence.IVersionManager versioner;

		protected override void OnInit(EventArgs e)
		{
            Page.Title = string.Format("{0}: {1}", GetLocalResourceString("VersionsPage.Title", "Versions"), Selection.SelectedItem.Title);

			persister = Engine.Persister;
			versioner = Engine.Resolve<Persistence.IVersionManager>();

			bool isVersionable = versioner.IsVersionable(Selection.SelectedItem);
            cvVersionable.IsValid = isVersionable;

			publishedItem = Selection.SelectedItem.VersionOf.Value ?? Selection.SelectedItem;

			base.OnInit(e);
		}

		protected void gvHistory_RowCommand(object sender, GridViewCommandEventArgs e)
		{
            ContentItem currentVersion = Selection.SelectedItem;
			int id = Convert.ToInt32(e.CommandArgument);
			if (e.CommandName == "Publish")
			{
				if (currentVersion.ID == id)
				{
					currentVersion.SavedBy = User.Identity.Name;
					if (!currentVersion.Published.HasValue || currentVersion.Published.Value > Utility.CurrentTime())
						currentVersion.Published = DateTime.Now;
					Engine.Resolve<StateChanger>().ChangeTo(currentVersion, ContentState.Published);
					persister.Save(currentVersion);
					Refresh(currentVersion, ToolbarArea.Both);

				}
				else
				{
					N2.ContentItem versionToRestore = Engine.Persister.Get(id);
					bool storeCurrent = versionToRestore.State == ContentState.Unpublished;
					ContentItem unpublishedVersion = versioner.ReplaceVersion(currentVersion, versionToRestore, storeCurrent);

					currentVersion.SavedBy = User.Identity.Name;
					
					if (!currentVersion.Published.HasValue || currentVersion.Published.Value > Utility.CurrentTime())
						currentVersion.Published = DateTime.Now;
					if (storeCurrent)
						currentVersion.VersionIndex = versioner.GetVersionsOf(currentVersion).Max(v => v.VersionIndex) + 1;
					Engine.Resolve<StateChanger>().ChangeTo(currentVersion, ContentState.Published);
					persister.Save(currentVersion);
					Refresh(currentVersion, ToolbarArea.Both);
				}
			}
			else if (currentVersion.ID != id && e.CommandName == "Delete")
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

			gvHistory.DataSource = versions.Select(v => new { v.ID, v.Title, v.State, v.IconUrl, v.Published, v.Expires, v.VersionIndex, v.SavedBy, Content = v });
			gvHistory.DataBind();
		}

		protected override string GetPreviewUrl(ContentItem item)
		{
			if (!item.VersionOf.HasValue)
				return item.Url;

			return Url.Parse(item.FindPath(PathData.DefaultAction).GetRewrittenUrl())
				.AppendQuery("preview", item.ID)
				.AppendQuery("original", item.VersionOf.ID);
		}

		protected bool IsVisible(object dataItem)
		{
			return Engine.SecurityManager.IsAuthorized(User, dataItem as ContentItem, Permission.Publish)
				&& !IsPublished(dataItem as ContentItem);
		}

		protected bool IsPublished(object dataItem)
		{
			var item = dataItem as ContentItem;
			return publishedItem.Equals(item) && item.Published.HasValue;
		}

		protected bool IsFuturePublished(object dataItem)
		{
			var item = dataItem as ContentItem;
			if (item["FuturePublishDate"] is DateTime)
				return true;
			if (!item.VersionOf.HasValue && item.Published.HasValue && item.Published > Utility.CurrentTime())
				return true;
			
			return false;
		}
	}
}
