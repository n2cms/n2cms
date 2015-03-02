using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using N2.Configuration;
using N2.Definitions;
using N2.Definitions.Static;
using N2.Details;
using N2.Edit.Trash;
using N2.Edit.Versioning;
using N2.Engine;
using N2.Installation;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Persistence.Serialization;
using N2.Plugin;
using N2.Web;
using NHibernate;
using NHibernate.Driver;
using NHibernate.SqlTypes;
using NHibernate.Tool.hbm2ddl;
using Environment = NHibernate.Cfg.Environment;
using System.Linq;

namespace N2.Edit.Installation
{
    public abstract class InstallationManager
    {
        public const string InstallationAppPath = "Installation.AppPath";
        public const string installationHost = "Installation.Host";
        public const string installationAssemblyVersion = "Installation.AssemblyVersion";
        public const string installationFileVersion = "Installation.FileVersion";
        public const string installationFeatures = "Installation.Features";
        public const string installationImageSizes = "Installation.ImageSizes";

        public abstract string CheckConnection(out string stackTrace);
        public abstract string CheckDatabase();
        public abstract string CheckRootItem();
        public abstract string CheckStartPage();
        public abstract DatabaseStatus GetStatus();
        public abstract void Upgrade();
        public abstract void Install();
        public abstract IEnumerable<ContentItem> ExecuteQuery(string query);
        public abstract string ExportSchema();
        public abstract void ExportSchema(TextWriter output);
        public abstract string ExportUpgradeSchema();
        ConnectionMonitor connectionContext;
        private Importer importer;
        private IWebContext webContext;
        private IPersister persister;
        private ContentActivator activator;

        public InstallationManager(ConnectionMonitor connectionContext, Importer importer, IWebContext webContext, IPersister persister, ContentActivator activator)
        {
            this.connectionContext = connectionContext;
            this.importer = importer;
            this.webContext = webContext;
            this.persister = persister;
            this.activator = activator;
        }

        public virtual void UpdateStatus(SystemStatusLevel currentLevel)
        {
            connectionContext.SetConnected(currentLevel);
        }

        public virtual ContentItem InsertExportFile(Stream stream, string filename)
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

        public virtual ContentItem InsertRootNode(Type type, string name, string title)
        {
            ContentItem item = activator.CreateInstance(type, null);
            item.Name = name;
            item.Title = title;
            item[InstallationAppPath] = N2.Web.Url.ToAbsolute("~/");
            item[installationHost] = webContext.Url.HostUrl.ToString();
            persister.Save(item);
            return item;
        }

        public virtual ContentItem InsertStartPage(Type type, ContentItem root, string name, string title)
        {
            ContentItem item = activator.CreateInstance(type, root);
            item.Name = name;
            item.Title = title;
            persister.Save(item);
            return item;
        }

        /// <summary>Gets definitions suitable as start pages.</summary>
        /// <returns>An enumeration of item definitions.</returns>
        public virtual IEnumerable<ItemDefinition> GetStartDefinitions(IEnumerable<ItemDefinition> allDefinitions)
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
        public virtual IEnumerable<ItemDefinition> GetRootDefinitions(IEnumerable<ItemDefinition> allDefinitions)
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
        public virtual IEnumerable<ItemDefinition> GetRootAndStartDefinitions(IEnumerable<ItemDefinition> allDefinitions)
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
    }

    /// <summary>
    /// Wraps functionality to request database status and generate n2's 
    /// database schema on multiple database flavours.
    /// </summary>
    [Service]
    [Service(typeof(InstallationManager))]
    public class NHInstallationManager : InstallationManager
    {
        private readonly Engine.Logger<NHInstallationManager> logger;

        IConfigurationBuilder configurationBuilder;
        ContentActivator activator;
        Importer importer;
        IPersister persister;
        ISessionProvider sessionProvider;
        IHost host;
        IWebContext webContext;
        DefinitionMap map;
        ConnectionMonitor connectionContext;
        DatabaseSection config;
        bool isDatabaseFileSystemEnbled = false;
        NHibernate.Cfg.Configuration cfg;

        public NHInstallationManager(IHost host, DefinitionMap map, ContentActivator activator, Importer importer, IPersister persister, ISessionProvider sessionProvider, IConfigurationBuilder configurationBuilder, IWebContext webContext, ConnectionMonitor connectionContext, DatabaseSection config)
            : base(connectionContext, importer, webContext, persister, activator)
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
            this.config = config;
            this.isDatabaseFileSystemEnbled = config.Files.StorageLocation == FileStoreLocation.Database;
        }

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
        public override void Install()
        {
            ClearNHCache();

            SchemaExport exporter = new SchemaExport(Cfg);
            exporter.Create(ConsoleOutput, DatabaseExport);
        }

        private void ClearNHCache()
        {
            var sf = configurationBuilder.BuildSessionFactory();
            sf.EvictQueries();
            foreach (var collectionMetadata in sf.GetAllCollectionMetadata())
                sf.EvictCollection(collectionMetadata.Key);
            foreach (var classMetadata in sf.GetAllClassMetadata())
                sf.EvictEntity(classMetadata.Key);
        }

        public override void ExportSchema(TextWriter output)
        {
            SchemaExport exporter = new SchemaExport(Cfg);
            exporter.Execute(ConsoleOutput, NoDatabaseExport, false, null, output);
        }

        public override string ExportSchema()
        {
            StringBuilder sql = new StringBuilder();
            using (StringWriter sw = new StringWriter(sql))
            {
                ExportSchema(sw);
            }
            return sql.ToString();
        }



        public override void Upgrade()
        {
            SchemaUpdate schemaUpdater = new SchemaUpdate(Cfg);
            schemaUpdater.Execute(true, true);

            ClearNHCache();

            var status = GetStatus();
            if (status.RootItem != null)
            {
                status.RootItem[InstallationManager.installationAssemblyVersion] = typeof(N2.Context).Assembly.GetName().Version.ToString();
                status.RootItem[InstallationManager.installationFileVersion] = typeof(N2.Context).Assembly.GetFileVersion();
                persister.Save(status.RootItem);
            }
        }

        public override string ExportUpgradeSchema()
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

        public override DatabaseStatus GetStatus()
        {
            logger.Debug("checking database status");

            DatabaseStatus status = new DatabaseStatus();

            if (IsSql(status))
                if (UpdateConnection(status))
                    if (UpdateVersion(status))
                        if (UpdateSchema(status))
                            if (UpdateCount(status))
                                if (UpdateItems(status))
                                    UpdateRecordedValues(status);
            return status;
        }

        private bool IsSql(DatabaseStatus status)
        {
            if (config.Flavour.IsFlagSet(DatabaseFlavour.NoSql))
                return false;
            return true;
        }


        private void UpdateRecordedValues(DatabaseStatus status)
        {
            try
            {
                if (status.RootItem == null)
                    return;

                status.AppPath = status.RootItem[InstallationAppPath] as string;
                status.NeedsRebase = !string.IsNullOrEmpty(status.AppPath) && !string.Equals(status.AppPath, N2.Web.Url.ToAbsolute("~/"));

                Version v;
                if (status.RootItem[installationAssemblyVersion] != null && Version.TryParse(status.RootItem[installationAssemblyVersion].ToString(), out v))
                    status.RecordedAssemblyVersion = v;
                if (status.RootItem[installationFileVersion] != null && Version.TryParse(status.RootItem[installationFileVersion].ToString(), out v))
                    status.RecordedFileVersion = v;

                status.RecordedFeatures = status.RootItem.GetInstalledFeatures();

                status.RecordedImageSizes = status.RootItem.GetInstalledImageSizes().ToArray();
            }
            catch (Exception ex)
            {
                status.ItemsError = ex.Message;
            }
        }

	    /// <summary>
	    /// Determines if the given item is in the trash or is the trash can itself. There will be problems if the root node or start page is trashed.
	    /// </summary>
	    /// <param name="item"></param>
	    /// <returns></returns>
		private bool IsTrashed(ContentItem item)
	    {
		    if (item == null)
			    return false;

			if (item is ITrashCan)
			    return true;

			/* N2 should detect if item is in trash */
		    if (Find.Closest<ITrashCan>(item) != null)
			    return true;

		    return false;
	    }

	    private bool UpdateItems(DatabaseStatus status)
        {
            try
            {
                status.StartPageID = host.DefaultSite.StartPageID;
                status.RootItemID = host.DefaultSite.RootItemID;
                status.StartPage = persister.Get(status.StartPageID);
                status.RootItem = persister.Get(status.RootItemID);
	            status.IsInstalled = status.RootItem != null && status.StartPage != null
					&& !IsTrashed(status.RootItem) && !IsTrashed(status.StartPage) /* fix #583 -- ~/N2 should detect if the RootNode is Trash */;

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

                // checking for properties added between 4 and 5
                sessionProvider.OpenSession.Session.CreateQuery("select ci.ChildState from " + typeof(ContentItem).Name + " ci").SetMaxResults(1).List();
                status.DatabaseVersion = 5;

                // checking for properties added between 5 and 6
                sessionProvider.OpenSession.Session.CreateQuery("select cd.Meta from " + typeof(ContentDetail).Name + " cd").SetMaxResults(1).List();
                status.DatabaseVersion = 6;

                // checking for properties added between 6 and 7
                sessionProvider.OpenSession.Session.CreateQuery("select cv.FuturePublish from " + typeof(ContentVersion).Name + " cv").SetMaxResults(1).List();
                status.DatabaseVersion = 7;

                if (isDatabaseFileSystemEnbled)
                {
                    // checking file system table (if enabled)
                    sessionProvider.OpenSession.Session.CreateQuery("select ci from " + typeof(N2.Edit.FileSystem.NH.FileSystemItem).Name + " ci").SetMaxResults(1).List();
                    status.DatabaseVersion = 8;
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
                //session.CreateQuery("from ContentVersion").SetMaxResults(1).List();

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
                status.Versions = Convert.ToInt32(session.CreateQuery("select count(*) from ContentVersion").UniqueResult());
                
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

        public override string CheckConnection(out string stackTrace)
        {
            try
            {
                using (IDbConnection conn = GetConnection())
                {
                    conn.Open();
                    stackTrace = null;
                    return "Connection OK";
                }
            }
            catch (Exception ex)
            {
                stackTrace = ex.StackTrace;
                return "Connection problem, hopefully this error message can help you figure out what's wrong: <br/>" +
                                 ex.Message;
            }
        }
        
        /// <summary>Method that will checks the database. If something goes wrong an exception is thrown.</summary>
        /// <returns>A string with diagnostic information about the database.</returns>
        public override string CheckDatabase()
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
        public override string CheckRootItem()
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
        public override string CheckStartPage()
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

        public const string QueryItemsWithAuthorizedRoles = "select distinct ci from ContentItem ci join ci.AuthorizedRoles ar where ci.AlteredPermissions is null or ci.AlteredPermissions = 0 order by ci.ID";
        public const string QueryItemsWithoutAncestralTrail = "from ContentItem ci where ci.AncestralTrail is null order by ci.ID";

        public override IEnumerable<ContentItem> ExecuteQuery(string query)
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
    }
}
