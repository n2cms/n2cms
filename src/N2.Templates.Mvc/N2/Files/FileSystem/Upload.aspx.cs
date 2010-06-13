using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.Configuration;
using N2.Edit.Web;
using N2.Edit.FileSystem;
using N2.Web;

namespace N2.Management.Files.FileSystem
{
	public partial class Upload : EditPage
	{
		protected int maxFileSize = 4096 * 1024 - 10000;

		protected void Page_Load(object sender, EventArgs e)
		{
			hlCancel.NavigateUrl = Selection.ActionUrl("info");

			var config = ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection;
			maxFileSize = config.MaxRequestLength * 1024 - 10000;

		}
	}
}
