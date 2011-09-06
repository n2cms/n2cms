using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Web;
using N2.Plugin;
using System.Web;

namespace N2.Edit.FileSystem.NH
{
	[Service(Configuration = "dbfs")]
	public class FileSystemHandlerFactory : IAutoStart
	{
		private readonly IFileSystem fs;
		private readonly IEditManager editManager;
		private readonly EventBroker broker;
		private readonly IWebContext requestContext;

		public FileSystemHandlerFactory(IFileSystem fs, IEditManager editManager, IWebContext requestContext, EventBroker broker)
		{
			this.fs = fs;
			this.editManager = editManager;
			this.requestContext = requestContext;
			this.broker = broker;
		}

		void Broker_PostResolveRequestCache(object sender, EventArgs args)
		{
			var path = requestContext.Request.AppRelativeCurrentExecutionFilePath;

			foreach (var uploadFolder in this.editManager.UploadFolders)
			{
				if (path.StartsWith(uploadFolder, StringComparison.InvariantCultureIgnoreCase))
				{
					if (fs.FileExists(path))
					{
						new UploadFileHttpHandler(fs).ProcessRequest(HttpContext.Current);
						HttpContext.Current.ApplicationInstance.CompleteRequest();
					}
				}
			}
		}

		#region IAutoStart Members

		public void Start()
		{
			broker.PostMapRequestHandler += Broker_PostResolveRequestCache;
		}

		public void Stop()
		{
			broker.PostMapRequestHandler -= Broker_PostResolveRequestCache;
		}

		#endregion
	}
}
