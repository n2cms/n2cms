using System.IO;
using System.Web;
using System;
using System.Web.Hosting;

namespace N2.Web.Hosting
{
	public class VirtualPathFileHandler : IHttpHandler
	{
		VirtualPathProvider vpp;

		public VirtualPathFileHandler()
		{
			vpp = HostingEnvironment.VirtualPathProvider;
		}

		public VirtualPathFileHandler(VirtualPathProvider vpp)
		{
			this.vpp = vpp;
		}

		public DateTime? Modified { get; set; }

		#region IHttpHandler Members

		public bool IsReusable
		{
			get { return true; }
		}

		public void ProcessRequest(HttpContext context)
		{
			if (File.Exists(context.Request.PhysicalPath))
			{
				var fileModified = File.GetLastWriteTimeUtc(context.Request.PhysicalPath);
				if(CacheUtility.IsUnmodifiedSince(context.Request, fileModified))
					CacheUtility.NotModified(context.Response);

				N2.Web.CacheUtility.SetValidUntilExpires(context.Response, DateTime.UtcNow);
				context.Response.TransmitFile(context.Request.PhysicalPath);
			}
			else if (vpp.FileExists(context.Request.AppRelativeCurrentExecutionFilePath))
			{
				if (Modified.HasValue && CacheUtility.IsUnmodifiedSince(context.Request, Modified.Value))
					CacheUtility.NotModified(context.Response);

				string contentType = GetContentType(context.Request.AppRelativeCurrentExecutionFilePath);
				if (contentType != null)
					context.Response.ContentType = contentType;

				N2.Web.CacheUtility.SetValidUntilExpires(context.Response, DateTime.UtcNow);

				var f = vpp.GetFile(context.Request.AppRelativeCurrentExecutionFilePath);
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
