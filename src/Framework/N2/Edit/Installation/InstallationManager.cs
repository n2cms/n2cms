using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using N2.Definitions;
using N2.Definitions.Static;
using N2.Engine;
using N2.Installation;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Persistence.Serialization;
using N2.Web;
using NHibernate;
using NHibernate.Driver;
using NHibernate.SqlTypes;
using NHibernate.Tool.hbm2ddl;
using Environment = NHibernate.Cfg.Environment;
using N2.Plugin;
using N2.Configuration;
using N2.Details;
using System.Web;

namespace N2.Edit.Installation
{
	/// <summary>
	/// Wraps functionality to request database status and generate n2's 
	/// database schema on multiple database flavours.
	/// </summary>
	[Service]
	public class InstallationManager
	{
		public const string InstallationAppPath = "Installation.AppPath";
		public const string installationHost = "Installation.Host";
		private readonly Engine.Logger<InstallationManager> logger;

		IConfigurationBuilder configurationBuilder;
		ContentActivator activator;
		Importer importer;
		IPersister persister;
		ISessionProvider sessionProvider;
		IHost host;
		IWebContext webContext;
		DefinitionMap map;
		ConnectionMonitor connectionContext;
		bool isDatabaseFileSystemEnbled = false;

		public InstallationManager(IHost host, DefinitionMap map, ContentActivator activator, Importer importer, IPersister persister, ISessionProvider sessionProvider, IConfigurationBuilder configurationBuilder, IWebContext webContext, ConnectionMonitor connectionContext, DatabaseSection config)
		{
			this.host = host;
			this.map = map;
			this.activator = activator;
			this.importer = importer;
			this.persister = persister;
			this.sessionProvider = sessionProvider;
			this.configurationBuilder = configurationBuilder;
			this.webContext = webContext;
			this.connectionContext = connectionContext;
			this.isDatabaseFileSystemEnbled = config.Files.StorageLocation == FileStoreLocation.Database;
		}

		NHibernate.Cfg.Configuration cfg;
		protected NHibernate.Cfg.Configuration Cfg
		{
			get { return cfg ?? (cfg = configurationBuilder.BuildConfiguration()); }
		}

		public static string GetResourceString(string resourceKey)
		{
			Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceKey);
			StreamReader sr = new StreamReader(s);
			return sr.ReadToEnd();
		}

		const bool ConsoleOutput = false;
		const bool DatabaseExport = true;
		const bool NoDatabaseExport = false;

		/// <summary>Executes sql create database scripts.</summary>
		public void Install()
		{
			var sf = configurationBuilder.BuildSessionFactory();
			sf.EvictQueries();
			foreach (var collectionMetadata in sf.GetAllCollectionMetadata())
				sf.EvictCollection(collectionMetadata.Key);
			foreach (var classMetadata in sf.GetAllClassMetadata())
				sf.EvictEntity(classMetadata.Key);

			SchemaExport exporter = new SchemaExport(Cfg);
			exporter.Create(ConsoleOutput, DatabaseExport);
		}

		public void ExportSchema(TextWriter output)
		{
			SchemaExport exporter = new SchemaExport(Cfg);
			exporter.Execute(ConsoleOutput, NoDatabaseExport, false, null, output);
		}

		public string ExportSchema()
		{
			StringBuilder sql = new StringBuilder();
			using (StringWriter sw = new StringWriter(sql))
			{
				ExportSchema(sw);
			}
			return sql.ToString();
		}



		public void Upgrade()
		{
			SchemaUpdate schemaUpdater = new SchemaUpdate(Cfg);
			schemaUpdater.Execute(true, true);
		}

		public string ExportUpgradeSchema()
		{
			SchemaUpdate schemaUpdater = new SchemaUpdate(Cfg);

			StringBuilder sql = new StringBuilder();
			using (StringWriter sw = new StringWriter(sql))
			{
				schemaUpdater.Execute((s) =>
				{
					sql.Append(s);
				}, false);
			}
			return sql.ToString();
		}

		public void DropDatabaseTables()
		{
			SchemaExport exporter = new SchemaExport(Cfg);
			exporter.Drop(ConsoleOutput, DatabaseExport);
		}

		public DatabaseStatus GetStatus()
		{
			logger.Debug("InstallationManager: checking database status");

			DatabaseStatus status = new DatabaseStatus();

			if (UpdateConnection(status))
				if (UpdateVersion(status))
					if (UpdateSchema(status))
						if (UpdateCount(status))
							if (UpdateItems(status))
								UpdateAppPath(status);
			return status;
		}

		const string installationAppPath = "Installation.AppPath";

		private void UpdateAppPath(DatabaseStatus status)
		{
			try
			{
				if (status.RootItem == null)
					return;

				status.AppPath = status.RootItem[installationAppPath] as string;
				status.NeedsRebase = !string.IsNullOrEmpty(status.AppPath) && !string.Equals(status.AppPath, N2.Web.Url.ToAbsolute("~/"));
			}
			catch (Exception ex)
			{
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
				return true;
			}
			catch (Exception ex)
			{
				status.IsInstalled = false;
				status.ItemsError = ex.Message;
				return false;
			}
		}

		private bool UpdateVersion(DatabaseStatus status)
		{
			try
			{
				status.DatabaseVersion = 0;
				sessionProvider.OpenSession.Session.CreateQuery("select ci.ID from " + typeof(ContentItem).Name + " ci").SetMaxResults(1).List();
				status.DatabaseVersion = 1;

				// checking for properties added between version 1 and 2
				sessionProvider.OpenSession.Session.CreateQuery("select ci.AncestralTrail from " + typeof(ContentItem).Name + " ci").SetMaxResults(1).List();
				status.DatabaseVersion = 2;

				// checking for properties added between version 2 and 3
				sessionProvider.OpenSession.Session.CreateQuery("select ci.AlteredPermissions from " + typeof(ContentItem).Name + " ci").SetMaxResults(1).List();
				status.DatabaseVersion = 3;

				// checking for properties added between version 3 and 4
				sessionProvider.OpenSession.Session.CreateQuery("select ci.TemplateKey from " + typeof(ContentItem).Name + " ci").SetMaxResults(1).List();
				status.DatabaseVersion = 4;

				// checking persistable properties added in application
				sessionProvider.OpenSession.Session.CreateQuery("select ci.ChildState from " + typeof(ContentItem).Name + " ci").SetMaxResults(1).List();
				status.DatabaseVersion = 5;

				// checking persistable properties added in application
				sessionProvider.OpenSession.Session.CreateQuery("select cd.Meta from " + typeof(ContentDetail).Name + " cd").SetMaxResults(1).List();
				status.DatabaseVersion = 6;

				if (isDatabaseFileSystemEnbled)
				{
					// checking file system table (if enabled)
					sessionProvider.OpenSession.Session.CreateQuery("select ci from " + typeof(N2.Edit.FileSystem.NH.FileSystemItem).Name + " ci").SetMaxResults(1).List();
					status.DatabaseVersion = 7;
				}

				// checking persistable properties added in application
				sessionProvider.OpenSession.Session.CreateQuery("select ci from " + typeof(ContentItem).Name + " ci").SetMaxResults(1).List();
				status.DatabaseVersion = DatabaseStatus.RequiredDatabaseVersion;
				return true;
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				return false;
			}
		}

		private bool UpdateSchema(DatabaseStatus status)
		{
			try
			{
				ISession session = sessionProvider.OpenSession.Session;

				session.CreateQuery("from ContentItem").SetMaxResults(1).List();
				session.CreateQuery("from ContentDetail").SetMaxResults(1).List();
				session.CreateQuery("from AuthorizedRole").SetMaxResults(1).List();
				session.CreateQuery("from DetailCollection").SetMaxResults(1).List();

				status.HasSchema = true;

				return true;
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				status.HasSchema = false;
				status.SchemaError = ex.Message;
				status.SchemaException = ex;

				return false;
			}
		}


		private bool UpdateCount(DatabaseStatus status)
		{
			try
			{
				ISession session = sessionProvider.OpenSession.Session;
				status.Items = Convert.ToInt32(session.CreateQuery("select count(*) from ContentItem").UniqueResult());
				status.Details = Convert.ToInt32(session.CreateQuery("select count(*) from ContentDetail").UniqueResult());
				status.DetailCollections = Convert.ToInt32(session.CreateQuery("select count(*) from AuthorizedRole").UniqueResult());
				status.AuthorizedRoles = Convert.ToInt32(session.CreateQuery("select count(*) from DetailCollection").UniqueResult());

				return true;
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				return false;
			}
		}

		private bool UpdateConnection(DatabaseStatus status)
		{
			try
			{
				using (IDbConnection conn = GetConnection())
				{
					status.ConnectionType = conn.GetType().Name;
					conn.Open();
					conn.Close();
				}
				status.IsConnected = true;
				status.ConnectionError = null;
				return true;
			}
			catch (Exception ex)
			{
				status.IsConnected = false;
				status.ConnectionError = ex.Message;
				status.ConnectionException = ex;
				return false;
			}
		}

		/// <summary>Method that will checks the database. If something goes wrong an exception is thrown.</summary>
		/// <returns>A string with diagnostic information about the database.</returns>
		public string CheckDatabase()
		{
			ISession session = sessionProvider.OpenSession.Session;

			// this is supposed to catch mis-matches between database and code e.g. due to refactorings during development
			session.CreateQuery("from ContentItem").SetMaxResults(1000).List();

			int itemCount = Convert.ToInt32(session.CreateQuery("select count(*) from ContentItem").UniqueResult());
			int detailCount = Convert.ToInt32(session.CreateQuery("select count(*) from ContentDetail").UniqueResult());
			int allowedRoleCount = Convert.ToInt32(session.CreateQuery("select count(*) from AuthorizedRole").UniqueResult());
			int detailCollectionCount = Convert.ToInt32(session.CreateQuery("select count(*) from DetailCollection").UniqueResult());

			return string.Format("Database OK, items: {0}, details: {1}, allowed roles: {2}, detail collections: {3}",
									itemCount, detailCount, allowedRoleCount, detailCollectionCount);
		}

		/// <summary>Checks the root node in the database. Throws an exception if there is something really wrong with it.</summary>
		/// <returns>A diagnostic string about the root node.</returns>
		public string CheckRootItem()
		{
			int rootID = host.DefaultSite.RootItemID;
			ContentItem rootItem = persister.Get(rootID);
			if (rootItem != null)
				return String.Format("Root node OK, id: {0}, name: {1}, type: {2}, discriminator: {3}, published: {4} - {5}",
									 rootItem.ID, rootItem.Name, rootItem.GetContentType(),
									 map.GetOrCreateDefinition(rootItem), rootItem.Published, rootItem.Expires);
			else
				return "No root item found with the id: " + rootID;
		}

		/// <summary>Checks the root node in the database. Throws an exception if there is something really wrong with it.</summary>
		/// <returns>A diagnostic string about the root node.</returns>
		public string CheckStartPage()
		{
			int startID = host.DefaultSite.StartPageID;
			ContentItem startPage = persister.Get(startID);
			if (startPage != null)
				return String.Format("Start page OK, id: {0}, name: {1}, type: {2}, discriminator: {3}, published: {4} - {5}",
									 startPage.ID, startPage.Name, startPage.GetContentType(),
									 map.GetOrCreateDefinition(startPage), startPage.Published, startPage.Expires);
			else
				return "No start page found with the id: " + startID;
		}

		public ContentItem InsertRootNode(Type type, string name, string title)
		{
			ContentItem item = activator.CreateInstance(type, null);
			item.Name = name;
			item.Title = title;
			item[InstallationAppPath] = N2.Web.Url.ToAbsolute("~/");
			item[installationHost] = webContext.Url.HostUrl.ToString();
			persister.Save(item);
			return item;
		}

		public ContentItem InsertStartPage(Type type, ContentItem root, string name, string title)
		{
			ContentItem item = activator.CreateInstance(type, root);
			item.Name = name;
			item.Title = title;
			persister.Save(item);
			return item;
		}

		#region Helper Methods

		public IDbConnection GetConnection()
		{
			IDriver driver = GetDriver();

			IDbConnection conn = driver.CreateConnection();
			if (Cfg.Properties.ContainsKey(Environment.ConnectionString))
				conn.ConnectionString = (string)Cfg.Properties[Environment.ConnectionString];
			else if (Cfg.Properties.ContainsKey(Environment.ConnectionStringName))
				conn.ConnectionString = ConfigurationManager.ConnectionStrings[(string)Cfg.Properties[Environment.ConnectionStringName]].ConnectionString;
			else
				throw new Exception("Didn't find a confgiured connection string or connection string name in the nhibernate configuration.");
			return conn;
		}

		public IDbCommand GenerateCommand(CommandType type, string sqlString)
		{
			IDriver driver = GetDriver();
			return driver.GenerateCommand(type, new NHibernate.SqlCommand.SqlString(sqlString), new SqlType[0]);
		}

		private IDriver GetDriver()
		{
			string driverName = (string)Cfg.Properties[Environment.ConnectionDriver];
			Type driverType = NHibernate.Util.ReflectHelper.ClassForName(driverName);
			return (IDriver)Activator.CreateInstance(driverType);
		}

		#endregion

		public ContentItem InsertExportFile(Stream stream, string filename)
		{
			IImportRecord record = importer.Read(stream, filename);
			record.RootItem["Installation.AppPath"] = N2.Web.Url.ToAbsolute("~/");
			try
			{
				record.RootItem["Installation.Host"] = webContext.Url.HostUrl.ToString();
			}
			catch (HttpException)
			{
				// silently ignore "Request is not available in this context" when calling this from init
			}

			importer.Import(record, null, ImportOption.All);

			return record.RootItem;
		}

		/// <summary>Gets definitions suitable as start pages.</summary>
		/// <returns>An enumeration of item definitions.</returns>
		public IEnumerable<ItemDefinition> GetStartDefinitions(IEnumerable<ItemDefinition> allDefinitions)
		{
			ICollection<ItemDefinition> preferred = new List<ItemDefinition>();
			ICollection<ItemDefinition> fallback = new List<ItemDefinition>();

			foreach (ItemDefinition d in allDefinitions)
			{
				InstallerHint hint = d.Installer;

				if (Is(hint, InstallerHint.PreferredStartPage))
					preferred.Add(d);
				if (!Is(hint, InstallerHint.NeverStartPage))
					fallback.Add(d);
			}

			if (preferred.Count == 0)
				preferred = fallback;

			return preferred;
		}

		/// <summary>Gets definitions suitable as root nodes.</summary>
		/// <returns>An enumeration of item definitions.</returns>
		public IEnumerable<ItemDefinition> GetRootDefinitions(IEnumerable<ItemDefinition> allDefinitions)
		{
			ICollection<ItemDefinition> preferred = new List<ItemDefinition>();
			ICollection<ItemDefinition> fallback = new List<ItemDefinition>();

			foreach (ItemDefinition d in allDefinitions)
			{
				InstallerHint hint = d.Installer;

				if (Is(hint, InstallerHint.PreferredRootPage))
					preferred.Add(d);
				if (!Is(hint, InstallerHint.NeverRootPage))
					fallback.Add(d);
			}

			if (preferred.Count == 0)
				preferred = fallback;

			return preferred;
		}

		/// <summary>Gets definitions suitable as start pages and root node.</summary>
		/// <returns>An enumeration of item definitions.</returns>
		public IEnumerable<ItemDefinition> GetRootAndStartDefinitions(IEnumerable<ItemDefinition> allDefinitions)
		{
			ICollection<ItemDefinition> preferred = new List<ItemDefinition>();
			ICollection<ItemDefinition> fallback = new List<ItemDefinition>();

			foreach (ItemDefinition d in allDefinitions)
			{
				InstallerHint hint = d.Installer;

				if (Is(hint, InstallerHint.PreferredRootPage) || Is(hint, InstallerHint.PreferredStartPage))
					preferred.Add(d);
				if (!Is(hint, InstallerHint.NeverRootPage) && !Is(hint, InstallerHint.NeverStartPage))
					fallback.Add(d);
			}

			if (preferred.Count == 0)
				preferred = fallback;

			return preferred;
		}

		/// <summary>Checks installer hint bit flags.</summary>
		/// <param name="flags">The defined flags.</param>
		/// <param name="expected">The expected flags.</param>
		/// <returns>True if the defined contains the expected.</returns>
		public static bool Is(InstallerHint flags, InstallerHint expected)
		{
			return (flags & expected) == expected;
		}

		public const string QueryItemsWithAuthorizedRoles = "select distinct ci from ContentItem ci join ci.AuthorizedRoles ar where ci.AlteredPermissions is null or ci.AlteredPermissions = 0 order by ci.ID";
		public const string QueryItemsWithoutAncestralTrail = "from ContentItem ci where ci.AncestralTrail is null order by ci.ID";

		public virtual IEnumerable<ContentItem> ExecuteQuery(string query)
		{
			int iterationSize = 100;
			long count = persister.Repository.Count();
			long iterations = (count + iterationSize - 1) / iterationSize;
			for (int i = 0; i < iterations; i++)
			{
				var all = this.sessionProvider.OpenSession.Session.CreateQuery(query)
					.SetFirstResult(i * iterationSize)
					.SetMaxResults(iterationSize)
					.List<ContentItem>();

				foreach (var item in all)
				{
					yield return item;
				}
			}
		}

		public void UpdateStatus(SystemStatusLevel currentLevel)
		{
			connectionContext.SetConnected(currentLevel);
		}
	}
}
