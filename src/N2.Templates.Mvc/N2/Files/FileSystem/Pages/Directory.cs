using N2.Integrity;
using N2.Details;
using N2.Persistence;
using N2.Installation;
using System.Web;

namespace N2.Edit.FileSystem.Items
{
	[PageDefinition("Directory",
		IconUrl = "{ManagementUrl}/Resources/icons/folder.png",
		InstallerVisibility = InstallerHint.NeverRootOrStartPage,
		SortOrder = 2015)]
	[RestrictParents(typeof(AbstractDirectory))]
	[WithEditableName(Focus = true)]
	[N2.Web.Template("info", "{ManagementUrl}/Files/FileSystem/Directory.aspx")]
	[N2.Web.Template("upload", "{ManagementUrl}/Files/FileSystem/Upload.aspx")]
	public class Directory : AbstractDirectory, IActiveContent
	{
		string url;
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
			url = directory.VirtualPath;
		}

		public override string Url
		{
			get { return url ?? N2.Web.Url.Combine(Parent.Url, Name); }
		}

		public override string IconUrl
		{
			get
			{
				if (base.GetFiles().Count > 0)
					return Context.Current.ManagementPaths.ResolveResourceUrl("{ManagementUrl}/Resources/icons/folder_page_white.png");
				return base.IconUrl;
			}
		}

		public override void AddTo(ContentItem newParent)
		{
			if (newParent is AbstractDirectory)
			{
				AbstractDirectory dir = EnsureDirectory(newParent);

				string to = Combine(dir.Url, Name);
				if (FileSystem.FileExists(to))
					throw new NameOccupiedException(this, dir);

				if (FileSystem.DirectoryExists(Url))
					FileSystem.MoveDirectory(Url, to);
				else
					FileSystem.CreateDirectory(to);

				Parent = newParent;
			}
			else if (newParent != null)
			{
				throw new N2Exception(newParent + " is not a Directory. AddTo only works on directories.");
			}
		}

		#region IActiveContent Members

		public void Save()
		{
			if (Name != originalName)
			{
				string oldPath = N2.Web.Url.Combine(Parent.Url, originalName);
				string newPath = N2.Web.Url.Combine(Parent.Url, Name);
				FileSystem.MoveDirectory(oldPath, newPath);
				ClearUrl();
			}
			if (!FileSystem.DirectoryExists(Url))
				FileSystem.CreateDirectory(Url);
		}

		private void ClearUrl()
		{
			url = null;
		}

		public void Delete()
		{
			FileSystem.DeleteDirectory(Url);
		}

		public void MoveTo(ContentItem destination)
		{
			AbstractDirectory d = EnsureDirectory(destination);

			string to = Combine(d.Url, Name);
			if (FileSystem.FileExists(to))
				throw new NameOccupiedException(this, d);

			FileSystem.MoveDirectory(Url, to);
		}

		public ContentItem CopyTo(ContentItem destination)
		{
			AbstractDirectory d = AbstractDirectory.EnsureDirectory(destination);

			string to = Combine(d.Url, Name);
			if (FileSystem.FileExists(to))
				throw new NameOccupiedException(this, d);

			FileSystem.CreateDirectory(to);
			Directory copy = new Directory(FileSystem.GetDirectory(to), d);
			copy.Set(FileSystem);

			foreach (File f in GetFiles())
				f.CopyTo(copy);

			foreach (Directory childDir in GetDirectories())
				childDir.CopyTo(copy);

			return copy;
		}

		#endregion
	}
}