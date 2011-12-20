using System;
using System.Collections.Generic;
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

namespace N2.RavenDB
{
	[Service(typeof(InstallationManager), Replaces = typeof(InstallationManager))]
	public class RavenInstallationManager : InstallationManager
	{
		private RavenConnectionProvider cp;

		public RavenInstallationManager(IHost host, DefinitionMap map, ContentActivator activator, Importer importer, IPersister persister, ISessionProvider sessionProvider, IConfigurationBuilder configurationBuilder, IWebContext webContext, ConnectionMonitor connectionContext, DatabaseSection config, N2.RavenDB.RavenConnectionProvider cp)
			: base(host, map, activator, importer, persister, sessionProvider, configurationBuilder, webContext, connectionContext, config)
		{
			this.cp = cp;
		}

		public override Exception GetConnectionException()
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

		protected override void UpdateConnection(DatabaseStatus status)
		{
			//base.UpdateConnection(status);
			if (GetConnectionException() == null)
			{
				status.IsConnected = true;
			}
		}

		protected override void UpdateVersion(DatabaseStatus status)
		{
			//base.UpdateVersion(status);
			status.DatabaseVersion = DatabaseStatus.RequiredDatabaseVersion;
		}

		protected override void UpdateSchema(DatabaseStatus status)
		{
			//base.UpdateSchema(status);

			status.HasSchema = true;
		}

		protected override void UpdateCount(DatabaseStatus status)
		{
			try
			{
				status.Items = cp.Session.Query<ContentItem>().Count();
				status.Details = cp.Session.Query<ContentDetail>().Count();
				status.DetailCollections = cp.Session.Query<DetailCollection>().Count();
				status.AuthorizedRoles = cp.Session.Query<AuthorizedROle>().Count();
			}
			catch (Exception)
			{
				throw;
			}

		}
	}
}
