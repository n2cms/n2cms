using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.IO;
using System.Collections.Generic;

namespace N2.Edit.Js
{
	public abstract class DirectoryCompiler : IHttpHandler
	{
		public abstract string FolderUrl { get; }

		protected virtual TimeSpan CacheExpiration
		{
			get { return TimeSpan.FromHours(12); }
		}

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "text/javascript";
			context.Response.Buffer = false;
			SetCache(context);

			foreach (string file in GetFiles(context))
			{
#if DEBUG
				context.Response.Write(Environment.NewLine + "////// " + Path.GetFileName(file) + Environment.NewLine + Environment.NewLine);
#endif

				context.Response.TransmitFile(file);
			}
		}

		protected virtual IEnumerable<string> GetFiles(HttpContext context)
		{
			string dir = context.Server.MapPath(FolderUrl);
			return Directory.GetFiles(dir);
		}

		protected virtual void SetCache(HttpContext context)
		{
			context.Response.Cache.SetExpires(DateTime.Now.Add(CacheExpiration));
			context.Response.Cache.SetCacheability(HttpCacheability.Public);
			context.Response.Cache.SetValidUntilExpires(false);
			context.Response.Cache.VaryByHeaders["Accept-Encoding"] = true;
			context.Response.Cache.VaryByParams["*"] = true;
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}
