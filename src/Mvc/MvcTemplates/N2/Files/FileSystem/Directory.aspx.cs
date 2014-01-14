using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using N2.Edit.FileSystem.Items;
using N2.Edit.Web;
using N2.Resources;
using N2.Web.Drawing;
using System.Configuration;
using System.Web.Configuration;
using N2.Web;

namespace N2.Edit.FileSystem
{
    public partial class Directory1 : EditPage
    {
        protected override void RegisterToolbarSelection()
        {
            string script = GetToolbarSelectScript("preview");
            Register.JavaScript(this, script, ScriptPosition.Bottom, ScriptOptions.ScriptTags);
        }

        protected IEnumerable<ContentItem> ancestors;

        IList<Directory> directories;
        IList<File> files;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Page.StyleSheet("{ManagementUrl}/Files/Css/Files.css");

            ancestors = Find.EnumerateParents(Selection.SelectedItem, null, true).Where(a => a is AbstractNode).Reverse();

            Reload();

            Refresh(Selection.SelectedItem, ToolbarArea.Navigation, force: false);
            btnDelete.Enabled = Engine.SecurityManager.IsAuthorized(User, Selection.SelectedItem, N2.Security.Permission.Publish);
            hlEdit.NavigateUrl = Engine.ManagementPaths.GetEditExistingItemUrl(Selection.SelectedItem);
        }

        private void Reload()
        {
            var dir = Selection.SelectedItem as Directory;
            directories = dir.GetDirectories();
            files = dir.GetFiles();
            
            rptDirectories.DataSource = directories;
            rptFiles.DataSource = files;
            DataBind();
        }

        public void OnDeleteCommand(object sender, CommandEventArgs args)
        {
            Delete(Request.Form["directory"], directories.Select(f => f.Url), Engine.Resolve<IFileSystem>().DeleteDirectory);
            Delete(Request.Form["file"], files.Select(f => f.Url), Engine.Resolve<IFileSystem>().DeleteFile);
        }

        private void Delete(string itemsToDelete, IEnumerable<string> allowed, Action<string> deleteAction)
        {
            if (string.IsNullOrEmpty(itemsToDelete))
                return;

            var items = itemsToDelete.Split(',');
            foreach (string item in items.Select(i => i.TrimEnd('/')).Intersect(allowed.Select(a => a.TrimEnd('/'))))
            {
                deleteAction(item);
            }

            Reload();
        }

        protected string ImageBackgroundStyle(string url)
        {
            if(ImagesUtility.IsImagePath(url))
                return string.Format("background-image:url({0})", N2.Edit.Web.UI.Controls.ResizedImage.GetResizedImageUrl(url, 100, 100, N2.Web.Drawing.ImageResizeMode.Fit));
            else
                return "";
        }
    }
}
