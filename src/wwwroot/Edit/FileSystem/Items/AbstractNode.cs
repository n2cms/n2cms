using System.Web.Hosting;
using N2.Edit.Trash;
using N2.Persistence;

namespace N2.Edit.FileSystem.Items
{
    [NotThrowable, NotVersionable]
    public abstract class AbstractNode : ContentItem, INode
    {
    	protected virtual IFileSystem FileSystem
    	{
			get { return Context.Current.Resolve<IFileSystem>(); }
    	}

        public override string Title
        {
            get { return Name; }
            set { Name = value; }
        }

		public virtual AbstractDirectory Directory
        {
            get { return Parent as AbstractDirectory; }
        }

    	public string PhysicalPath
    	{
			get { return HostingEnvironment.MapPath(Url); }
    	}

        public override string IconUrl
        {
            get { return "~/Edit/img/ico/folder.gif"; }
        }

        public override string TemplateUrl
        {
            get { return "~/Edit/FileSystem/Directory.aspx"; }
        }

        public override string Extension
        {
            get { return string.Empty; }
        }

        string INode.PreviewUrl
        {
            get { return N2.Web.Url.Parse(TemplateUrl).AppendQuery("selected", Path); }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                return true;
            }
            AbstractNode otherNode = obj as AbstractNode;
            return otherNode != null && Url == otherNode.Url;
        }

        public override int GetHashCode()
        {
            return (Url + ID).GetHashCode();
        }

		protected File CreateFile(FileData file)
		{
			File f = new File();
			f.Name = file.Name;
			f.Title = file.Name;
			f.Size = file.Length;
			f.Updated = file.Updated;
			f.Created = file.Created;
			f.Parent = this;
			((N2.Web.IUrlParserDependency)f).SetUrlParser(N2.Context.UrlParser);
			return f;
		}

		protected Directory CreateDirectory(DirectoryData directory)
		{
			Directory d = new Directory();
			d.Name = directory.Name;
			d.Title = directory.Name;
			d.Updated = directory.Updated;
			d.Created = directory.Created;
			d.Parent = this;
			((N2.Web.IUrlParserDependency)d).SetUrlParser(N2.Context.UrlParser);
			return d;
		}
    }
}
