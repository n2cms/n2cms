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
using N2.Security;

namespace N2.Edit.FileSystem.Items
{
	[Service]
	public class ImageSizeCache
	{
		public HashSet<string> ImageSizes { get; private set; }

		public ImageSizeCache(ConfigurationManagerWrapper config)
		{
			ImageSizes = new HashSet<string>(config.Sections.Management.Images.Sizes.AllElements.Select(ise => ise.Name), StringComparer.InvariantCultureIgnoreCase);
		}

		public virtual string GetSizeName(string fileName)
		{
			int separatorIndex;
			int dotIndex;
			if (!TryGetSeparatorAndDotIndex(fileName, out separatorIndex, out dotIndex))
				return null;

			var sizeName = fileName.Substring(separatorIndex + 1, dotIndex - separatorIndex - 1);
			if (!ImageSizes.Contains(sizeName))
				return null;

			return sizeName;
		}

		public virtual string RemoveImageSize(string fileName)
		{
			int separatorIndex;
			int dotIndex;
			if (!TryGetSeparatorAndDotIndex(fileName, out separatorIndex, out dotIndex))
				return null;

			return fileName.Substring(0, separatorIndex) + fileName.Substring(dotIndex);
		}

		private static bool TryGetSeparatorAndDotIndex(string fileName, out int separatorIndex, out int dotIndex)
		{
			separatorIndex = fileName.LastIndexOf(ImagesUtility.Separator[0]);
			dotIndex = fileName.LastIndexOf('.');

			return separatorIndex >= 0 && separatorIndex < dotIndex;
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

				var fileMap = new Dictionary<string, File>(StringComparer.OrdinalIgnoreCase);
				foreach (var fd in FileSystem.GetFiles(Url))
				{
					var file = new File(fd, this);
					file.Set(FileSystem);
					file.Set(ImageSizes);

					var unresizedFileName = ImageSizes.RemoveImageSize(file.Name);
					if (unresizedFileName != null && fileMap.ContainsKey(unresizedFileName))
					{
						fileMap[unresizedFileName].Add(file);

						if (ImageSizes.GetSizeName(file.Name) == "icon")
							file.IsIcon = true;
					}
					else
					{
						files.Add(file);
						fileMap[file.Name] = file;
					}
				}
				files.Sort(new TitleComparer<File>());
            	return files;
            }
            catch (DirectoryNotFoundException ex)
            {
				Engine.Logger.Warn(ex);
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
					if (!DynamicPermissionMap.IsAllRoles(this, Permission.Read))
						DynamicPermissionMap.SetRoles(node, Permission.Read, DynamicPermissionMap.GetRoles(this, Permission.Read).ToArray());
					if (!DynamicPermissionMap.IsAllRoles(this, Permission.Write))
						DynamicPermissionMap.SetRoles(node, Permission.Write, DynamicPermissionMap.GetRoles(this, Permission.Write).ToArray());
					directories.Add(node);
				}
				directories.Sort(new TitleComparer<Directory>());
            	return directories;
            }
            catch (DirectoryNotFoundException ex)
            {
				Engine.Logger.Warn(ex);
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
