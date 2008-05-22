using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Diagnostics;

namespace N2.Installation
{
	public class InstallerModule : IHttpModule
	{
		public void Dispose()
		{
		}

		static bool alreadyChecked = false;
		public void Init(HttpApplication context)
		{
			if (!alreadyChecked)
			{
				alreadyChecked = true;

				Context.Initialize(false);
				InstallationManager im = new InstallationManager(Context.Current);
				DatabaseStatus status = im.GetStatus();
				if (!status.IsInstalled)
				{
					string message = "There seems to be a problem with the configuration and/or database. Please check the web configuration and navigate to the installer located at /edit/install. Available information: " 
						+ status.ToStatusString();
					throw new N2Exception(message);
				}
			}
		}
	}
}
