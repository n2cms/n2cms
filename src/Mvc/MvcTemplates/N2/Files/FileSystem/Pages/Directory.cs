using N2.Details;
using N2.Installation;
using N2.Integrity;
using N2.Persistence;
using N2.Engine;
using N2.Management.Api;

namespace N2.Edit.FileSystem.Items
{
    [PageDefinition("Directory",
        IconClass = "fa fa-folder-close",
        InstallerVisibility = InstallerHint.NeverRootOrStartPage,
        SortOrder = 2015)]
    [RestrictParents(typeof(AbstractDirectory))]
    [WithEditableName(Focus = true)]
    [N2.Web.Template("info", "{ManagementUrl}/Files/FileSystem/Directory.aspx")]
    [N2.Web.Template("upload", "{ManagementUrl}/Files/FileSystem/Upload.aspx")]
    [InterfaceFlags("Management")]
    public class Directory : AbstractDirectory, IActiveContent
    {
        string localUrl;
        string originalName;

        public Directory()
        {
        }

        public Directory(DirectoryData directory, ContentItem parent)
        {
            Parent = parent;

            originalName = directory.Name;
            Name = directory.Name;
            Title = directory.Name;
            Updated = directory.Updated;
            Created = directory.Created;
            localUrl = N2.Web.Url.ToAbsolute(directory.VirtualPath);
        }

        public override string LocalUrl
        {
            get
            {
                return localUrl
                  ?? (ParentDirectory != null
                    ? N2.Web.Url.Combine(ParentDirectory.localUrl, Name)
                    : Parent != null
                        ? N2.Web.Url.Combine(Parent.Url, Name)
                        : N2.Web.Url.Combine("~/", Name));
            }
        }

        public override string Url
        {
            get
            {
                return UrlPrefix + LocalUrl;
            }
        }

        public override string IconClass
        {
            get
            {
                if (base.GetFiles().Count > 0)
                    return "fa fa-folder-open";
                return base.IconClass;
            }
        }

        public virtual Directory ParentDirectory
        {
            get { return Parent as Directory; }
        }

        public override void AddTo(ContentItem newParent)
        {
            if (newParent is AbstractDirectory)
            {
                AbstractDirectory dir = EnsureDirectory(newParent);

                string to = Combine(dir.LocalUrl, Name);
                if (FileSystem.FileExists(to))
                    throw new NameOccupiedException(this, dir);

                if (FileSystem.DirectoryExists(LocalUrl))
                    FileSystem.MoveDirectory(LocalUrl, to);
                else
                    FileSystem.CreateDirectory(to);

                Parent = newParent;

                ClearUrl();
            }
            else if (newParent != null)
            {
                throw new N2Exception(newParent + " is not a Directory. AddTo only works on directories.");
            }
        }

        #region IActiveContent Members

        public void Save()
        {
            if (!string.IsNullOrEmpty(originalName) && Name != originalName)
            {
                var parentUrl = ParentDirectory != null ? ParentDirectory.LocalUrl : Parent.Url;
                string oldPath = N2.Web.Url.Combine(parentUrl, originalName);
                string newPath = N2.Web.Url.Combine(parentUrl, Name);
                FileSystem.MoveDirectory(oldPath, newPath);
                ClearUrl();
            }
            if (!FileSystem.DirectoryExists(LocalUrl))
                FileSystem.CreateDirectory(LocalUrl);
        }

        private void ClearUrl()
        {
            localUrl = null;
        }

        public void Delete()
        {
            FileSystem.DeleteDirectory(LocalUrl);
        }

        public void MoveTo(ContentItem destination)
        {
            AbstractDirectory d = EnsureDirectory(destination);

            string to = Combine(d.LocalUrl, Name);
            if (FileSystem.FileExists(to))
                throw new NameOccupiedException(this, d);

            FileSystem.MoveDirectory(LocalUrl, to);

            Parent = d;
            ClearUrl();
        }

        public ContentItem CopyTo(ContentItem destination)
        {
            AbstractDirectory d = AbstractDirectory.EnsureDirectory(destination);

            string to = Combine(d.LocalUrl, Name);
            if (FileSystem.FileExists(to))
                throw new NameOccupiedException(this, d);

            FileSystem.CreateDirectory(to);
            Directory copy = New(FileSystem.GetDirectory(to), d, DependencyInjector);

            foreach (File f in GetFiles())
                f.CopyTo(copy);

            foreach (Directory childDir in GetDirectories())
                childDir.CopyTo(copy);

            return copy;
        }

        #endregion

        internal static Items.Directory New(DirectoryData dir, ContentItem parent, IDependencyInjector injector)
        {
            var node = new Directory(dir, parent);
            injector.FulfilDependencies(node);
            return node;
        }

    }
}
