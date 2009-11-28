using System.Web;

namespace N2.Edit.Web
{
    public class DirectoryNode : FileSiteMapNode
    {
        public DirectoryNode(SiteMapProvider provider, string url) : base(provider, url)
        {
        }

        public override string IconUrl
        {
            get { return "../img/ico/png/folder.png"; }
        }

        public override string Target
        {
            get { return "folder"; }
        }
    }
}
