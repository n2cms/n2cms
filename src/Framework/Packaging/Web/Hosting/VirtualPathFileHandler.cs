using System.IO;
using System.Web;
using System;
using System.Web.Hosting;

namespace N2.Web.Hosting
{
	public class VirtualPathFileHandler : IHttpHandler
	{
		private static readonly Engine.Logger<VirtualPathFileHandler> logger;

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

				logger.DebugFormat("Transmitting virtual file {0} available on disk {1}", context.Request.AppRelativeCurrentExecutionFilePath, context.Request.PhysicalPath);
				N2.Web.CacheUtility.SetValidUntilExpires(context.Response, DateTime.UtcNow);
				context.Response.ContentType = GetContentType(context.Request.PhysicalPath);
				context.Response.TransmitFile(context.Request.PhysicalPath);
			}
			else if (vpp.FileExists(context.Request.AppRelativeCurrentExecutionFilePath))
			{
				if (Modified.HasValue && CacheUtility.IsUnmodifiedSince(context.Request, Modified.Value))
				{
					logger.DebugFormat("Not modified: {0}", context.Request.AppRelativeCurrentExecutionFilePath);
					CacheUtility.NotModified(context.Response);
				}

				byte[] cached = context.Cache["VirtualPathFileHandler:" + context.Request.AppRelativeCurrentExecutionFilePath] as byte[];
				if (cached != null)
				{
					logger.DebugFormat("Transmitting cached file: {0}", context.Request.AppRelativeCurrentExecutionFilePath);
					context.Response.ContentType = GetContentType(context.Request.AppRelativeCurrentExecutionFilePath);
					context.Response.OutputStream.Write(cached, 0, cached.Length);
					return;
				}
				

				var f = vpp.GetFile(context.Request.AppRelativeCurrentExecutionFilePath);
				using (var s = f.Open())
				{
					byte[] buffer = new byte[131072];
					int readBytes = ReadBlock(s, buffer);
					if (readBytes <= 0)
					{
						logger.DebugFormat("Empty file: {0}", context.Request.AppRelativeCurrentExecutionFilePath);
						return;
					}

					N2.Web.CacheUtility.SetValidUntilExpires(context.Response, DateTime.UtcNow);
					context.Response.ContentType = GetContentType(context.Request.AppRelativeCurrentExecutionFilePath);
					logger.DebugFormat("Writing file: {0}", context.Request.AppRelativeCurrentExecutionFilePath);
					context.Response.OutputStream.Write(buffer, 0, readBytes);

					if (readBytes < buffer.Length)
					{
						cached = new byte[readBytes];
						Array.Copy(buffer, cached, readBytes);
						logger.DebugFormat("Adding to cache: {0}", context.Request.AppRelativeCurrentExecutionFilePath);
						context.Cache.Add("VirtualPathFileHandler:" + context.Request.AppRelativeCurrentExecutionFilePath, cached, vpp.GetCacheDependency(context.Request.AppRelativeCurrentExecutionFilePath, new[] { context.Request.AppRelativeCurrentExecutionFilePath }, DateTime.UtcNow), DateTime.MaxValue, TimeSpan.FromMinutes(1), System.Web.Caching.CacheItemPriority.BelowNormal, null);
						return;
					}
					logger.DebugFormat("Transmitting rest of file {0}", context.Request.AppRelativeCurrentExecutionFilePath);
					TransferBetweenStreams(buffer, s, context.Response.OutputStream);
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
				case ".txt":
					return "text/txt";
				case ".swf":
					return "application/x-shockwave-flash";
				default:
					return "application/data";
			}
		}

		protected virtual void TransferBetweenStreams(byte[] buffer, Stream inputStream, Stream outputStream)
		{
			while (true)
			{
				int bytesRead = ReadBlock(inputStream, buffer);
				if (bytesRead <= 0)
					break;

				outputStream.Write(buffer, 0, bytesRead);
			}
		}

		private int ReadBlock(Stream inputStream, byte[] buffer)
		{
			return inputStream.Read(buffer, 0, buffer.Length);
		}

		#endregion
	}
}
