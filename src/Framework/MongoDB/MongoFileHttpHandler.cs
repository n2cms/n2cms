using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using N2.Edit;
using N2.Edit.FileSystem;
using N2.Engine;
using N2.Plugin;
using N2.Web;

namespace N2.Persistence.MongoDB
{
    /// <summary>
    /// Handles requests for files in the mongo database.
    /// </summary>
    [Service(Configuration = "mongofs")]
    public class MongoFileHttpHandler : IHttpHandler, IAutoStart
    {
        private readonly IFileSystem fileSystem;
        private readonly UploadFolderSource folderSource;
        private readonly EventBroker broker;
        private readonly MD5 md5 = MD5.Create();

        public MongoFileHttpHandler(IFileSystem fileSystem, UploadFolderSource folderSource, EventBroker broker)
        {
            this.fileSystem = fileSystem;
            this.folderSource = folderSource;
            this.broker = broker;
        }

        public void ProcessRequest(HttpContext context)
        {
            if (fileSystem.FileExists(context.Request.Path))
            {
                FileData fileData = fileSystem.GetFile(context.Request.Path);

                String resourceEtag = CalculateEtag(context.Request.Path, fileData.Updated);
                string clientEtag = context.Request.Headers.Get("If-None-Match");

                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                // Cache response for one day - SetMaxAge does not seem to work as expected when using SetETag as well. This does the trick.
                context.Response.Cache.AppendCacheExtension(string.Concat("max-age=", new TimeSpan(1, 00, 00).TotalSeconds));
                
                if (!string.Equals(resourceEtag, clientEtag, StringComparison.InvariantCultureIgnoreCase))
                {
                    context.Response.Cache.SetETag(resourceEtag);

                    context.Response.ContentType = GetMimeTypeFromExtension(context.Request.Path);
                    fileSystem.ReadFileContents(context.Request.Path, context.Response.OutputStream);
                }
                else
                {
                    context.Response.ClearContent();
                    context.Response.StatusCode = (int)System.Net.HttpStatusCode.NotModified;
                    context.Response.SuppressContent = true;
                }
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

        private string CalculateEtag(string path, DateTime modifiedDateTime)
        {
            byte[] pathBytes = Encoding.UTF8.GetBytes(path);
            byte[] dateBytes = BitConverter.GetBytes(modifiedDateTime.ToBinary());
            byte[] md5bytes = md5.ComputeHash(CombineByteArrays(pathBytes, dateBytes));
            return String.Concat(Array.ConvertAll(md5bytes, x => x.ToString("X2")));
        }

        public static byte[] CombineByteArrays(byte[] first, byte[] second)
        {
            var ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
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
