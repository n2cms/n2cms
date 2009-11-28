using System.IO;
using N2.Integrity;
using N2.Persistence;
using N2.Installation;

namespace N2.Edit.FileSystem.Items
{
    [PageDefinition("File",
        IconUrl = "~/Edit/img/ico/png/page_white.png",
		InstallerVisibility = InstallerHint.NeverRootOrStartPage)]
    [RestrictParents(typeof(AbstractDirectory))]
    [Editables.EditableUpload]
	[N2.Web.Template("info", "~/Edit/FileSystem/File.aspx")]
    public class File : AbstractNode, IActiveContent
    {
        public long Size { get; set; }

        public override void AddTo(ContentItem newParent)
        {
            if (newParent != null)
                MoveTo(newParent);
        }

		public override string Url
		{
			get { return Directory.Url + "/" + Name; }
		}

		public virtual void TransmitTo(Stream stream)
		{
			FileSystem.ReadFileContents(Url, stream);
		}

		public void WriteToDisk(Stream stream)
		{
			FileSystem.WriteFile(Url, stream);
		}

		public bool Exists
		{
			get { return FileSystem.FileExists(Url); }
		}

    	public string NewName { get; set; }

    	#region IActiveRecord Members

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
