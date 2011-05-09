using System.IO;
using System.Web;

namespace N2.Web.Hosting
{
	public class VirtualPathFileHandler : IHttpHandler
	{
		#region IHttpHandler Members

		public bool IsReusable
		{
			get { return true; }
		}

		public void ProcessRequest(HttpContext context)
		{
			if (File.Exists(context.Request.PhysicalPath))
			{
				context.Response.TransmitFile(context.Request.PhysicalPath);
			}
			else if (System.Web.Hosting.HostingEnvironment.VirtualPathProvider.FileExists(context.Request.AppRelativeCurrentExecutionFilePath))
			{
				string contentType = GetContentType(context.Request.AppRelativeCurrentExecutionFilePath);
				if (contentType != null)
					context.Response.ContentType = contentType;

				var f = System.Web.Hosting.HostingEnvironment.VirtualPathProvider.GetFile(context.Request.AppRelativeCurrentExecutionFilePath);
				using (var s = f.Open())
				{
					TransferBetweenStreams(s, context.Response.OutputStream);
				}
			}
		}

		private static string GetContentType(string filename)
		{
			var extension = Path.GetExtension(filename).ToLower();
			switch (extension)
			{
				case ".gif":
					return "image/gif";
				case ".png":
					return "image/png";
				case ".jpg":
				case ".jpeg":
					return "image/jpeg";
				case ".css":
					return "text/css";
				case ".js":
					return "application/x-javascript";
				case ".htm":
				case ".html":
					return "text/html";
				case ".swf":
					return "application/x-shockwave-flash";
				default:
					return null;
			}
		}

		protected virtual void TransferBetweenStreams(Stream inputStream, Stream outputStream)
		{
			byte[] buffer = new byte[32768];
			while (true)
			{
				int bytesRead = inputStream.Read(buffer, 0, buffer.Length);
				if (bytesRead <= 0)
					break;

				outputStream.Write(buffer, 0, bytesRead);
			}
		}

		#endregion
	}
}
