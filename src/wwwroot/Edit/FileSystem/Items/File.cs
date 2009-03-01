using System;
using System.IO;
using N2.Integrity;
using N2.Persistence;
using System.Diagnostics;
using N2.Installation;
using System.Web;

namespace N2.Edit.FileSystem.Items
{
    [Definition(Installer = InstallerHint.NeverRootOrStartPage)]
    [RestrictParents(typeof(AbstractDirectory))]
    [Editables.EditableUpload]
    public class File : AbstractNode, IActiveContent
    {
        public long Size { get; set; }

        public override string IconUrl
        {
            get { return "~/Edit/img/ico/page_white.gif"; }
        }

        public override string TemplateUrl
        {
            get { return "~/Edit/FileSystem/File.aspx"; }
        }

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

        #region IActiveRecord Members

        public void Save()
        {
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
