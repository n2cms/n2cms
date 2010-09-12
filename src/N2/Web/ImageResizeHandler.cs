using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using N2.Edit.FileSystem;
using System.IO;

namespace N2.Web
{
	/// <summary>
	/// Resizes the requested image to the requested size.
	/// </summary>
	public class ImageResizeHandler : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			//UrlDecode is neccessary to compensate encoding in ImageAdapter
			string imageUrl = HttpUtility.UrlDecode(context.Request["img"]);
			string w = context.Request["w"];
			string h = context.Request["h"];
			string m = context.Request["m"];

			double width = 0;
			double.TryParse(w, out width);

			double height = 0;
			double.TryParse(h, out height);

			ImageResizeMode mode;
			switch (m)
			{
				case "Fill":
					mode = ImageResizeMode.Fill;
					break;
				case "Stretch":
					mode = ImageResizeMode.Stretch;
					break;
				default:
					mode = ImageResizeMode.Fit;
					break;
			}

			IFileSystem fs = N2.Context.Current.Resolve<IFileSystem>();
			if (fs.FileExists(imageUrl))
			{
				string path = context.Server.MapPath(imageUrl);
				if (CacheUtility.IsModifiedSince(context.Request, path))
				{
					CacheUtility.NotModified(context.Response);
				}

				context.Response.ContentType = "image/jpeg";

				string extension = VirtualPathUtility.GetExtension(imageUrl);
				ImageResizer ir = new ImageResizer();

				using (var s = fs.OpenFile(imageUrl))
				{
					ir.Resize(s, extension, width, height, mode, context.Response.OutputStream);
				}

				CacheUtility.SetValidUntilExpires(context.Response, TimeSpan.FromDays(7));
			}
			else
			{
				throw new HttpException(404, "Not found");
			}
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}
