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
using N2.Web;

namespace N2.Details
{
	/// <summary>
	/// Allows to upload or select a file to use.
	/// </summary>
	public class EditableFileUploadAttribute : AbstractEditableAttribute, IRelativityTransformer, IWritingDisplayable, IDisplayable
	{
		private string alt = string.Empty;
		private string cssClass = string.Empty;



		public EditableFileUploadAttribute()
			: this(null, 42)
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

		/// <summary>CSS class on the image element.</summary>
		public string UploadText { get; set; }

		/// <summary>The image size to display by default if available.</summary>
		public string PreferredSize { get; set; }



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

				item[Name] = Url.ToAbsolute(filePath);
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
			composite.UploadLabel.Text = UploadText ?? "Upload";
			container.Controls.Add(composite);
			return composite;
		}



		class SelectorUploadComposite : Control, INamingContainer
		{
			public HtmlGenericControl SelectorContainer { get; set; }
			public HtmlGenericControl UploadContainer { get; set; }
			public FileSelector SelectorControl { get; set; }
			public Label UploadLabel { get; set; }
			public FileUpload UploadControl { get; set; }

			public SelectorUploadComposite()
			{
				SelectorControl = new FileSelector();
				SelectorControl.ID = "Selector";
				UploadControl = new FileUpload();
				UploadControl.ID = "Uploader";
				UploadLabel = new Label();
				UploadLabel.AssociatedControlID = UploadControl.ID;
			}

			protected override void CreateChildControls()
			{
				base.CreateChildControls();

				SelectorContainer = new HtmlGenericControl("span");
				SelectorContainer.Attributes["class"] = "uploadableContainer selector";
				Controls.Add(SelectorContainer);

				SelectorContainer.Controls.Add(SelectorControl);

				UploadContainer = new HtmlGenericControl("span");
				UploadContainer.Attributes["class"] = "uploadableContainer uploader";
				Controls.Add(UploadContainer);

				UploadContainer.Controls.Add(UploadLabel);
				UploadContainer.Controls.Add(UploadControl);
			}

			public void Select(string url)
			{
				EnsureChildControls();
				SelectorControl.Url = url;

				if (string.IsNullOrEmpty(url))
					SelectorContainer.Attributes["class"] = "uploadableContainer selector";
				else
					UploadContainer.Attributes["class"] = "uploadableContainer uploader";
			}
		}

		#region IDisplayable Members

		public override Control AddTo(ContentItem item, string detailName, Control container)
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

				return DisplayableImageAttribute.AddImage(container, item, detailName, PreferredSize, CssClass, Alt);
			}
			return null;
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

		public void Write(ContentItem item, string propertyName, TextWriter writer)
		{
			string url = item[propertyName] as string;
			if (string.IsNullOrEmpty(url))
				return;
			
			string extension = VirtualPathUtility.GetExtension(url);
			switch (extension.ToLower())
			{
				case ".gif":
				case ".png":
				case ".jpg":
				case ".jpeg":
					DisplayableImageAttribute.WriteImage(item, propertyName, PreferredSize, alt, CssClass, writer);
					return;
				default:
					WriteUrl(item, propertyName, cssClass, writer, url);
					return;
			}
		}

		private static void WriteUrl(ContentItem item, string propertyName, string cssClass, TextWriter writer, string url)
		{
			cssClass = item[propertyName + "_CssClass"] as string ?? cssClass;
			if(string.IsNullOrEmpty(cssClass))
				writer.Write(string.Format("<a href=\"{0}\">{2}</a>", url, VirtualPathUtility.GetFileName(url)));
			else
				writer.Write(string.Format("<a href=\"{0}\" class=\"{1}\">{2}</a>", url, cssClass, VirtualPathUtility.GetFileName(url)));
		}

		#endregion
	}
}
