using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web;
using N2.Collections;
using N2.Web.Drawing;
using N2.Definitions;
using N2.Configuration;
using N2.Engine;

namespace N2.Edit.FileSystem.Items
{
	[Service]
	public class ImageSizeCache
	{
		public HashSet<string> ImageSizes { get; private set; }

		public ImageSizeCache(ConfigurationManagerWrapper config)
		{
			ImageSizes = new HashSet<string>(config.Sections.Management.Images.Sizes.AllElements.Select(ise => ise.Name));
		}
	}

    public abstract class AbstractDirectory : AbstractNode, IFileSystemDirectory
    {
    	protected override ContentItem FindNamedChild(string nameSegment)
    	{
    		return GetFiles().FirstOrDefault(f => f.Name == nameSegment) ??
    		       (ContentItem) GetDirectories().FirstOrDefault(d => d.Name == nameSegment);
    	}

		public virtual IList<File> GetFiles()
        {
            try
            {
				List<File> files = new List<File>();

				string lastFileName = "";
				string lastFileExtension = "";
				File lastFile = null;
				foreach (var fd in FileSystem.GetFiles(Url))
				{
					var file = new File(fd, this);
					file.Set(FileSystem);
					file.Set(ImageSizes);

					if (lastFile != null
						&& file.Name.StartsWith(lastFileName + ImagesUtility.Separator)
						&& file.Name.EndsWith(lastFileExtension))
					{
						if (!ImageSizes.ImageSizes.Contains(GetSizeName(lastFileName, lastFileExtension, file)))
						{
							files.Add(file);
							continue;
						}

						if (GetSizeName(lastFileName, lastFileExtension, file) == "icon")
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
				files.Sort(new TitleComparer<File>());
            	return files;
            }
            catch (DirectoryNotFoundException ex)
            {
                Trace.TraceWarning(ex.ToString());
                return new List<File>();
            }
		}

		private static string GetSizeName(string lastFileName, string lastFileExtension, File file)
		{
			int lastFileNameLength = (lastFileName + ImagesUtility.Separator).Length;
			string size = file.Name.Substring(lastFileNameLength, file.Name.Length - lastFileNameLength - lastFileExtension.Length);
			return size;
		}

		public virtual IList<Directory> GetDirectories()
        {
            try
            {
				List<Directory> directories = new List<Directory>();
				foreach(DirectoryData dir in FileSystem.GetDirectories(Url))
				{
					var node = Items.Directory.New(dir, this, DependencyInjector);
					directories.Add(node);
				}
				directories.Sort(new TitleComparer<Directory>());
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

		class TitleComparer<T> : IComparer<T> where T: ContentItem
		{
			#region IComparer<ContentItem> Members

			public int Compare(T x, T y)
			{
				return StringComparer.InvariantCultureIgnoreCase.Compare(x.Title, y.Title);
			}

			#endregion
		}


        public static AbstractDirectory EnsureDirectory(ContentItem item)
        {
        	if (item is AbstractDirectory)
                return item as AbstractDirectory;
        	
			throw new N2Exception(item + " is not a Directory.");
        }
	}
}
