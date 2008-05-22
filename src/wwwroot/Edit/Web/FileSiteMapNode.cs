using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

namespace N2.Edit.Web
{
    public class FileSiteMapNode : System.Web.SiteMapNode
    {
        public FileSiteMapNode(SiteMapProvider provider, string url) 
            : base(provider, url)
        {
            this.Url = url;
            string path = HttpContext.Current.Server.MapPath(url);
            this.Title = Path.GetFileName(path);
            this.IsDirectory = !File.Exists(path);
            if (!this.IsDirectory)
                this.FileExtension = Path.GetExtension(path).TrimStart('.');
        }

        private string fileExtension;
        private bool isDirectory;

        public string FileExtension
        {
            get { return fileExtension; }
            set { fileExtension = value; }
        }
        
        public bool IsDirectory
        {
            get { return isDirectory; }
            set { isDirectory = value; }
        }
    }
}
