using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Web.UI.WebControls;
using N2.Edit.FileSystem;
using System.Web;
using System.IO;
using System.Web.UI.HtmlControls;

namespace N2.Details
{
	/// <summary>
	/// Allows to upload or select a file to use.
	/// </summary>
	public class EditableFileUploadAttribute : AbstractEditableAttribute, IDisplayable
	{
		private string alt = string.Empty;
		private string cssClass = string.Empty;



		public EditableFileUploadAttribute()
			: base(null, 42)
		{
		}

		public EditableFileUploadAttribute(string title, int sortOrder)
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

		public override bool UpdateItem(ContentItem item, Control editor)
		{
			SelectorUploadComposite composite = (SelectorUploadComposite)editor;

			HttpPostedFile postedFile = composite.UploadControl.PostedFile;
			if (postedFile != null && !string.IsNullOrEmpty(postedFile.FileName))
			{
				IFileSystem fs = Engine.Resolve<IFileSystem>();
				string directoryPath = Engine.Resolve<IDefaultDirectory>().GetDefaultDirectory(item);
				if (!fs.DirectoryExists(directoryPath))
					fs.CreateDirectory(directoryPath);

				string fileName = Path.GetFileName(postedFile.FileName);
				string filePath = VirtualPathUtility.Combine(directoryPath, fileName);

				fs.WriteFile(filePath, postedFile.InputStream);

				item[Name] = filePath;
				return true;
			} 

			if (composite.SelectorControl.Url != item[Name] as string)
			{
				item[Name] = composite.SelectorControl.Url;
				return true;
			}

			return false;
		}

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			SelectorUploadComposite composite = (SelectorUploadComposite)editor;
			composite.Select(item[Name] as string);
		}

		protected override Control AddEditor(Control container)
		{
			SelectorUploadComposite composite = new SelectorUploadComposite();
			composite.ID = Name;
			container.Controls.Add(composite);
			return composite;
		}



		class SelectorUploadComposite : Control, INamingContainer
		{
			public HtmlGenericControl SelectorContainer { get; set; }
			public HtmlGenericControl UploadContainer { get; set; }
			public FileSelector SelectorControl { get; set; }
			public FileUpload UploadControl { get; set; }

			public SelectorUploadComposite()
			{
				SelectorControl = new FileSelector();
				SelectorControl.ID = "Selector";
				UploadControl = new FileUpload();
				UploadControl.ID = "Uploader";
			}

			protected override void CreateChildControls()
			{
				base.CreateChildControls();

				SelectorContainer = new HtmlGenericControl("span");
				SelectorContainer.Attributes["class"] = "uploadableContainer dimmable selector";
				Controls.Add(SelectorContainer);

				HtmlImage selectorImage = new HtmlImage();
				selectorImage.Src = "~/N2/Resources/Img/Ico/page_white_link.gif";
				SelectorContainer.Controls.Add(selectorImage);
				SelectorContainer.Controls.Add(SelectorControl);

				UploadContainer = new HtmlGenericControl("span");
				UploadContainer.Attributes["class"] = "uploadableContainer dimmable uploader";
				Controls.Add(UploadContainer);

				HtmlImage uploadImage = new HtmlImage();
				uploadImage.Src = "~/N2/Resources/Img/Ico/page_white_get.gif";
				UploadContainer.Controls.Add(uploadImage);
				UploadContainer.Controls.Add(UploadControl);
			}

			public void Select(string url)
			{
				EnsureChildControls();
				SelectorControl.Url = url;

				if (string.IsNullOrEmpty(url))
					SelectorContainer.Attributes["class"] = "uploadableContainer dimmable selector dimmed";
				else
					UploadContainer.Attributes["class"] = "uploadableContainer dimmable uploader dimmed";
			}
		}

		#region IDisplayable Members

		public Control AddTo(ContentItem item, string detailName, Control container)
		{
			string url = item[detailName] as string;
			if(!string.IsNullOrEmpty(url))
			{
				string extension = VirtualPathUtility.GetExtension(url);
				switch (extension.ToLower())
				{
					case ".gif":
					case ".png":
					case ".jpg":
					case ".jpeg":
						break;
					default:
						return null;
				}

				return DisplayableImageAttribute.AddImage(container, item, detailName, CssClass, Alt);
			}
			return null;
		}

		#endregion
	}
}
