using N2.Edit;
using N2.Edit.Versioning;
using N2.Persistence;
using N2.Resources;
using N2.Web;
using N2.Web.Parts;
using N2.Web.UI;
using N2.Web.UI.WebControls;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
            ((Page)HttpContext.Current.CurrentHandler).JavaScript("{ManagementUrl}/Resources/Js/LoadingModal.js?v="+Register.ScriptVersion);

            LinkButton btn = new LinkButton();
            btn.CausesValidation = false;
            btn.Text = Title;
            btn.OnClientClick = "n2LoadingModal.openModal()";
            btn.CssClass = "btnLoadingModal";
            btn.Command += (s, a) => {
                ContentItem item = FindTopEditor(container).CurrentItem;
                
                var parentEditor = ItemUtility.FindInParents<ItemEditor>(container);
                var autoSaveVersion = parentEditor.GetAutosaveVersion();
                if (autoSaveVersion != null)
                {
                    //Posted back item has all the latest updates. Discard the auto-saved version if exists.
                    Engine.Resolve<IVersionManager>().DeleteVersion(Find.ClosestPage(autoSaveVersion));
                }

                var path = EnsureDraft(item);

                UpdateItemFromTopEditor(path, container);
                
                if (path.CurrentPage.VersionOf.HasValue)
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

                    var slug = N2.Context.Current.Resolve<Slug>();
                    defaultUploadDirectoryPath = string.Format("{0}{1}/content/{2}", RootFolder.ToLower(), slug.Create(start.Title), slug.Create(itemType.Name));
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
            
            //New/Draft master version or a version of an item
			if (page.ID == 0 || (!page.VersionOf.HasValue && (page.State == ContentState.New || page.State == ContentState.Draft)))
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
