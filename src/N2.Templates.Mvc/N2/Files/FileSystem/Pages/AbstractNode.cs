using N2.Edit.Trash;
using N2.Persistence;
using N2.Web;
using N2.Management.Files;
using System;
using N2.Security;
using N2.Definitions;

namespace N2.Edit.FileSystem.Items
{
    [Throwable(AllowInTrash.No), Versionable(AllowVersions.No), PermissionRemap(From = Permission.Publish, To = Permission.Write)]
    public abstract class AbstractNode : ContentItem, INode
    {
		IFileSystem fileSystem;


		public AbstractNode(IFileSystem fs)
		{
			fileSystem = fs;
		}


    	protected virtual IFileSystem FileSystem
    	{
			get { return fileSystem; }
    	}

		public override string Path
		{
			get { return base.Path; }
		}

		public virtual AbstractDirectory Directory
        {
            get { return Parent as AbstractDirectory; }
        }

        public override string Extension
        {
            get { return string.Empty; }
        }

		protected string iconUrl;
		public override string IconUrl
		{
			get { return iconUrl ?? base.IconUrl; }
		}

        string INode.PreviewUrl
        {
			get { return N2.Web.Url.Parse(FindPath("info").TemplateUrl).AppendQuery("selected", Path).ResolveTokens(); }
        }

		public override PathData FindPath(string remainingUrl)
		{
			if (string.IsNullOrEmpty(remainingUrl))
				return PathData.NonRewritable(this);

			return base.FindPath(remainingUrl);
		}

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                return true;
            }
            AbstractNode other = obj as AbstractNode;
			return other != null
				&& string.Equals(other.Path, this.Path, StringComparison.InvariantCultureIgnoreCase);
		}

        public override int GetHashCode()
        {
            return (Url + ID).GetHashCode();
        }

		protected string Combine(string first, string second)
		{
			return first.TrimEnd('/') + "/" + second.TrimStart('/');
		}
    }
}
