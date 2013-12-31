using System.Web;

namespace N2.Edit.Web
{
    public abstract class FileSiteMapNode : SiteMapNode
    {
        public FileSiteMapNode(SiteMapProvider provider, string url) 
            : base(provider, url)
        {
            Url = url;
            Title = VirtualPathUtility.GetFileName(url);
            FileExtension = VirtualPathUtility.GetExtension(url).TrimStart('.');
        }

        private string fileExtension;

        public string FileExtension
        {
            get { return fileExtension; }
            set { fileExtension = value; }
        }

        public abstract string IconUrl { get; }

        public abstract string Target { get; }
    }
}
