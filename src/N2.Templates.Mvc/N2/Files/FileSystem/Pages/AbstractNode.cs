using N2.Edit.Trash;
using N2.Persistence;
using N2.Web;
using Management.N2.Files;

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
			get { return N2.Web.Url.Parse(FindPath("info").TemplateUrl).AppendQuery("selected", Path); }
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
            AbstractNode otherNode = obj as AbstractNode;
            return otherNode != null && Url == otherNode.Url;
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
