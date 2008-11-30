using System.Web;
using System.IO;
using System.Web.Hosting;

namespace N2.Edit.Web
{
    public abstract class FileSiteMapNode : SiteMapNode
    {
        public FileSiteMapNode(SiteMapProvider provider, string url) 
            : base(provider, url)
        {
            Url = url;
			string path = HostingEnvironment.MapPath(url);
            Title = Path.GetFileName(path);
            //this.IsDirectory = !File.Exists(path);
            //if (!this.IsDirectory)
            FileExtension = Path.GetExtension(path).TrimStart('.');
        }

        private string fileExtension;
        //private bool isDirectory;

        public string FileExtension
        {
            get { return fileExtension; }
            set { fileExtension = value; }
        }
        
        //public bool IsDirectory
        //{
        //    get { return isDirectory; }
        //    set { isDirectory = value; }
        //}

        public abstract string IconUrl { get; }

        public abstract string Target { get; }
    }
}
