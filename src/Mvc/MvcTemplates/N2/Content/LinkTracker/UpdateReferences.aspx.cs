using System;
using System.Linq;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Persistence;
using N2.Linq;
using N2.Management.Content.LinkTracker;
using N2.Web;
using N2.Edit.FileSystem.Items;
using N2.Web.Drawing;
using N2.Management.Files.FileSystem.Pages;

namespace N2.Edit.LinkTracker
{
    public partial class UpdateReferences : N2.Edit.Web.EditPage
    {
        private Tracker tracker;
        private string previousName;
        private string previousUrl;
        private ContentItem previousParent;

        protected override void OnInit(EventArgs e)
        {
            tracker = Engine.Resolve<Tracker>();

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Refresh(Selection.SelectedItem, ToolbarArea.Navigation);

            Title = "Update links leading to " + Selection.SelectedItem.Title;

            var item = Selection.SelectedItem;

            previousParent = Engine.Resolve<Navigator>().Navigate(Request["previousParent"]);
            previousName = Request["previousName"];
            previousUrl = Request["previousUrl"];

            if (item is IFileSystemNode && previousParent != null)
            {
                previousUrl = Url.Combine(previousParent.Url, previousName);
            }

            if (!IsPostBack)
            {
                var referrers = item.ID == 0
                    ? tracker.FindReferrers(previousUrl).ToList()
                    : tracker.FindReferrers(item).ToList();
                bool showReferences = referrers.Count > 0;
                if (showReferences)
                {
                    rptReferencingItems.DataSource = referrers;
                    DataBind();
                }
                else
                    fsReferences.Visible = false;

                bool showChildren = item.Children.Count > 0;
                if (showChildren)
                {
                    targetsToUpdate.CurrentItem = item;
                    targetsToUpdate.DataBind();
                }
                else
                    fsChildren.Visible = false;

                chkPermanentRedirect.Visible = previousParent != null 
                    && item.ID != 0
                    && Engine.Resolve<Configuration.EditSection>().LinkTracker.PermanentRedirectEnabled;

                if (!showReferences && !showChildren && previousParent == null)
                {
                    Refresh(item, ToolbarArea.Both);
                }
            }
        }

        protected void OnUpdateCommand(object sender, CommandEventArgs args)
        {
            if (chkPermanentRedirect.Checked && previousParent != null)
            {
                
                var redirect = Engine.Resolve<ContentActivator>().CreateInstance<PermanentRedirect>(previousParent);
                redirect.Title = previousName + GetLocalResourceString("PermanentRedirect", " (permanent redirect)");
                redirect.Name = previousName;
                redirect.RedirectUrl = Selection.SelectedItem.Url;
                redirect.RedirectTo = Selection.SelectedItem;
                redirect.AddTo(previousParent);
                
                Engine.Persister.Save(redirect);
            }

            tracker.UpdateReferencesTo(Selection.SelectedItem, previousUrl, isRenamingDirectory: Selection.SelectedItem is IFileSystemDirectory);

            if (Selection.SelectedItem is IFileSystemFile)
            {
                var sizes = this.Engine.Resolve<ImageSizeCache>().ImageSizes;
                foreach (var sizedImage in Selection.SelectedItem.Children)
                {
                    var size = ImagesUtility.GetSize(sizedImage.Url, sizes);
                    var previousSizeUrl = ImagesUtility.GetResizedPath(previousUrl, size);

                    tracker.UpdateReferencesTo(sizedImage, previousSizeUrl, isRenamingDirectory: false);
                }
            }

            if (chkChildren.Checked)
            {
                mvPhase.ActiveViewIndex = 1;
                //rptDescendants.DataSource = Content.Search.Find.Where.AncestralTrail.Like(Selection.SelectedItem.GetTrail() + "%").Select()
                //  .Where(Content.Is.Accessible());
                rptDescendants.DataSource = Content.Search.Repository.Find(N2.Persistence.Parameter.Below(Selection.SelectedItem)).Where(Content.Is.Accessible());
                rptDescendants.DataBind();
            }
            else
            {
                Refresh(Selection.SelectedItem, ToolbarArea.Both);
            }
        }
    }
}
