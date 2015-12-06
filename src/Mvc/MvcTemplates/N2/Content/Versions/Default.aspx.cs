using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using N2.Edit.Workflow;
using N2.Security;
using N2.Web;
using N2.Persistence;
using N2.Edit.Versioning;
using N2.Resources;

namespace N2.Edit.Versions
{
    [ToolbarPlugin("VERS", "versions", "{ManagementUrl}/Content/Versions/Default.aspx?{Selection.SelectedQueryKey}={selected}", ToolbarArea.Preview, Targets.Preview, "{ManagementUrl}/Resources/icons/book_previous.png", 90,
        ToolTip = "versions",
        GlobalResourceClassName = "Toolbar",
        RequiredPermission = Permission.Publish,
        Legacy = true)]
    [ControlPanelPendingVersion("View draft", 200)]
    public partial class Default : Web.EditPage
    {
        ContentItem publishedItem;

        IPersister persister;
        IVersionManager versioner;

        protected override void OnInit(EventArgs e)
        {
            Page.Title = string.Format("{0}: {1}", GetLocalResourceString("VersionsPage.Title", "Versions"), Selection.SelectedItem.Title);

            persister = Engine.Persister;
            versioner = Engine.Resolve<IVersionManager>();

            bool isVersionable = versioner.IsVersionable(Selection.SelectedItem);
            cvVersionable.IsValid = isVersionable;

            publishedItem = Selection.SelectedItem.VersionOf.Value ?? Selection.SelectedItem;

			Page.JavaScript(Register.IconsCssPath);

            base.OnInit(e);
        }

        protected void gvHistory_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var stateChanger = Engine.Resolve<StateChanger>();

            ContentItem currentVersion = Selection.SelectedItem;
            int versionIndex = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName == "Publish")
            {
                if (currentVersion.VersionIndex == versionIndex)
                {
                    currentVersion.SavedBy = User.Identity.Name;
                    if (!currentVersion.Published.HasValue || currentVersion.Published.Value > Utility.CurrentTime())
                        currentVersion.Published = N2.Utility.CurrentTime();
                    stateChanger.ChangeTo(currentVersion, ContentState.Published);
                    persister.Save(currentVersion);
                    Refresh(currentVersion, ToolbarArea.Both);

                }
                else
                {
                    N2.ContentItem versionToRestore = versioner.GetVersion(currentVersion, versionIndex);
                    bool storeCurrent = versionToRestore.State == ContentState.Unpublished;
                    ContentItem unpublishedVersion = versioner.ReplaceVersion(currentVersion, versionToRestore, storeCurrent);

                    currentVersion.SavedBy = User.Identity.Name;

                    if (!currentVersion.Published.HasValue || currentVersion.Published.Value > Utility.CurrentTime())
                        currentVersion.Published = N2.Utility.CurrentTime();
                    stateChanger.ChangeTo(currentVersion, ContentState.Published);
                    persister.Save(currentVersion);
                    Refresh(currentVersion, ToolbarArea.Both);
                }
            }
            else if (currentVersion.VersionIndex != versionIndex && e.CommandName == "Delete")
            {
                ContentItem item = versioner.GetVersion(currentVersion, versionIndex);
                versioner.DeleteVersion(item);
            }
        }

        protected void gvHistory_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        //public class VersionInfo
        //{
        //    public int ID { get; set; }
        //    public string Title { get; set; }
        //    public ContentState State { get; set; }
        //    public string IconUrl { get; set; }
        //    public DateTime? Published { get; set; }
        //    public DateTime? Expires { get; set; }
        //    public int VersionIndex { get; set; }
        //    public string SavedBy { get; set; }
        //    public ContentItem Content { get; set; }
        //}

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var versions = versioner.GetVersionsOf(publishedItem)
                //.Select(v => new VersionInfo { ID = v.ID, Title = v.Title, State = v.State, IconUrl = v.IconUrl, Published = v.Published, Expires = v.Expires, VersionIndex = v.VersionIndex, SavedBy = v.SavedBy, Content = v })
                .ToList();

            gvHistory.DataSource = versions;
            gvHistory.DataBind();
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

		protected string GetStateIconClass(ContentState state)
		{
			switch (state)
			{
				case ContentState.Deleted:
					return "fa fa-trash-o red";
				case ContentState.Draft:
				case ContentState.New:
					return "fa fa-circle-o orange";
				case ContentState.Published:
					return "fa fa-play-circle green";
				case ContentState.Unpublished:
					return "fa fa-stop red";
				case ContentState.Waiting:
					return "fa fa-clock orange";
				case ContentState.None:
				default:
					return "";
			}
		}
    }
}
