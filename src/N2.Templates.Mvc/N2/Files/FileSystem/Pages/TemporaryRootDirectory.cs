using System;
using System.Collections.Generic;
using System.Web;
using N2.Edit.FileSystem.Items;
using N2.Collections;
using N2.Web;

namespace N2.Edit.FileSystem.Items
{
	[Template("info", "~/N2/Files/FileSystem/Directory.aspx")]
	public class TemporaryRootDirectory : AbstractDirectory
	{
		IList<string> configuredFolders;

		protected TemporaryRootDirectory()
		{
		}

		public TemporaryRootDirectory(IList<string> directories)
		{
			this.configuredFolders = directories;
		}

		public override string Url
		{
			get { return "~/"; }
		}

		public override IList<Directory> GetDirectories()
		{
			List<Directory> directories = new List<Directory>();
			foreach (var folder in configuredFolders)
			{
				var dir = FileSystem.GetDirectory(folder);
				directories.Add(new Directory(dir, this));
			}
			return directories;
		}

		public override IList<File> GetFiles()
		{
			return new List<File>();
		}
	}
}
