using System;
using System.Linq;
using System.Web;
using N2.Configuration;

namespace N2.Edit.FileSystem
{
    class UploadFileHttpHandler : IHttpHandler
    {
		IFileSystem fileSystem;

		public UploadFileHttpHandler(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
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

        internal static void HttpApplication_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            if (app == null) return;

			
            var uploadFolders = new EditSection().UploadFolders;

            if (uploadFolders.Folders.Any(x => app.Request.Path.StartsWith(x.TrimStart('~'), StringComparison.OrdinalIgnoreCase)))
            {
                app.Context.Handler = new UploadFileHttpHandler(Context.Current.Resolve<IFileSystem>());
            }
        }
    }
}
