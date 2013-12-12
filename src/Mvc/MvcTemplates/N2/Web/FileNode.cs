using System.Web;

namespace N2.Edit.Web
{
    public class FileNode : FileSiteMapNode
    {
        public FileNode(SiteMapProvider provider, string url) : base(provider, url)
        {
        }

        public override string IconUrl
        {
            get { return "../Resources/icons/page_white.png"; }
        }

        public override string Target
        {
            get { return "file"; }
        }
    }
}
