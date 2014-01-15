using System;
using System.Linq;
using System.Web;
using N2.Configuration;
using N2.Engine;
using N2.Plugin;
using N2.Web;

namespace N2.Edit.FileSystem.NH
{
    /// <summary>
    /// Handles requests for files in the database file system.
    /// </summary>
    [Service(Configuration = "dbfs")]
    public class UploadFileHttpHandler : IHttpHandler, IAutoStart
    {
        private IFileSystem fileSystem;
        private UploadFolderSource folderSource;
        private EventBroker broker;

        public UploadFileHttpHandler(IFileSystem fileSystem, UploadFolderSource folderSource, EventBroker broker)
        {
            this.fileSystem = fileSystem;
            this.folderSource = folderSource;
            this.broker = broker;
        }

        public void ProcessRequest(HttpContext context)
        {
            if (fileSystem.FileExists(context.Request.Path))
            {
                context.Response.ContentType = GetMimeTypeFromExtension(context.Request.Path);
                fileSystem.ReadFileContents(context.Request.Path, context.Response.OutputStream);
            }
            else
            {
                throw new HttpException(404, "File not found: " + context.Request.Path);
            }
        }

        private static string GetMimeTypeFromExtension(string path)
        {
            switch (VirtualPathUtility.GetExtension(path.ToLower()))
            {
                case ".css":
                    return "text/css";
                case ".gif":
                    return "image/gif";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".js":
                    return "text/javascript";
                case ".pdf":
                    return "application/pdf";
                case ".png":
                    return "image/png";
                case ".txt":
                    return "text/plain";
                default:
                    return "application/octet-stream";
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }

        private void HttpApplication_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            if (app == null) return;


            var uploadFolders = folderSource.GetUploadFoldersForCurrentSite();
            if (!uploadFolders.Any(x => app.Request.Path.StartsWith(x.Path.TrimStart('~'), StringComparison.OrdinalIgnoreCase))) 
                return;

            if (!fileSystem.FileExists(app.Request.Path))
                return;

            app.Context.Handler = this;
        }

        #region IAutoStart Members

        public void Start()
        {
            broker.PreRequestHandlerExecute += HttpApplication_PreRequestHandlerExecute;
        }

        public void Stop()
        {
            broker.PreRequestHandlerExecute -= HttpApplication_PreRequestHandlerExecute;
        }

        #endregion
    }
}
