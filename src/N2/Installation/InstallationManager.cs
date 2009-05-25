using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using N2.Definitions;
using N2.Details;
using N2.Persistence.NH;
using N2.Security;
using N2.Serialization;
using N2.Web;
using NHibernate;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using Environment=NHibernate.Cfg.Environment;
using N2.Persistence;
using NHibernate.SqlTypes;

namespace N2.Installation
{
    /// <summary>
    /// Wraps functionality to request database status and generate n2's 
    /// database schema on multiple database flavours.
    /// </summary>
	public class InstallationManager
	{
        IConfigurationBuilder configurationBuilder;
        IDefinitionManager definitions;
        Importer importer;
        IPersister persister;
        ISessionProvider sessionProvider;
        IHost host;

        public InstallationManager(IHost host, IDefinitionManager definitions, Importer importer, IPersister persister, ISessionProvider sessionProvider, IConfigurationBuilder configurationBuilder)
		{
            this.host = host;
            this.definitions = definitions;
            this.importer = importer;
            this.persister = persister;
            this.sessionProvider = sessionProvider;
            this.configurationBuilder = configurationBuilder;
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
			SchemaExport exporter = new SchemaExport(Cfg);
			exporter.Create(ConsoleOutput, DatabaseExport);
		}

		public void ExportSchema(TextWriter output)
		{
			SchemaExport exporter = new SchemaExport(Cfg);
			exporter.Execute(ConsoleOutput, NoDatabaseExport, false, true, null, output);
		}

		public void DropDatabaseTables()
		{
			SchemaExport exporter = new SchemaExport(Cfg);
			exporter.Drop(ConsoleOutput, DatabaseExport);
		}

		public DatabaseStatus GetStatus()
		{
			DatabaseStatus status = new DatabaseStatus();

			UpdateConnection(status);
			UpdateSchema(status);
			UpdateItems(status);

			return status;
		}

		private void UpdateItems(DatabaseStatus status)
		{
			try
			{
				status.StartPageID = host.DefaultSite.StartPageID;
                status.RootItemID = host.DefaultSite.RootItemID;
				status.StartPage = persister.Get(status.StartPageID);
				status.RootItem = persister.Get(status.RootItemID);
				status.IsInstalled = status.RootItem != null && status.StartPage != null;
			} 
			catch (Exception ex)
			{
				status.IsInstalled = false;
				status.ItemsError = ex.Message;
			}
		}

		private void UpdateSchema(DatabaseStatus status)
		{
			try
			{
				using (sessionProvider)
				{
                    ISession session = sessionProvider.OpenSession.Session;
					status.Items = session.CreateCriteria(typeof (ContentItem)).List().Count;
					status.Details = session.CreateCriteria(typeof(ContentDetail)).List().Count;
					status.DetailCollections = session.CreateCriteria(typeof(DetailCollection)).List().Count;
					status.AuthorizedRoles = session.CreateCriteria(typeof(AuthorizedRole)).List().Count;
				}
				status.HasSchema = true;
			}
			catch(Exception ex)
			{
				status.HasSchema = false;
				status.SchemaError = ex.Message;
				status.SchemaException = ex;
			}
		}

		private void UpdateConnection(DatabaseStatus status)
		{
			try
			{
				using (IDbConnection conn = GetConnection())
				{
					conn.Open();
					conn.Close();
					status.ConnectionType = conn.GetType().Name;
				}
				status.IsConnected = true;
				status.ConnectionError = null;
			}
			catch(Exception ex)
			{
				status.IsConnected = false;
				status.ConnectionError = ex.Message;
				status.ConnectionException = ex;
			}
		}

		/// <summary>Method that will checks the database. If something goes wrong an exception is thrown.</summary>
		/// <returns>A string with diagnostic information about the database.</returns>
		public string CheckDatabase()
		{
            using (sessionProvider)
			{
                ISession session = sessionProvider.OpenSession.Session;
				int itemCount = session.CreateCriteria(typeof (ContentItem)).List().Count;
				int detailCount = session.CreateCriteria(typeof (ContentDetail)).List().Count;
				int allowedRoleCount = session.CreateCriteria(typeof (AuthorizedRole)).List().Count;
				int detailCollectionCount = session.CreateCriteria(typeof (DetailCollection)).List().Count;
				return string.Format("Database OK, items: {0}, details: {1}, allowed roles: {2}, detail collections: {3}",
				                     itemCount, detailCount, allowedRoleCount, detailCollectionCount);
			}
		}

		/// <summary>Checks the root node in the database. Throws an exception if there is something really wrong with it.</summary>
		/// <returns>A diagnostic string about the root node.</returns>
		public string CheckRootItem()
		{
			int rootID = host.DefaultSite.RootItemID;
			ContentItem rootItem = persister.Get(rootID);
			if (rootItem != null)
				return String.Format("Root node OK, id: {0}, name: {1}, type: {2}, discriminator: {3}, published: {4} - {5}",
									 rootItem.ID, rootItem.Name, rootItem.GetType(),
									 definitions.GetDefinition(rootItem.GetType()), rootItem.Published, rootItem.Expires);
			else
				return "No root item found with the id: " + rootID;
		}

		/// <summary>Checks the root node in the database. Throws an exception if there is something really wrong with it.</summary>
		/// <returns>A diagnostic string about the root node.</returns>
		public string CheckStartPage()
		{
			int startID = host.DefaultSite.StartPageID;
			ContentItem startPage = persister.Get(startID);
			if(startPage != null)
				return String.Format("Root node OK, id: {0}, name: {1}, type: {2}, discriminator: {3}, published: {4} - {5}",
									 startPage.ID, startPage.Name, startPage.GetType(),
									 definitions.GetDefinition(startPage.GetType()), startPage.Published, startPage.Expires);
			else
				return "No start page found with the id: " + startID;
		}

		public ContentItem InsertRootNode(Type type, string name, string title)
		{
			ContentItem item = definitions.CreateInstance(type, null);
			item.Name = name;
			item.Title = title;
			persister.Save(item);
			return item;
		}

		public ContentItem InsertStartPage(Type type, ContentItem root, string name, string title)
		{
			ContentItem item = definitions.CreateInstance(type, root);
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
            importer.Import(record, null, ImportOption.All);

			return record.RootItem;
		}

		/// <summary>Gets definitions suitable as start pages.</summary>
		/// <returns>An enumeration of item definitions.</returns>
		public IEnumerable<ItemDefinition> GetStartDefinitions()
		{
			ICollection<ItemDefinition> preferred = new List<ItemDefinition>();
			ICollection<ItemDefinition> fallback = new List<ItemDefinition>();

			foreach (ItemDefinition d in definitions.GetDefinitions())
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
		public IEnumerable<ItemDefinition> GetRootDefinitions()
		{
			ICollection<ItemDefinition> preferred = new List<ItemDefinition>();
			ICollection<ItemDefinition> fallback = new List<ItemDefinition>();

			foreach (ItemDefinition d in definitions.GetDefinitions())
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
		public IEnumerable<ItemDefinition> GetRootAndStartDefinitions()
		{
			ICollection<ItemDefinition> preferred = new List<ItemDefinition>();
			ICollection<ItemDefinition> fallback = new List<ItemDefinition>();
			
			foreach (ItemDefinition d in definitions.GetDefinitions())
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
	}
}
