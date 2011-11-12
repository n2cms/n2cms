using System.Web;
using System.Linq;
using System.Web.Security;
using N2.Edit;
using N2.Edit.FileSystem;
using N2.Web;
using System.Collections.Generic;
using System.IO;

namespace N2.Management.Files.FileSystem
{
	public class UploadFile : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.AddHeader("Cache-Control", "private, no-cache");

            UploadTheFile(context);

            //var request = context.Request;

            //var ticket = FormsAuthentication.Decrypt(request["ticket"]);
            //if (ticket.Expired)
            //    throw new N2.N2Exception("Upload ticket expired");
            //if(!ticket.Name.StartsWith("SecureUpload-"))
            //    throw new N2.N2Exception("Unknown ticket");

            //SelectionUtility selection = new SelectionUtility(request, N2.Context.Current);

            //if (request.Files.Count > 0)
            //{
            //    var fs = N2.Context.Current.Resolve<IFileSystem>();
            //    foreach (string key in request.Files.Keys)
            //    {
            //        var file = request.Files[key];
            //        fs.WriteFile(Url.Combine(selection.SelectedItem.Url, file.FileName), file.InputStream);
            //    }
            //}
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

        // Upload file to the server
        private void UploadTheFile(HttpContext context)
        {
            var statuses = new List<FilesStatus>();
            var headers = context.Request.Headers;

            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                UploadWholeFile(context, statuses);
            }
            else
            {
                UploadPartialFile(headers["X-File-Name"], context, statuses);
            }

            WriteJsonIframeSafe(context, statuses);
        }


        // Upload partial file
        private void UploadPartialFile(string fileName, HttpContext context, List<FilesStatus> statuses)
        {
            if (context.Request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
            var fileUpload = context.Request.Files[0];
            var inputStream = fileUpload.InputStream;

            var selection = new SelectionUtility(context.Request, N2.Context.Current);
            var fs = N2.Context.Current.Resolve<IFileSystem>();
            var virtualPath = Url.Combine(selection.SelectedItem.Url, fileName);

            using (var s = fs.OpenFile(virtualPath))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    s.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                s.Flush();
                s.Close();
            }
            statuses.Add(new FilesStatus(virtualPath, fileUpload.ContentLength));
        }

        // Upload entire file
        private void UploadWholeFile(HttpContext context, List<FilesStatus> statuses)
        {
            var fs = N2.Context.Current.Resolve<IFileSystem>();

            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                var file = context.Request.Files[i];
                var selection = new SelectionUtility(context.Request, N2.Context.Current);
                var fileName = Path.GetFileName(file.FileName);
                var virtualPath = Url.Combine(selection.SelectedItem.Url, fileName);

                fs.WriteFile(virtualPath, file.InputStream);
                statuses.Add(new FilesStatus(virtualPath, file.ContentLength));
            }
        }

        private void WriteJsonIframeSafe(HttpContext context, List<FilesStatus> statuses)
        {
            context.Response.AddHeader("Vary", "Accept");
            try
            {
                if (context.Request["HTTP_ACCEPT"].Contains("application/json"))
                    context.Response.ContentType = "application/json";
                else
                    context.Response.ContentType = "text/plain";
            }
            catch
            {
                context.Response.ContentType = "text/plain";
            }

            var json = statuses.ToJson();
            context.Response.Write(json);
        }

	}


    public class FilesStatus
    {
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string progress { get; set; }
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string error { get; set; }

        public FilesStatus(string fileName, int fileLength) { SetValues(fileName, fileLength); }

        private void SetValues(string fileName, int fileLength)
        {
            name = Url.GetName(fileName);
            type = "image/png";
            size = fileLength;
            progress = "1.0";
            url = "File.aspx?selected=" + fileName;
            thumbnail_url = "../Resize.ashx?w=32&h=32&img=" + fileName;
        }
    }
}
