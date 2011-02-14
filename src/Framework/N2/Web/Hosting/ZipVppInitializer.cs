using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using System.Web.Hosting;
using System.Web;
using N2.Plugin;
using System.IO;
using System.Diagnostics;

namespace N2.Web.Hosting
{
	[Service]
	public class ZipVppInitializer : IAutoStart
	{
		Configuration.ConfigurationManagerWrapper configFactory;
		EventBroker broker;

		protected Func<string, string> MapPath = HostingEnvironment.MapPath;
		protected Action<VirtualPathProvider> Register = HostingEnvironment.RegisterVirtualPathProvider;

		public ZipVppInitializer(Configuration.ConfigurationManagerWrapper configFactory, EventBroker broker)
		{
			this.configFactory = configFactory;
			this.broker = broker;
		}

		#region IAutoStart Members

		public void Start()
		{
			foreach (var vppElement in configFactory.Sections.Web.Vpp.Zips.AllElements)
			{
				string virtualPath = vppElement.VirtualPath;
				string zipPath = MapPath(virtualPath);
				if (!File.Exists(zipPath))
				{
					Trace.TraceWarning("Did not find configured (" + vppElement.Name + ") zip vpp on disk: " + zipPath);
					continue;
				}

				var vpp = new Ionic.Zip.Web.VirtualPathProvider.ZipFileVirtualPathProvider(zipPath);
				Register(vpp);

				broker.BeginRequest += (s, a) =>
				{
					var app = s as HttpApplication;
					var ctx = app.Context;
					var requestPath = ctx.Request.AppRelativeCurrentExecutionFilePath;

					if (!requestPath.StartsWith(virtualPath, StringComparison.InvariantCultureIgnoreCase))
						return;
					if (!vpp.DirectoryExists(requestPath))
						return;

					ctx.RewritePath(requestPath.TrimEnd('/') + "/Default.aspx");
				};
			}

		}

		public void Stop()
		{
		}

		#endregion
	}
}
