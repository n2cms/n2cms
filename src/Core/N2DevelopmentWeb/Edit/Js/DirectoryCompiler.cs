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

namespace N2.Edit.Js
{
	public abstract class DirectoryCompiler : IHttpHandler
	{
		public abstract string FolderUrl { get; }

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "text/javascript";
			string dir = context.Server.MapPath(FolderUrl);
			foreach (string file in Directory.GetFiles(dir))
			{
#if DEBUG
				context.Response.Write(Environment.NewLine + "////// " + Path.GetFileName(file) + Environment.NewLine + Environment.NewLine);
#endif

				context.Response.TransmitFile(file);
			}
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}
