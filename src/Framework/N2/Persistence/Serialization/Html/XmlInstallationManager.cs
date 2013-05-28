using N2.Edit.Installation;
using N2.Edit.Versioning;
using N2.Engine;
using N2.Persistence.Serialization;
using N2.Plugin;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Serialization.Xml
{
	[Service]
	[Service(typeof(InstallationManager), Configuration = "xml", Replaces = typeof(NHInstallationManager))]
	public class XmlInstallationManager : InstallationManager
	{
		Logger<XmlInstallationManager> logger;
		private IHost host;
		private IPersister persister;

		public XmlInstallationManager(IHost host, IPersister persister, ConnectionMonitor connectionContext, 
				Importer importer, IWebContext webContext, ContentActivator activator)
			: base(connectionContext, importer, webContext, persister, activator)
		{
			this.host = host;
			this.persister = persister;
		}

		public override string CheckConnection(out string stackTrace)
		{
			stackTrace = null;
			var status = GetStatus();
			return status.ToConnectionStatusString();
		}

		public override string CheckDatabase()
		{
			throw new NotImplementedException();
		}

		public override string CheckRootItem()
		{
			throw new NotImplementedException();
		}

		public override string CheckStartPage()
		{
			throw new NotImplementedException();
		}

		public override DatabaseStatus GetStatus()
		{
			logger.Debug("checking database status");

			var status = new DatabaseStatus();

			if (UpdateConnection(status))
				if (UpdateCount(status))
					if (UpdateItems(status))
						UpdateAppPath(status);

			return status;
		}

		private void UpdateAppPath(DatabaseStatus status)
		{
			try
			{
				if (status.RootItem == null)
					return;

				status.AppPath = status.RootItem[InstallationAppPath] as string;
				status.NeedsRebase = !string.IsNullOrEmpty(status.AppPath) && !string.Equals(status.AppPath, N2.Web.Url.ToAbsolute("~/"));
			}
			catch (Exception ex)
			{
				logger.Warn(ex);
				status.ItemsError = ex.Message;
			}
		}

		private bool UpdateItems(DatabaseStatus status)
		{
			try
			{
				status.StartPageID = host.DefaultSite.StartPageID;
				status.RootItemID = host.DefaultSite.RootItemID;
				status.StartPage = persister.Get(status.StartPageID);
				status.RootItem = persister.Get(status.RootItemID);
				status.IsInstalled = status.RootItem != null && status.StartPage != null;
				status.HasSchema = true;
				status.DatabaseVersion = DatabaseStatus.RequiredDatabaseVersion;
				return true;
			}
			catch (Exception ex)
			{
				logger.Warn(ex);

				status.HasSchema = false;
				status.IsInstalled = false;
				status.ItemsError = ex.Message;
				return false;
			}
		}

		private bool UpdateCount(DatabaseStatus status)
		{
			try
			{
				status.Items = 0; //TODO: XML: Get # of content items
				status.Details = 0;
				status.DetailCollections = 0;
				status.AuthorizedRoles = 0;
				status.Versions = 0; //TODO: XML: Get # of versions
				return true;
			}
			catch (Exception ex)
			{
				logger.Warn(ex);
				return false;
			}
		}

		private bool UpdateConnection(DatabaseStatus status)
		{
			status.IsConnected = true;
			return true;
		}

		public override void Upgrade()
		{
			throw new NotImplementedException();
		}

		public override void Install()
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<ContentItem> ExecuteQuery(string query)
		{
			throw new NotImplementedException();
		}

		public override string ExportSchema()
		{
			return "[No schema required for XML db]";
		}

		public override void ExportSchema(System.IO.TextWriter output)
		{
			throw new NotImplementedException();
		}

		public override string ExportUpgradeSchema()
		{
			throw new NotImplementedException();
		}
	}
}
