using System;
using N2.Definitions;
using N2.Engine;
using N2.Security;
using N2.Web;
using N2.Web.Drawing;
using N2.Persistence.Search;
using System.Collections.Generic;
using N2.Persistence;
using N2.Management.Files.FileSystem.Pages;

namespace N2.Edit.FileSystem.Items
{
    [Adapts(typeof(AbstractNode))]
    public class AbstractNodeAdapter : NodeAdapter
    {
        public override string GetPreviewUrl(ContentItem item)
        {
            return N2.Web.Url.Parse(item.FindPath("info").TemplateUrl).AppendQuery(SelectionUtility.SelectedQueryKey, item.Path).ResolveTokens();
        }

        public override string GetPreviewUrl(ContentItem item, bool allowDraft)
        {
            return GetPreviewUrl(item);
        }
    }

    [Throwable(AllowInTrash.No)]
    [Versionable(AllowVersions.No)]
    [PermissionRemap(From = Permission.Publish, To = Permission.Write)]
    [Indexable(IsIndexable = false)]
    public abstract class AbstractNode : ContentItem, IFileSystemNode, IActiveChildren, IInjectable<IFileSystem>, IInjectable<ImageSizeCache>, IInjectable<IDependencyInjector>
    {
        public AbstractNode()
        {
            State = ContentState.Published;
        }

        IFileSystem fileSystem;

        public abstract string LocalUrl { get; }

        protected ImageSizeCache ImageSizes { get; set; }
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
            get 
            {
                if (Parent is AbstractDirectory)
                    return Parent as AbstractDirectory;
                else if (Parent is File)
                    return Parent.Parent as AbstractDirectory;
                else
                    return null;
            }
        }

        public override string Extension
        {
            get { return string.Empty; }
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
