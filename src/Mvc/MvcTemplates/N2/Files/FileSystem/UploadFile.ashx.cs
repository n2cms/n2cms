using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Edit.FileSystem;
using N2.Web;
using N2.Edit;
using System.Web.Security;

namespace N2.Management.Files.FileSystem
{
	public class UploadFile : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			var request = context.Request;

			var ticket = FormsAuthentication.Decrypt(request["ticket"]);
			if (ticket.Expired)
				throw new N2.N2Exception("Upload ticket expired");
			if(!ticket.Name.StartsWith("SecureUpload-"))
				throw new N2.N2Exception("Unknown ticket");

			SelectionUtility selection = new SelectionUtility(request, N2.Context.Current);

			if (request.Files.Count > 0)
			{
				var fs = N2.Context.Current.Resolve<IFileSystem>();
				foreach (string key in request.Files.Keys)
				{
					var file = request.Files[key];
					fs.WriteFile(Url.Combine(selection.SelectedItem.Url, file.FileName), file.InputStream);
				}
			}
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}
