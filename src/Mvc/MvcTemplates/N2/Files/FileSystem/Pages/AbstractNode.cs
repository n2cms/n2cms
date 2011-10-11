using System;
using N2.Definitions;
using N2.Engine;
using N2.Security;
using N2.Web;
using N2.Web.Drawing;
using N2.Persistence.Search;
using System.Collections.Generic;
using N2.Persistence;

namespace N2.Edit.FileSystem.Items
{
    [Throwable(AllowInTrash.No)]
	[Versionable(AllowVersions.No)]
	[PermissionRemap(From = Permission.Publish, To = Permission.Write)]
	[Indexable(IsIndexable = false)]
	public abstract class AbstractNode : ContentItem, INode, IFileSystemNode, IActiveChildren, IInjectable<IFileSystem>, IInjectable<ImageSizeCache>, IInjectable<IDependencyInjector>
    {
		public ImageSizeCache ImageSizes { get; protected set; }

		IFileSystem fileSystem;
		protected IDependencyInjector DependencyInjector { get; set; }

    	protected virtual IFileSystem FileSystem
    	{
			get { return fileSystem ?? (fileSystem = N2.Context.Current.Resolve<IFileSystem>()); }
			set { fileSystem = value; }
    	}

		public override string Path
		{
			get { return (Parent != null ? Parent.Path : "/" ) + Name + "/"; }
		}

		public virtual AbstractDirectory Directory
        {
            get { return Parent as AbstractDirectory; }
        }

        public override string Extension
        {
            get { return string.Empty; }
        }

        string INode.PreviewUrl
        {
			get { return N2.Web.Url.Parse(FindPath("info").TemplateUrl).AppendQuery(SelectionUtility.SelectedQueryKey, Path).ResolveTokens(); }
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

		#region IActiveChildren Members

		IEnumerable<ContentItem> IActiveChildren.GetChildren(Collections.ItemFilter filter)
		{
			return GetChildren(filter);
		}

		#endregion

		#region IInjectable<IFileSystem> Members

		public void Set(IFileSystem dependency)
		{
			fileSystem = dependency;
		}

		#endregion

		#region IInjectable<ImageSizeCache> Members

		public void Set(ImageSizeCache dependency)
		{
			ImageSizes = dependency;
		}

		#endregion

		#region IInjectable<ContentDependencyInjector> Members

		public void Set(IDependencyInjector dependency)
		{
			this.DependencyInjector = dependency;
		}

		#endregion
	}
}
