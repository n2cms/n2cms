using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using N2.Edit.FileSystem.Items;
using N2.Management.Files;
using N2.Engine;
using N2.Edit.FileSystem;
using N2.Edit;
using N2.Persistence;
using N2.Collections;
using N2.Security;

namespace N2.Management.Files
{
	/// <summary>
	/// A custom <see cref="INodeProvider" /> that enumerates the FileSystem as special ContentItem instances
	/// </summary>
	[Service]
	public class FolderNodeProvider : INodeProvider
	{
		private IFileSystem fs;
		private IPersister persister;
		private IDependencyInjector dependencyInjector;

		internal FolderPair[] UploadFolderPaths { get; set; }

		public FolderNodeProvider(IFileSystem fs, IPersister persister, IDependencyInjector dependencyInjector)
		{
			UploadFolderPaths = new FolderPair[0];
			this.fs = fs;
			this.persister = persister;
			this.dependencyInjector = dependencyInjector;
		}


		#region INodeProvider Members

		public ContentItem Get(string path)
		{
			foreach (var pair in UploadFolderPaths)
			{
				if (path.StartsWith(pair.Path, StringComparison.InvariantCultureIgnoreCase))
				{
					var dir = CreateDirectory(pair);

					string remaining = path.Substring(pair.Path.Length);
					if (string.IsNullOrEmpty(remaining))
						return dir;
					return dir.GetChild(remaining);
				}
			}

			return null;
		}

		public IEnumerable<ContentItem> GetChildren(string path)
		{
			foreach (var pair in UploadFolderPaths)
			{
				if (pair.ParentPath.Equals(path, StringComparison.InvariantCultureIgnoreCase))
				{
					yield return CreateDirectory(pair);
				}
                else if (path.StartsWith(pair.Path, StringComparison.InvariantCultureIgnoreCase))
				{
					ContentItem dir = CreateDirectory(pair);

					var subdirPath = path.Substring(pair.Path.Length);
					if (subdirPath != "")
					{
						dir = dir.GetChild(subdirPath);
					}

                    if (dir != null)
                    {
						foreach (var child in dir.GetChildren(new NullFilter()))
						{
							yield return child;
						}
                    }
				}
			}
		}

		private Directory CreateDirectory(FolderPair pair)
		{
			var dd = fs.GetDirectoryOrVirtual(pair.Folder.Path);
			var parent = persister.Get(pair.ParentID);

			var dir = Directory.New(dd, parent, dependencyInjector);
			dir.Name = pair.Path.Substring(pair.ParentPath.Length).Trim('/');
			dir.Title = pair.Folder.Title ?? dir.Name;

			Apply(pair.Folder.Readers, dir);
			Apply(pair.Folder.Writers, dir);
			
			return dir;
		}

		private static void Apply(PermissionMap map, Directory dir)
		{
			if (map.IsAltered)
				DynamicPermissionMap.SetRoles(dir, map.Permissions, map.Roles);
			else
				DynamicPermissionMap.SetAllRoles(dir, map.Permissions);
		}

		#endregion
	}
}
