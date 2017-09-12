using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit.FileSystem;
using N2.Web.UI;
using N2.Web.UI.WebControls;
using N2.Edit.Versioning;
using N2.Persistence;
using N2.Web;
using N2.Web.Parts;
using N2.Edit;

namespace N2.Details
{
    public class EditableMultiUploadButtonAttribute : AbstractEditableAttribute
    {
        private string targetType = null;
        private string targetProperty = null;
        private string targetZoneName = null;
        private string targetDomain = null;
        private string rootFolder = "/upload/";
        private string targetID = "";
        private bool useDefaultUploadDirectory = false;

        /// <summary>
        /// Fully Qualified Assembly name and type with name space found in typeof(Type).AssemblyQualifiedName eg. "AssemblyName,Namespace.Type"
        /// </summary>
        public string TargetType
        {
            get { return targetType; }
            set { targetType = value; }
        }

        /// <summary>
        /// Target Property to fill with images url from specified type
        /// </summary>
        public string TargetProperty
        {
            get { return targetProperty; }
            set { targetProperty = value; }
        }

        public string TargetZoneName
        {
            get { return targetZoneName; }
            set { targetZoneName = value; }
        }

        public string RootFolder
        {
            get { return rootFolder; }
            set { rootFolder = value; }
        }

        public string TargetDomain
        {
            get { return targetDomain; }
            set { targetDomain = value; }
        }

        public bool UseDefaultUploadDirectory
        {
            get { return useDefaultUploadDirectory; }
            set { useDefaultUploadDirectory = value; }
        }

        protected override Control AddEditor(Control container)
        {
            LinkButton btn = new LinkButton();
            btn.CausesValidation = false;
            btn.Text = Title;
            btn.Command += (s, a) => {
                ContentItem item = FindTopEditor((Control)s).CurrentItem;
                
                var parentEditor = ItemUtility.FindInParents<ItemEditor>(container);
                var parentVersion = parentEditor.GetAutosaveVersion()
                    ?? item;

                var path = EnsureDraft(parentVersion);

                UpdateItemFromTopEditor(path, container);

                if (path.CurrentPage.ID == 0 && path.CurrentPage.VersionOf.HasValue)
                {
                    var cvr = Engine.Resolve<ContentVersionRepository>();
                    cvr.Save(path.CurrentPage);
                }
                else
                {
                    Engine.Persister.SaveRecursive(path.CurrentPage);
                }

                var verIndex = path.CurrentPage.VersionIndex;
                var verKey = path.CurrentPage.GetVersionKey();

                targetID = item.ID > 0 ? item.ID.ToString() : item.VersionOf.ID.ToString();

                string defaultUploadDirectoryPath = RootFolder;
                if (UseDefaultUploadDirectory)
                {
                    var start = Find.ClosestOf<Definitions.IStartPage>(item);
                    Type itemType = item.GetContentType();

                    defaultUploadDirectoryPath = string.Format("{0}{1}/content/{2}", RootFolder.ToLower(), start.Title.ToLower().Trim().Replace(" ", "-"), itemType.Name.ToLower().Trim().Replace(" ", "-"));
                }

                var navigateUrl = string.Format("/N2/Files/FileSystem/Directory.aspx?selected={0}&TargetType={1}&TargetProperty={2}&TargetID={3}&TargetZone={4}&TargetDomain={5}&UseDefaultUploadDirectory={6}&VersionIndex={7}&VersionKey={8}", defaultUploadDirectoryPath, TargetType, TargetProperty, targetID, targetZoneName, targetDomain, UseDefaultUploadDirectory.ToString(), verIndex, verKey);
                HttpContext.Current.Response.Redirect(navigateUrl);
            };
            container.Controls.Add(btn);

            return btn;
        }
        
        protected override Label AddLabel(Control container)
        {
            return null;
        }

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            
        }

        public override bool UpdateItem(ContentItem item, Control editor)
        {
            return true;
        }


        private PathData EnsureDraft(ContentItem item)
        {
            var page = Find.ClosestPage(item);

            if (page.ID == 0)
                return new PathData(page, item);

            var cvr = Engine.Resolve<ContentVersionRepository>();
            var vm = Engine.Resolve<IVersionManager>();
            var path = PartsExtensions.EnsureDraft(vm, cvr, "", item.GetVersionKey(), item);

            return path;
        }

        private void UpdateItemFromTopEditor(PathData path, Control parent)
        {
            var editor = FindTopEditor(parent);
            var draftOfTopEditor = path.CurrentPage.FindPartVersion(editor.CurrentItem);
            editor.UpdateObject(new Edit.Workflow.CommandContext(editor.Definition, draftOfTopEditor, Interfaces.Editing, HttpContext.Current.User));
        }

        private ItemEditor FindTopEditor(Control parent)
        {
            var editor = ItemUtility.FindInParents<ItemEditor>(parent);
            if (editor == null)
                return null;
            return FindTopEditor(editor.Parent) ?? editor;
        }
    }
}
