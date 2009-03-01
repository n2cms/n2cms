using System.Collections.Generic;
using System.IO;
using N2.Collections;
using System.Diagnostics;

namespace N2.Edit.FileSystem.Items
{
    public abstract class AbstractDirectory : AbstractNode
    {
        public IList<File> GetFiles()
        {
            try
            {
				List<File> files = new List<File>();
				foreach(FileData file in FileSystem.GetFiles(Url))
				{
					files.Add(CreateFile(file));
				}
            	return files;
            }
            catch (DirectoryNotFoundException ex)
            {
                Trace.TraceWarning(ex.ToString());
                return new List<File>();
            }
        }

        public IList<Directory> GetDirectories()
        {
            try
            {
				List<Directory> directories = new List<Directory>();
				foreach(DirectoryData dir in FileSystem.GetDirectories(Url))
				{
					directories.Add(CreateDirectory(dir));
				}
            	return directories;
				//DirectoryInfo currentDirectory = new DirectoryInfo(PhysicalPath);
				//DirectoryInfo[] subDirectories = currentDirectory.GetDirectories();
				//List<Directory> directories = new List<Directory>(subDirectories.Length);
				//foreach (DirectoryInfo subDirectory in subDirectories)
				//{
				//    if ((subDirectory.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
				//        directories.Add(CreateDirectory(subDirectory));
				//}
				//return directories;
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
