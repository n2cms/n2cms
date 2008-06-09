using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Diagnostics;
using N2.Web;
using System.Configuration;

namespace N2.Installation
{
	public class InstallerModule : IHttpModule
	{
		HttpApplication context;
		bool isChecked = false;

		public void Init(HttpApplication context)
		{
			this.context = context;
			N2.Context.Initialize(false);
			context.BeginRequest += context_BeginRequest;
		}

		void context_BeginRequest(object sender, EventArgs e)
		{
			if (isChecked || IsEditing() || !AllowRedirectToInstallPage())
				return;
			else
				isChecked = true;

			context.BeginRequest -= context_BeginRequest;

			InstallationManager im = new InstallationManager(N2.Context.Current);
			
			DatabaseStatus status = im.GetStatus();
			if (!status.IsInstalled)
			{
				string url = ConfigurationManager.AppSettings["N2.InstallUrl"] ?? "~/Edit/Install/Begin/Default.aspx";
				context.Response.Redirect(url);
			}
		}

		private static bool AllowRedirectToInstallPage()
		{
			return string.Equals(ConfigurationManager.AppSettings["N2.AllowRedirectToInstallPage"], "true", StringComparison.InvariantCultureIgnoreCase);
		}

		private static bool IsEditing()
		{
			IWebContext web = N2.Context.Current.Resolve<IWebContext>();
			return web.ToAppRelative(web.AbsolutePath).StartsWith("~/Edit", StringComparison.InvariantCultureIgnoreCase);
		}

		public void Dispose()
		{
		}
	}
}
