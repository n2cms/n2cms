using N2.Web.Drawing;
using N2.Web.UI.WebControls;
using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Details
{
    /// <summary>
    /// Allows to upload or select an media file using the new Media Browser.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableMediaAttribute : AbstractEditableAttribute, IRelativityTransformer, IWritingDisplayable, IDisplayable
    {
        private string alt = string.Empty;
        private string cssClass = string.Empty;
        private string uploadDirectory = string.Empty;
        private string extensions = string.Empty;

        public EditableMediaAttribute()
            : this(null, 41)
        {
        }

        public EditableMediaAttribute(string title, int sortOrder)
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

        /// <summary>Set a specific upload directory.</summary>
        public string UploadDirectory
        {
            get { return uploadDirectory; }
            set { uploadDirectory = value; }
        }

        /// <summary>CSS class on the image element.</summary>
        public string UploadText { get; set; }

        /// <summary>The image size to display by default if available.</summary>
        public string PreferredSize { get; set; }

        /// <summary>A comma separated list of extensions: ie '.jpg,.png'; if blank image extensions are used.</summary>
        public string Extensions
        {
            get { return string.IsNullOrEmpty(extensions) ? MediaSelector.ImageExtensions : extensions; }
            set { extensions = value; }
        }


        public override bool UpdateItem(ContentItem item, Control editor)
        {
            SelectingMediaControl composite = (SelectingMediaControl)editor;
            
            if (composite.SelectorControl.Url != item[Name] as string)
            {
                item[Name] = composite.SelectorControl.Url;
                return true;
            }

            return false;
        }

        public bool ReadOnly { get; set; }

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            var composite = (SelectingMediaControl)editor;
            composite.Select(item[Name] as string);
        }


        protected override Control AddEditor(Control container)
        {
            var composite = new SelectingMediaControl();
            composite.ID = Name;
            composite.SelectorControl.Placeholder(GetLocalizedText("Placeholder") ?? Placeholder);
            composite.SelectorControl.SelectableExtensions = Extensions;
            composite.SelectorControl.PreferredSize = PreferredSize;

            if (ReadOnly)
                composite.SelectorControl.Attributes["readonly"] = "readonly";

            if(!string.IsNullOrEmpty(PreferredSize))
                composite.SelectorControl.Attributes["preferredSize"] = PreferredSize;

            container.Controls.Add(composite);
            return composite;
        }

        /// <summary>Adds a required field validator.</summary>
        /// <param name="container">The container control for this validator.</param>
        /// <param name="editor">The editor control to validate.</param>
        protected override Control AddRequiredFieldValidator(Control container, Control editor)
        {
            var composite = (SelectingUploadCompositeControl)editor;
            if (composite == null) return null;
            var rfv = new RequireEitherFieldValidator();
            rfv.ID = Name + "_rfv";
            rfv.ControlToValidate = composite.SelectorControl.ID;
            rfv.OtherControlToValidate = composite.UploadControl.ID;
            rfv.Display = ValidatorDisplay.Dynamic;
            rfv.Text = GetLocalizedText("RequiredText") ?? RequiredText;
            rfv.ErrorMessage = GetLocalizedText("RequiredMessage") ?? RequiredMessage;
            editor.Controls.Add(rfv);

            return rfv;
        }


        #region IDisplayable Members

        public override Control AddTo(ContentItem item, string detailName, Control container)
        {
            using (var sw = new StringWriter())
            {
                Write(item, detailName, sw);

                LiteralControl lc = new LiteralControl(sw.ToString());
                container.Controls.Add(lc);
                return lc;
            }
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

        public virtual void Write(ContentItem item, string propertyName, TextWriter writer)
        {
            string url = item[propertyName] as string;
            if (string.IsNullOrEmpty(url))
                return;

            string extension = VirtualPathUtility.GetExtension(url);
            switch (ImagesUtility.GetExtensionGroup(extension))
            {
                case ImagesUtility.ExtensionGroups.Images:
                    var sizes = DisplayableImageAttribute.GetSizes(PreferredSize);
                    DisplayableImageAttribute.WriteImage(item, propertyName, sizes, alt, CssClass, writer);
                    return;
                default:
                    WriteUrl(item, propertyName, cssClass, writer, url);
                    return;
            }
        }

        private static void WriteUrl(ContentItem item, string propertyName, string cssClass, TextWriter writer, string url)
        {
            cssClass = item[propertyName + "_CssClass"] as string ?? cssClass;
            if (string.IsNullOrEmpty(cssClass))
                writer.Write(string.Format("<a href=\"{0}\">{1}</a>", url, VirtualPathUtility.GetFileName(url)));
            else
                writer.Write(string.Format("<a href=\"{0}\" class=\"{1}\">{2}</a>", url, cssClass, VirtualPathUtility.GetFileName(url)));
        }

        #endregion

        public const string ValidCharactersExpression = "^[^+]*$";
        public const string InvalidCharactersExpression = "[+]";
    }
}

