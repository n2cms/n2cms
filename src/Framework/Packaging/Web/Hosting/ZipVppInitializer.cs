using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Hosting;
using N2.Engine;
using N2.Plugin;
using System.Collections.Generic;

namespace N2.Web.Hosting
{
    [Service]
    public class ZipVppInitializer : IAutoStart
    {
        private static readonly Engine.Logger<ZipVppInitializer> logger;
        private Configuration.ConfigurationManagerWrapper configFactory;
        private EventBroker broker;

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
            var staticFileExtensions = new HashSet<string>(configFactory.Sections.Web.Vpp.Zips.StaticFileExtensions.OfType<string>());
            foreach (var vppElement in configFactory.Sections.Web.Vpp.Zips.AllElements)
            {
                string filePath = vppElement.FilePath;
                string observedPath = vppElement.ObservedPath;
                string path = MapPath(filePath);
                if (!File.Exists(path))
                {
                    N2.Engine.Logger.Warn("Did not find configured (" + vppElement.Name + ") zip vpp on disk: " + path);
                    continue;
                }
                DateTime lastModified = File.GetLastWriteTimeUtc(path);

                var vpp = new SharpZipLib.Web.VirtualPathProvider.ZipFileVirtualPathProvider(path);
                N2.Engine.Logger.Info("Registering VPP: " + vpp);
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
                        requestPath = requestPath.TrimEnd('/') + "/Default.aspx"; //context.RewritePath(requestPath.TrimEnd('/') + "/Default.aspx");

                    // There's a problem with RouteTable.Routes keeping storing the default vpp before we register our vpp, in which case
                    // RouteExistingFiles will not handle files in the zip vpp. This is a workaround.
                    switch (extension)
                    {
                        case "":
                        case ".aspx":
                        case ".axd":
                        case ".ashx":
                            if (vpp.FileExists(requestPath))
                            {
                                logger.DebugFormat("Remapping handler for path {0}", requestPath);
                                var handler = System.Web.Compilation.BuildManager.CreateInstanceFromVirtualPath(requestPath, typeof(IHttpHandler));
                                context.RemapHandler(handler as IHttpHandler);
                            }
                            else
                                logger.DebugFormat("Not found virtual path path {0}", requestPath);
                            break;
                        case ".config":
                            break;
                        default:
                            if (staticFileExtensions.Contains(extension) && vpp.FileExists(requestPath))
                            {
                                logger.DebugFormat("Remapping to virtual path file handler {0}", requestPath);
                                context.RemapHandler(new VirtualPathFileHandler(vpp) { Modified = lastModified });
                            }
                            break;
                    }
                };
            }

        }

        public void Stop()
        {
        }

        #endregion
    }
}
