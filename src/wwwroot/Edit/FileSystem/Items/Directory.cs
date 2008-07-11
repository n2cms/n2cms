using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using N2.Integrity;
using System.Collections.Generic;
using System.IO;
using N2.Collections;
using N2.Web;
using System.Diagnostics;

namespace N2.Edit.FileSystem
{
    [RestrictParents(typeof(Directory))]
    public class Directory : ContentItem, INode
    {
        public virtual string PhysicalPath { get; set; }

        public override string IconUrl
        {
            get { return "~/Edit/img/ico/folder.gif"; }
        }

        public override string TemplateUrl
        {
            get { return "~/Edit/FileSystem/Directory.aspx"; }
        }

        string INode.PreviewUrl
        {
            get { return N2.Web.Url.Parse(Utility.ToAbsolute(TemplateUrl)).AppendQuery("selected", Path); }
        }

        public override void AddTo(ContentItem newParent)
        {
            if (newParent is Directory)
            {
                Directory dir = newParent as Directory;
                string path = System.IO.Path.Combine(dir.PhysicalPath, Name);
                if (System.IO.Directory.Exists(path))
                    throw new NameOccupiedException(this, newParent);
                System.IO.Directory.Move(PhysicalPath, path);
                PhysicalPath = path;
                Parent = newParent;
            }
            else
            {
                new N2Exception(newParent + " is not a Directory. AddTo only works on directories.");
            }
        }

        protected void AddToContentItem(ContentItem newParent)
        {
            base.AddTo(newParent);
        }

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
                    if((subDirectory.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
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
            return d;
        }

        public override N2.Collections.ItemList GetChildren(ItemFilter filter)
        {
            ItemList items = new ItemList();
            items.AddRange(filter.Pipe(GetDirectories()));
            items.AddRange(filter.Pipe(GetFiles()));
            return items;
        }
    }
}
