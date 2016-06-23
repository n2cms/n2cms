using N2.Edit.FileSystem;
using N2.Web;
using N2.Web.Drawing;
using N2.Web.UI.WebControls;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Details
{
	/// <summary>
	/// Allows to upload or select a file to use.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class EditableFileUploadAttribute : EditableMediaAttribute
	{
		public EditableFileUploadAttribute()
			: this(null, 42)
		{
		}

		public EditableFileUploadAttribute(string title, int sortOrder = 42)
			: base(title, sortOrder)
		{
		}
	}
}

//AbstractEditableAttribute, IRelativityTransformer, IWritingDisplayable, IDisplayable
//    {
//        private string alt = string.Empty;
//        private string cssClass = string.Empty;
//        private string uploadDirectory = string.Empty;


//        public EditableFileUploadAttribute()
//            : this(null, 42)
//        {
//        }

//        public EditableFileUploadAttribute(string title, int sortOrder)
//            : base(title, sortOrder)
//        {
//        }


        
//        /// <summary>Image alt text.</summary>
//        public string Alt
//        {
//            get { return alt; }
//            set { alt = value; }
//        }

//        /// <summary>CSS class on the image element.</summary>
//        public string CssClass
//        {
//            get { return cssClass; }
//            set { cssClass = value; }
//        }

//        /// <summary>Set a specific upload directory.</summary>
//        public string UploadDirectory
//        {
//            get { return uploadDirectory; }
//            set { uploadDirectory = value; }
//        }

//        /// <summary>CSS class on the image element.</summary>
//        public string UploadText { get; set; }

//        /// <summary>The image size to display by default if available.</summary>
//        public string PreferredSize { get; set; }



//        public override bool UpdateItem(ContentItem item, Control editor)
//        {
//            SelectingUploadCompositeControl composite = (SelectingUploadCompositeControl)editor;

//            HttpPostedFile postedFile = composite.UploadControl.PostedFile;
//            if (postedFile != null && !string.IsNullOrEmpty(postedFile.FileName))
//            {
//                IFileSystem fs = Engine.Resolve<IFileSystem>();

//                string directoryPath;
//                if (uploadDirectory == string.Empty)
//                    directoryPath = Engine.Resolve<IDefaultDirectory>().GetDefaultDirectory(item);
//                else
//                    directoryPath = uploadDirectory;


//                if (!fs.DirectoryExists(directoryPath))
//                    fs.CreateDirectory(directoryPath);

//                string fileName = Path.GetFileName(postedFile.FileName);
//                fileName = Regex.Replace(fileName, InvalidCharactersExpression, "-");
//                string filePath = VirtualPathUtility.Combine(directoryPath, fileName);

//                if (Engine.Config.Sections.Management.UploadFolders.IsTrusted(fileName))
//                {
//                    fs.WriteFile(filePath, postedFile.InputStream);
//                }
//                else
//                {
//                    throw new Security.PermissionDeniedException("Invalid file name");
//                }

//                item[Name] = Url.ToAbsolute(filePath);
//                return true;
//            } 

//            if (composite.SelectorControl.Url != item[Name] as string)
//            {
//                item[Name] = composite.SelectorControl.Url;
//                return true;
//            }

//            return false;
//        }
//		public bool ReadOnly { get; set; }

//        public override void UpdateEditor(ContentItem item, Control editor)
//        {
//            var composite = (SelectingUploadCompositeControl)editor;
//            composite.Select(item[Name] as string);
//        }

//        protected override Control AddEditor(Control container)
//        {
//            var composite = new SelectingUploadCompositeControl();
//            composite.ID = Name;
//            composite.UploadLabel.Text = UploadText ?? "Upload";
//            composite.SelectorControl.Placeholder(GetLocalizedText("Placeholder") ?? Placeholder);
//			if (ReadOnly)
//				composite.SelectorControl.Attributes["readonly"] = "readonly";
//            container.Controls.Add(composite);
//            return composite;
//        }

//        /// <summary>Adds a required field validator.</summary>
//        /// <param name="container">The container control for this validator.</param>
//        /// <param name="editor">The editor control to validate.</param>
//        protected override Control AddRequiredFieldValidator(Control container, Control editor)
//        {
//            var composite = (SelectingUploadCompositeControl)editor;
//			if (composite == null) return null;
//	        var rfv = new RequireEitherFieldValidator();
//	        rfv.ID = Name + "_rfv";
//	        rfv.ControlToValidate = composite.SelectorControl.ID;
//	        rfv.OtherControlToValidate = composite.UploadControl.ID;
//	        rfv.Display = ValidatorDisplay.Dynamic;
//	        rfv.Text = GetLocalizedText("RequiredText") ?? RequiredText;
//	        rfv.ErrorMessage = GetLocalizedText("RequiredMessage") ?? RequiredMessage;
//	        editor.Controls.Add(rfv);

//	        return rfv;
//        }


//        #region IDisplayable Members

//        public override Control AddTo(ContentItem item, string detailName, Control container)
//        {
//            using (var sw = new StringWriter())
//            {
//                Write(item, detailName, sw);

//                LiteralControl lc = new LiteralControl(sw.ToString());
//                container.Controls.Add(lc);
//                return lc;
//            }
//        }

//        #endregion

//        #region IRelativityTransformer Members

//        public RelativityMode RelativeWhen { get; set; }

//        string IRelativityTransformer.Rebase(string currentPath, string fromAppPath, string toAppPath)
//        {
//            return N2.Web.Url.Rebase(currentPath, fromAppPath, toAppPath);
//        }

//        #endregion

//        #region IWritingDisplayable Members

//        public virtual void Write(ContentItem item, string propertyName, TextWriter writer)
//        {
//            string url = item[propertyName] as string;
//            if (string.IsNullOrEmpty(url))
//                return;
            
//            string extension = VirtualPathUtility.GetExtension(url);
//            switch (ImagesUtility.GetExtensionGroup(extension))
//            {
//                case ImagesUtility.ExtensionGroups.Images:
//                    var sizes = DisplayableImageAttribute.GetSizes(PreferredSize);
//                    DisplayableImageAttribute.WriteImage(item, propertyName, sizes, alt, CssClass, writer);
//                    return;
//                default:
//                    WriteUrl(item, propertyName, cssClass, writer, url);
//                    return;
//            }
//        }

//        private static void WriteUrl(ContentItem item, string propertyName, string cssClass, TextWriter writer, string url)
//        {
//            cssClass = item[propertyName + "_CssClass"] as string ?? cssClass;
//            if(string.IsNullOrEmpty(cssClass))
//                writer.Write(string.Format("<a href=\"{0}\">{1}</a>", url, VirtualPathUtility.GetFileName(url)));
//            else
//                writer.Write(string.Format("<a href=\"{0}\" class=\"{1}\">{2}</a>", url, cssClass, VirtualPathUtility.GetFileName(url)));
//        }

//        #endregion

//        public const string ValidCharactersExpression = "^[^+]*$";
//        public const string InvalidCharactersExpression = "[+]";
//    }
//}
