using System.Collections.Generic;
using System.IO;
using N2.Collections;
using System.Diagnostics;
using System.Web;
using System;
using N2.Management.Files;

namespace N2.Edit.FileSystem.Items
{
    public abstract class AbstractDirectory : AbstractNode
    {
		public AbstractDirectory(IFileSystem fs)
			: base(fs)
		{
		}


		public override ContentItem GetChild(string childName)
		{
			string name = HttpUtility.UrlDecode(childName.Trim('/'));
			foreach (var file in GetFiles())
			{
				if (file.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					return file;

				foreach(var file2 in file.Children)
				{
					if (file2.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
						return file2;
				}
			}
			return base.GetChild(childName);
		}

		public virtual IList<File> GetFiles()
        {
            try
            {
				IList<File> files = new List<File>();

				string lastFileName = "";
				string lastFileExtension = "";
				File lastFile = null;
				foreach (var fd in FileSystem.GetFiles(Url))
				{
					var file = new File(FileSystem, fd, this);
					if (lastFile != null
						&& file.Name.StartsWith(lastFileName + ImagesUtility.Separator)
						&& file.Name.EndsWith(lastFileExtension))
					{
						int lastFileNameLength = (lastFileName + ImagesUtility.Separator).Length;
						if (file.Name.Substring(lastFileNameLength, file.Name.Length - lastFileNameLength - lastFileExtension.Length) == "icon")
							file.IsIcon = true;
						lastFile.Add(file);
					}
					else
					{
						files.Add(file);

						int dotIndex = file.Name.LastIndexOf('.');
						if (dotIndex >= 0)
						{
							lastFileName = file.Name.Substring(0, dotIndex);
							lastFileExtension = file.Name.Substring(dotIndex);
							lastFile = file;
						}
					}
				}

            	return files;
            }
            catch (DirectoryNotFoundException ex)
            {
                Trace.TraceWarning(ex.ToString());
                return new List<File>();
            }
		}

		public virtual IList<Directory> GetDirectories()
        {
            try
            {
				IList<Directory> directories = new List<Directory>();
				foreach(DirectoryData dir in FileSystem.GetDirectories(Url))
				{
					directories.Add(new Directory(FileSystem, dir, this));
				}
            	return directories;
            }
            catch (DirectoryNotFoundException ex)
            {
                Trace.TraceWarning(ex.ToString());
                return new List<Directory>();
            }
		}

        public override ItemList GetChildren(ItemFilter filter)
        {
            ItemList items = new ItemList();
            items.AddRange(filter.Pipe(GetDirectories()));
            items.AddRange(filter.Pipe(GetFiles()));
            return items;
        }

        public static AbstractDirectory EnsureDirectory(ContentItem item)
        {
        	if (item is AbstractDirectory)
                return item as AbstractDirectory;
        	
			throw new N2Exception(item + " is not a Directory.");
        }
    }
}
