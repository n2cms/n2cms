using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using N2.Edit.Installation;
using N2.Engine;
using N2.Web;
using N2.Definitions.Static;
using N2.Persistence;
using N2.Persistence.Serialization;
using N2.Persistence.NH;
using N2.Plugin;
using N2.Configuration;
using N2.Details;
using N2.Security;

namespace N2.Raven
{
	[Service(typeof(InstallationManager), Replaces = typeof(InstallationManager))]
	public class RavenInstallationManager : InstallationManager
	{
		private RavenConnectionProvider cp;

		public RavenInstallationManager(IHost host, DefinitionMap map, ContentActivator activator, Importer importer, IPersister persister, ISessionProvider sessionProvider, IConfigurationBuilder configurationBuilder, IWebContext webContext, ConnectionMonitor connectionContext, DatabaseSection config, RavenConnectionProvider cp)
			: base(connectionContext, importer, webContext, persister, activator)
		{
			this.cp = cp;
		}

		public Exception GetConnectionException()
		{
			//return base.GetConnectionException();
			try
			{
				using (cp.OpenSession())
				{
					return null;
				}
			}
			catch (Exception ex)
			{
				return ex;
			}
		}

		protected void UpdateConnection(DatabaseStatus status)
		{
			//base.UpdateConnection(status);
			if (GetConnectionException() == null)
			{
				status.IsConnected = true;
			}
		}

		protected void UpdateVersion(DatabaseStatus status)
		{
			//base.UpdateVersion(status);
			status.DatabaseVersion = DatabaseStatus.RequiredDatabaseVersion;
		}

		protected void UpdateSchema(DatabaseStatus status)
		{
			//base.UpdateSchema(status);

			status.HasSchema = true;
		}

		protected void UpdateCount(DatabaseStatus status)
		{
			try
			{
				status.Items = cp.Session.Query<ContentItem>().Count();
				status.Details = cp.Session.Query<ContentDetail>().Count();
				status.DetailCollections = cp.Session.Query<DetailCollection>().Count();
				status.AuthorizedRoles = cp.Session.Query<AuthorizedRole>().Count();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public override string CheckConnection(out string stackTrace)
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}

		public override void ExportSchema(TextWriter output)
		{
			throw new NotImplementedException();
		}

		public override string ExportUpgradeSchema()
		{
			throw new NotImplementedException();
		}
	}
}
