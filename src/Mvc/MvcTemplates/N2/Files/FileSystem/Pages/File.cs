using System.IO;
using System.Text;
using System.Web;
using N2.Engine;
using N2.Installation;
using N2.Integrity;
using N2.Persistence;
using N2.Definitions;
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
    public class File : AbstractNode, IActiveContent, IFileSystemFile
    {
		public long Size { get; set; }
		public bool IsIcon { get; set; }
		private string iconUrl;

		public File()
		{
		}

		public File(FileData file, AbstractDirectory parent)
		{
			Parent = parent;

			NewName = file.Name;
			Name = file.Name;
			Title = file.Name;
			Size = file.Length;
			Updated = file.Updated;
			Created = file.Created;

			url = file.VirtualPath;
		}

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
				if (iconUrl != null)
					return iconUrl;

				string icon = ImagesUtility.GetResizedPath(Url, "icon");
				if (FileSystem.FileExists(icon))
					this.iconUrl = icon;
				else
					this.iconUrl = ImagesUtility.GetIconUrl(Url);

				return iconUrl;
			}
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
			file.Parent = this;
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
