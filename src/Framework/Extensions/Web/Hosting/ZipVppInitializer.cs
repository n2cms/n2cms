using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Hosting;
using N2.Engine;
using N2.Plugin;

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
			var staticFileExtensions = configFactory.Sections.Web.Vpp.Zips.StaticFileExtensions.OfType<string>().ToList();
			foreach (var vppElement in configFactory.Sections.Web.Vpp.Zips.AllElements)
			{
				string filePath = vppElement.FilePath;
				string observedPath = vppElement.ObservedPath;
				string path = MapPath(filePath);
				if (!File.Exists(path))
				{
					Trace.TraceWarning("Did not find configured (" + vppElement.Name + ") zip vpp on disk: " + path);
					continue;
				}
				DateTime lastModified = File.GetLastWriteTimeUtc(path);

				var vpp = new Ionic.Zip.Web.VirtualPathProvider.ZipFileVirtualPathProvider(path);
				Register(vpp);

				broker.PostResolveAnyRequestCache += (s, a) =>
				{
					var application = s as HttpApplication;
					var context = application.Context;
					var requestPath = context.Request.AppRelativeCurrentExecutionFilePath;

					if (!requestPath.StartsWith(observedPath, StringComparison.InvariantCultureIgnoreCase))
						return;
					if (!vpp.DirectoryExists(VirtualPathUtility.GetDirectory(requestPath)))
						return;

					string extension = VirtualPathUtility.GetExtension(requestPath).ToLower();
					if (extension == "")
						context.RewritePath(requestPath.TrimEnd('/') + "/Default.aspx");
					else if (staticFileExtensions.Contains(extension) && vpp.FileExists(requestPath))
						context.RemapHandler(new VirtualPathFileHandler() { Modified = lastModified });
				};
			}

		}

		public void Stop()
		{
		}

		#endregion
	}
}
