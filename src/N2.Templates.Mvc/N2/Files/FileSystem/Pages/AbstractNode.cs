using N2.Edit.Trash;
using N2.Persistence;
using N2.Web;
using N2.Management.Files;
using System;
using N2.Security;
using N2.Definitions;
using N2.Web.Drawing;

namespace N2.Edit.FileSystem.Items
{
    [Throwable(AllowInTrash.No), Versionable(AllowVersions.No), PermissionRemap(From = Permission.Publish, To = Permission.Write)]
    public abstract class AbstractNode : ContentItem, INode, IDependentEntity<IFileSystem>
    {
		IFileSystem fileSystem;
		protected string iconUrl;

    	protected virtual IFileSystem FileSystem
    	{
			get { return fileSystem ?? (fileSystem = N2.Context.Current.Resolve<IFileSystem>()); }
			set { fileSystem = value; }
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

		public override string IconUrl
		{
			get 
			{
				if (iconUrl == null)
				{
					string icon = ImagesUtility.GetResizedPath(Url, "icon");
					if (FileSystem.FileExists(icon))
						this.iconUrl = icon;
					else
						this.iconUrl = base.IconUrl;
				}
				return iconUrl;
			}
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

		#region IDependentEntity<IFileSystem> Members

		public void Set(IFileSystem dependency)
		{
			fileSystem = dependency;
		}

		#endregion
	}
}
