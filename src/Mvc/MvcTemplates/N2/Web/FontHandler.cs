using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace N2.Management.Web
{
	public class FontHandler : IHttpHandler
	{
		public bool IsReusable
		{
			get { return true; }
		}

		public void ProcessRequest(HttpContext context)
		{
			switch (VirtualPathUtility.GetExtension(context.Request.Url.AbsolutePath))
			{
				case ".eot":
					Transmit(context, "application/x-eot");
					return;
				case ".svg":
					Transmit(context, "image/svg+xml");
					return;
				case ".ttf":
					Transmit(context, "application/x-ttf");
					return;
				case ".woff":
					Transmit(context, "application/x-woff");
					return;
				case ".otf":
					Transmit(context, "application/x-otf");
					return;
			}
		}

		private void Transmit(HttpContext context, string contentType)
		{
			if (!File.Exists(context.Request.PhysicalPath))
				return;

			context.Response.ContentType = contentType;
			context.Response.TransmitFile(context.Request.PhysicalPath);
		}
	}
}