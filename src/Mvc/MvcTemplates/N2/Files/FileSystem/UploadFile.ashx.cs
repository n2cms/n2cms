using System.Web;
using System.Linq;
using System.Web.Security;
using N2.Edit;
using N2.Edit.FileSystem;
using N2.Web;
using System.Collections.Generic;
using System.IO;
using N2.Definitions;
using System.Text.RegularExpressions;

namespace N2.Management.Files.FileSystem
{
	public class UploadFile : IHttpHandler
	{
        private Engine.IEngine engine;
        
        public UploadFile()
            : this(Context.Current)
        {
        }

        public UploadFile(Engine.IEngine engine)
        {
            this.engine = engine;
        }

		public void ProcessRequest(HttpContext context)
		{
			context.Response.AddHeader("Pragma", "no-cache");
			context.Response.AddHeader("Cache-Control", "private, no-cache");

			ValidateTicket(context.Request["ticket"]);

			SelectionUtility selection = new SelectionUtility(context, engine);
            var fs = engine.Resolve<IFileSystem>();

			List<FilesStatus> statuses;

			var headers = context.Request.Headers;
			if (string.IsNullOrEmpty(headers["X-File-Name"]))
			{
				statuses = UploadWholeFile(context, fs, selection).ToList();
			}
			else
			{
				statuses = UploadPartialFile(headers["X-File-Name"], context, fs, selection).ToList();
			}

			WriteJsonIframeSafe(context, statuses);
		}

		private bool IsFilenameTrusted(string fileName)
		{
            var uploadSection = engine.Config.Sections.Management.UploadFolders;
            return uploadSection.IsTrusted(fileName);
		}

		// Upload partial file
		private IEnumerable<FilesStatus> UploadPartialFile(string fileName, HttpContext context, IFileSystem fs, SelectionUtility selection)
		{
			if (context.Request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");

			var fileUpload = context.Request.Files[0];
			var inputStream = fileUpload.InputStream;
			var virtualPath = Url.Combine(selection.SelectedItem.Url, fileName);

			if (!IsFilenameTrusted(fileName))
			{
				yield return new FilesStatus(virtualPath, 0) { error = "Unsafe filename" };
			}
			else
			{

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
				yield return new FilesStatus(virtualPath, fileUpload.ContentLength);
			}
		}

		// Upload entire file
		private IEnumerable<FilesStatus> UploadWholeFile(HttpContext context, IFileSystem fs, SelectionUtility selection)
		{
			for (int i = 0; i < context.Request.Files.Count; i++)
			{
				var file = context.Request.Files[i];
				var fileName = Path.GetFileName(file.FileName);
				var virtualPath = Url.Combine(((IFileSystemNode)selection.SelectedItem).LocalUrl, fileName);

				if (!IsFilenameTrusted(fileName))
				{
					yield return new FilesStatus(virtualPath, 0) { error = "Unsafe filename" };
				}
				else
				{
					fs.WriteFile(virtualPath, file.InputStream);
					yield return new FilesStatus(virtualPath, file.ContentLength);
				}
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

		private static void ValidateTicket(string encryptedTicket)
		{
			var ticket = FormsAuthentication.Decrypt(encryptedTicket);
			if (ticket.Expired)
				throw new Security.PermissionDeniedException("Upload ticket expired");
			if (!ticket.Name.StartsWith("SecureUpload-"))
                throw new Security.PermissionDeniedException("Unknown ticket");
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
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
			thumbnail_url = "../Resize.ashx?w=48&h=48&img=" + fileName;
		}
	}
}
