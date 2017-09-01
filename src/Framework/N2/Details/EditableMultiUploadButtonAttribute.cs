using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit.FileSystem;

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
            HyperLink btn = new HyperLink();
            btn.Text = Title;
            container.Controls.Add(btn);
            return btn;
        }

        protected override Label AddLabel(Control container)
        {
            return null;
        }

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            HyperLink btn = editor as HyperLink;
            if (btn != null)
            {
                targetID = item.ID > 0 ? item.ID.ToString() : item.VersionOf.ID.ToString();

                string defaultUploadDirectoryPath = RootFolder;
                if (UseDefaultUploadDirectory)
                {
                    var start = Find.ClosestOf<Definitions.IStartPage>(item);
                    Type itemType = item.GetContentType();

                    defaultUploadDirectoryPath = string.Format("{0}{1}/content/{2}", RootFolder.ToLower(), start.Title.ToLower().Trim().Replace(" ", "-"), itemType.Name.ToLower().Trim().Replace(" ", "-"));
                } 

                btn.NavigateUrl = string.Format("/N2/Files/FileSystem/Directory.aspx?selected={0}&TargetType={1}&TargetProperty={2}&TargetID={3}&TargetZone={4}&TargetDomain={5}&useDefaultUploadDirectory={6}", defaultUploadDirectoryPath, TargetType, TargetProperty, targetID, targetZoneName, targetDomain, UseDefaultUploadDirectory.ToString());
            }
        }

        public override bool UpdateItem(ContentItem item, Control editor)
        {
            return true;
        }
    }
}
