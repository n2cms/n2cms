using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Diagnostics;

namespace N2.Templates.UI
{
	public class Global : System.Web.HttpApplication
	{

		protected void Application_Start(object sender, EventArgs e)
		{
			Debug.Write("start");
		}

		protected void Application_End(object sender, EventArgs e)
		{

		}
	}
}