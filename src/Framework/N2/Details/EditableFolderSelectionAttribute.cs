using System;
using System.Web.UI;
using N2.Web.UI.WebControls;
using N2.Definitions;
using N2.Edit.FileSystem;

namespace N2.Details
{
    /// <summary>
    /// Defines an editable image this allows to select an image with the file picker.
    /// </summary>
    /// <example>
    ///     public class MyItem : N2.ContentItem
    ///     {
    ///         [N2.Details.EditableFolderSelection]
    ///         public virtual string FolderUrl { get; set; }
    ///     }
    /// </example>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableFolderSelectionAttribute : AbstractEditableAttribute, IDisplayable, IRelativityTransformer, IWritingDisplayable
    {
        private string alt = string.Empty;
        private string cssClass = string.Empty;



        public EditableFolderSelectionAttribute()
            : this(null, 40)
        {
        }

        public EditableFolderSelectionAttribute(string title, int sortOrder)
            : base(title, sortOrder)
        {
        }



        /// <summary>Image alt text.</summary>
        public string Alt
        {
            get { return alt; }
            set { alt = value; }
        }

        /// <summary>CSS class on the image element.</summary>
        public string CssClass
        {
            get { return cssClass; }
            set { cssClass = value; }
        }

        /// <summary>The image size to display by default if available.</summary>
        public string PreferredSize { get; set; }



        protected override Control AddEditor(Control container)
        {
            FileSelector selector = new FileSelector();
            selector.SelectableTypes = typeof(IFileSystemDirectory).Name;
            selector.ID = Name;
            selector.Placeholder(GetLocalizedText("Placeholder") ?? Placeholder);
            container.Controls.Add(selector);
            return selector;
        }
        
        public override void UpdateEditor(ContentItem item, Control editor)
        {
            FileSelector selector = (FileSelector)editor;
            selector.Url = item[Name] as string;
        }

        public override bool UpdateItem(ContentItem item, Control editor)
        {
            FileSelector selector = (FileSelector)editor;
            if (selector.Url != item[Name] as string)
            {
                item[Name] = selector.Url;
                return true;
            }
            return false;
        }

        #region IDisplayable Members
        public override Control AddTo(ContentItem item, string detailName, Control container)
        {
            return DisplayableImageAttribute.AddImage(container, item, detailName, PreferredSize, CssClass, Alt);
        }
        #endregion

        #region IRelativityTransformer Members

        public RelativityMode RelativeWhen { get; set; }

        string IRelativityTransformer.Rebase(string currentPath, string fromAppPath, string toAppPath)
        {
            return N2.Web.Url.Rebase(currentPath, fromAppPath, toAppPath);
        }

        #endregion

        #region IWritingDisplayable Members

        public void Write(ContentItem item, string propertyName, System.IO.TextWriter writer)
        {
            var folder = item[propertyName] as string;
            if (string.IsNullOrEmpty(folder))
                return;

            foreach (var potentialImage in Engine.Resolve<IFileSystem>().GetFiles(folder))
            {
                if (!N2.Web.Drawing.ImagesUtility.IsImagePath(potentialImage.VirtualPath))
                    continue;

                if (string.IsNullOrEmpty(PreferredSize))
                    DisplayableImageAttribute.WriteImage(potentialImage.VirtualPath, writer);
                else
                {
                    var possibleSizes = new[] { PreferredSize };
                    if (N2.Web.Drawing.ImagesUtility.GetSize(potentialImage.VirtualPath, possibleSizes) == PreferredSize)
                        DisplayableImageAttribute.WriteImage(potentialImage.VirtualPath, writer, possibleSizes);
                }
            }
        }

        #endregion
    }
}
