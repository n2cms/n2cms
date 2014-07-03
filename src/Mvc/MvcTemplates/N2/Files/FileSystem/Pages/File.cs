using System.IO;
using System.Text;
using System.Web;
using N2.Engine;
using N2.Installation;
using N2.Integrity;
using N2.Persistence;
using N2.Definitions;
using N2.Web.Drawing;
using N2.Management.Api;

namespace N2.Edit.FileSystem.Items
{
    [PageDefinition("File",
        IconClass = "fa fa-file-text-o",
        InstallerVisibility = InstallerHint.NeverRootOrStartPage,
        SortOrder = 2010)]
    [RestrictParents(typeof(AbstractDirectory))]
    [Editables.EditableUpload]
    [N2.Web.Template("info", "{ManagementUrl}/Files/FileSystem/File.aspx")]
    [InterfaceFlags("Management")]
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
        }

        public override void AddTo(ContentItem newParent)
        {
            if (newParent != null)
                MoveTo(newParent);
        }

        public override string LocalUrl
        {
            get { return N2.Web.Url.Combine(Directory.LocalUrl, Name); }
        }

        public override string Url
        {
            get 
            {
                return N2.Web.Url.Combine(Directory.Url, Name); 
            }
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

                string icon = ImagesUtility.GetResizedPath(LocalUrl, "icon");
                if (FileSystem.FileExists(icon))
                    this.iconUrl = icon;
                else
                    this.iconUrl = ImagesUtility.GetIconUrl(LocalUrl);

                return iconUrl;
            }
        }

        public bool Exists
        {
            get { return LocalUrl != null && FileSystem.FileExists(LocalUrl); }
        }

        public string NewName { get; set; }

        public virtual void TransmitTo(Stream stream)
        {
            FileSystem.ReadFileContents(LocalUrl, stream);
        }

        public void WriteToDisk(Stream stream)
        {
            if (!FileSystem.DirectoryExists(Directory.LocalUrl))
                FileSystem.CreateDirectory(Directory.LocalUrl);

            FileSystem.WriteFile(LocalUrl, stream);
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
                FileSystem.MoveFile(LocalUrl, Combine(Directory.LocalUrl, NewName));
                Name = NewName;
            }
        }

        public void Delete()
        {
            FileSystem.DeleteFile(LocalUrl);
        }

        public void MoveTo(ContentItem destination)
        {
            AbstractDirectory d = AbstractDirectory.EnsureDirectory(destination);

            string to = Combine(d.LocalUrl, Name);
            if (FileSystem.FileExists(to))
                throw new NameOccupiedException(this, d);

            FileSystem.MoveFile(LocalUrl, to);
            Parent = d;
        }

        public ContentItem CopyTo(ContentItem destination)
        {
            AbstractDirectory d = AbstractDirectory.EnsureDirectory(destination);

            string to = Combine(d.LocalUrl, Name);
            if (FileSystem.FileExists(to))
                throw new NameOccupiedException(this, d);

            FileSystem.CopyFile(LocalUrl, to);

            return d.GetChild(Name);
        }

        #endregion

        public string ReadFile()
        {
            using (var fs = FileSystem.OpenFile(LocalUrl))
            using (var sr = new StreamReader(fs))
            {
                return sr.ReadToEnd();
            }
        }

        public void WriteFile(string text)
        {
            using (var ms = new MemoryStream(Encoding.ASCII.GetBytes(text)))
            {
                FileSystem.WriteFile(LocalUrl, ms);
            }
        }
    }
}
