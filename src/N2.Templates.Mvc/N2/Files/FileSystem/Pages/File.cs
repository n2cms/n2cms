using System.IO;
using N2.Integrity;
using N2.Persistence;
using N2.Installation;
using Management.N2.Files;
using System.Web;

namespace N2.Edit.FileSystem.Items
{
    [PageDefinition("File",
        IconUrl = "~/N2/Resources/Img/ico/png/page_white.png",
		InstallerVisibility = InstallerHint.NeverRootOrStartPage,
		SortOrder = 2010)]
    [RestrictParents(typeof(AbstractDirectory))]
    [Editables.EditableUpload]
	[N2.Web.Template("info", "~/N2/Files/FileSystem/File.aspx")]
    public class File : AbstractNode, IActiveContent
    {
		protected File() 
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

			string icon = ImagesUtility.GetResizedPath(file.VirtualPath, "icon");
			if (FileSystem.FileExists(icon))
				this.iconUrl = icon;
		}

        public long Size { get; set; }

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
			}
        }

        public void Delete()
        {
			FileSystem.DeleteFile(Url);
        }

        public void MoveTo(ContentItem destination)
        {
            AbstractDirectory d = AbstractDirectory.EnsureDirectory(destination);

			string to = Combine(d.Url, Name);
			if (FileSystem.FileExists(to))
				throw new NameOccupiedException(this, d);

			FileSystem.MoveFile(Url, to);
        	Parent = d;
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
	}
}
