using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
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
                DirectoryInfo currentDirectory = new DirectoryInfo(PhysicalPath);
                FileInfo[] filesInDirectory = currentDirectory.GetFiles();
                List<File> files = new List<File>(filesInDirectory.Length);
                foreach (FileInfo fi in filesInDirectory)
                {
                    files.Add(CreateFile(fi));
                }
                return files;
            }
            catch (DirectoryNotFoundException ex)
            {
                Trace.TraceWarning(ex.ToString());
                return new List<File>();
            }
        }

        protected File CreateFile(FileInfo file)
        {
            File f = new File();
            f.Name = file.Name;
            f.Title = file.Name;
            f.Size = file.Length;
            f.Updated = file.LastWriteTime;
            f.Created = file.CreationTime;
            f.PhysicalPath = file.FullName;
            f.Parent = this;
            ((N2.Web.IUrlParserDependency)f).SetUrlParser(N2.Context.UrlParser);
            return f;
        }

        public IList<Directory> GetDirectories()
        {
            try
            {
                DirectoryInfo currentDirectory = new DirectoryInfo(PhysicalPath);
                DirectoryInfo[] subDirectories = currentDirectory.GetDirectories();
                List<Directory> directories = new List<Directory>(subDirectories.Length);
                foreach (DirectoryInfo subDirectory in subDirectories)
                {
                    if ((subDirectory.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                        directories.Add(CreateDirectory(subDirectory));
                }
                return directories;
            }
            catch (DirectoryNotFoundException ex)
            {
                Trace.TraceWarning(ex.ToString());
                return new List<Directory>();
            }
        }

        private Directory CreateDirectory(DirectoryInfo directory)
        {
            Directory d = new Directory();
            d.Name = directory.Name;
            d.Title = directory.Name;
            d.Updated = directory.LastWriteTime;
            d.Created = directory.CreationTime;
            d.PhysicalPath = directory.FullName;
            d.Parent = this;
            ((N2.Web.IUrlParserDependency)d).SetUrlParser(N2.Context.UrlParser);
            return d;
        }

        public override N2.Collections.ItemList GetChildren(ItemFilter filter)
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
            else
                throw new N2Exception(item + " is not a Directory.");
        }
    }
}
