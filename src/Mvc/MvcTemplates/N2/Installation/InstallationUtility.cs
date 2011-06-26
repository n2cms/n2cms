﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using N2.Web;

namespace N2.Management.Installation
{
	public static class InstallationUtility
	{
		public static string InstallationUnallowedHtml = @"<h1>Installation not allowed</h1>
<p>Your configuration specifies that installation isn't allowed. To allow installation modify web.config:</p>
<code><pre>&lt;n2&gt;&lt;edit&gt;&lt;installer allowInstallation=""<strong>true</strong>""/&gt;</pre></code>";

		public static void CheckInstallationAllowed(HttpContext context)
		{
			if(N2.Context.Current.Resolve<N2.Configuration.EditSection>().Installer.AllowInstallation)
				return;

			context.Response.Write("<html><head>");
			context.Response.Write("<link rel='stylesheet' type='text/css' href='" + Url.ResolveTokens("{ManagementUrl}/Resources/Css/all.css") + "' />");
			context.Response.Write("<link rel='stylesheet' type='text/css' href='" + Url.ResolveTokens("{ManagementUrl}/Resources/Css/framed.css") + "' />");
			context.Response.Write("<link rel='stylesheet' type='text/css' href='" + Url.ResolveTokens("{ManagementUrl}/Resources/Css/themes/default.css") + "' />");
			context.Response.Write("</head><body><div class='tabPanel' style='font-size:1.1em;width:800px;margin:10px auto;'>");
			context.Response.Write(InstallationUnallowedHtml);
			context.Response.Write("</div></body></html>");
			context.Response.End();
		}
	}
}