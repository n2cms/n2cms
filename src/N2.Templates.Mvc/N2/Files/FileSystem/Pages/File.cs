using System.IO;
using System.Text;
using System.Web;
using N2.Installation;
using N2.Integrity;
using N2.Persistence;
using N2.Web.Drawing;

namespace N2.Edit.FileSystem.Items
{
    [PageDefinition("File",
        IconUrl = "{ManagementUrl}/Resources/icons/page_white.png",
		InstallerVisibility = InstallerHint.NeverRootOrStartPage,
		SortOrder = 2010)]
    [RestrictParents(typeof(AbstractDirectory))]
    [Editables.EditableUpload]
	[N2.Web.Template("info", "{ManagementUrl}/Files/FileSystem/File.aspx")]
    public class File : AbstractNode, IActiveContent
    {
		protected File()
			: base(N2.Context.Current.Resolve<IFileSystem>())
		{
		}

		public File(IFileSystem fs, FileData file, AbstractDirectory parent)
			: base(fs)
		{
			Parent = parent;

			NewName = file.Name;
			Name = file.Name;
			Title = file.Name;
			Size = file.Length;
			Updated = file.Updated;
			Created = file.Created;

			url = file.VirtualPath;

			string icon = ImagesUtility.GetResizedPath(file.VirtualPath, "icon");
			if (FileSystem.FileExists(icon))
				this.iconUrl = icon;
		}

		public long Size { get; set; }
		public bool IsIcon { get; set; }

        public override void AddTo(ContentItem newParent)
        {
            if (newParent != null)
                MoveTo(newParent);
        }

		string url;
		public override string Url
		{
			get { return url ?? N2.Web.Url.Combine(Parent.Url, Name); }
		}

		public override bool IsPage
		{
			get { return !IsIcon; }
		}

		public override string IconUrl
		{
			get
			{
				if(iconUrl != null)
					return base.IconUrl;

				string extension = VirtualPathUtility.GetExtension(Name).ToLower();
				switch (extension)
				{
					case ".gif":
					case ".png":
					case ".jpg":
					case ".jpeg":
						return IconPath("page_white_picture");
					case ".pdf":
						return IconPath("page_white_acrobat");
					case ".cs":
					case ".vb":
					case ".js":
						return IconPath("page_white_csharp");
					case ".html":
					case ".htm":
					case ".xml":
					case ".xsd":
					case ".xslt":
					case ".aspx":
					case ".ascx":
					case ".ashx":
					case ".php":
						return IconPath("page_white_code");
					case ".zip":
					case ".gz":
					case ".7z":
					case ".rar":
						return IconPath("page_white_compressed");
					case ".swf":
						return IconPath("page_white_flash");
					case ".txt":
						return IconPath("page_white_text");
					case ".xls":
					case ".xlsx":
						return IconPath("page_white_excel");
					case ".xps":
					case ".ppt":
					case ".pptx":
						return IconPath("page_white_powerpoint");
					case ".doc":
					case ".docx":
						return IconPath("page_white_word");
					default:
						return IconPath("page_white");
				}
			}
		}

		private string IconPath(string iconName)
		{
			return Context.Current.ManagementPaths.ResolveResourceUrl(string.Format("{{ManagementUrl}}/Resources/icons/{0}.png", iconName));
		}

		public bool Exists
		{
			get { return Url != null && FileSystem.FileExists(Url); }
		}

    	public string NewName { get; set; }

		public virtual void TransmitTo(Stream stream)
		{
			FileSystem.ReadFileContents(Url, stream);
		}

		public void WriteToDisk(Stream stream)
		{
			if (!FileSystem.DirectoryExists(Directory.Url))
				FileSystem.CreateDirectory(Directory.Url);

			FileSystem.WriteFile(Url, stream);
		}

		internal void Add(File file)
		{
			Children.Add(file);
		}

		#region IActiveContent Members

		public void Save()
        {
			if (!string.IsNullOrEmpty(NewName))
			{
				FileSystem.MoveFile(Url, Combine(Directory.Url, NewName));
				Name = NewName;
				InvalidateUrl();
			}
        }

        public void Delete()
        {
			FileSystem.DeleteFile(Url);
			InvalidateUrl();
        }

        public void MoveTo(ContentItem destination)
        {
            AbstractDirectory d = AbstractDirectory.EnsureDirectory(destination);

			string to = Combine(d.Url, Name);
			if (FileSystem.FileExists(to))
				throw new NameOccupiedException(this, d);

			FileSystem.MoveFile(Url, to);
        	Parent = d;
			InvalidateUrl();
        }

		public ContentItem CopyTo(ContentItem destination)
        {
            AbstractDirectory d = AbstractDirectory.EnsureDirectory(destination);

			string to = Combine(d.Url, Name);
			if (FileSystem.FileExists(to))
				throw new NameOccupiedException(this, d);

			FileSystem.CopyFile(Url, to);

			return d.GetChild(Name);
        }

        #endregion

		public string ReadFile()
		{
			using (var fs = FileSystem.OpenFile(Url))
			using (var sr = new StreamReader(fs))
			{
				return sr.ReadToEnd();
			}
		}

		public void WriteFile(string text)
		{
			using (var ms = new MemoryStream(Encoding.ASCII.GetBytes(text)))
			{
				FileSystem.WriteFile(Url, ms);
			}
		}

		protected void InvalidateUrl()
		{
			this.url = null;
		}
	}
}
