using System;
using System.Configuration;
using N2.Configuration;
using N2.Definitions;
using N2.Definitions.Static;
using N2.Details;
using N2.Edit;
using N2.Edit.Versioning;
using N2.Edit.Workflow;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Persistence.NH;
using N2.Persistence.NH.Finder;
using N2.Persistence.Proxying;
using N2.Persistence.Serialization;
using N2.Plugin;
using N2.Security;
using N2.Tests.Fakes;
using N2.Tests.Workflow.Items;
using N2.Web;
using NHibernate.Tool.hbm2ddl;
using Rhino.Mocks;
using N2.Persistence.Sources;
using N2.Persistence.Behaviors;
using System.Linq;
using System.Web.UI;
using System.Reflection;
using System.Web;
using System.Collections;
using System.IO;
using N2.Engine.Globalization;
using System.Text;

namespace N2.Tests
{
    public static class TestSupport
    {
        public static void ShouldBe(this DateTime actual, DateTime expected, TimeSpan tolerance)
        {
            if (Math.Abs(actual.Subtract(expected).TotalMilliseconds) > tolerance.TotalMilliseconds)
                throw new Exception(actual + " != " + expected + " (tolerance: " + tolerance + ")");
        }

        public static void ShouldBe(this DateTime? actual, DateTime? expected, TimeSpan tolerance)
        {
            if (!actual.HasValue && !expected.HasValue)
                return;
            else if (actual.HasValue && expected.HasValue)
            {
                if (Math.Abs(actual.Value.Subtract(expected.Value).TotalMilliseconds) > tolerance.TotalMilliseconds)
                    throw new Exception(actual + " != " + expected + " (tolerance: " + tolerance + ")");
            }
            else
                throw new Exception(actual + " != " + expected);
        }

        public static void Setup(out IDefinitionManager definitions, out ContentActivator activator, out IItemNotifier notifier, out FakeSessionProvider sessionProvider, out ItemFinder finder, out SchemaExport schemaCreator, out InterceptingProxyFactory proxyFactory, params Type[] itemTypes)
        {
			var participators = new ConfigurationBuilderParticipator[] { new RelationConfigurationBuilderParticipator() };
			FakeWebContextWrapper context = new Fakes.FakeWebContextWrapper();
			DatabaseSection config = (DatabaseSection)ConfigurationManager.GetSection("n2/database");
			Setup(out definitions, out activator, out notifier, out sessionProvider, out finder, out schemaCreator, out proxyFactory, context, config, participators, itemTypes);
        }

		public static void Setup(out IDefinitionManager definitions, out ContentActivator activator, out IItemNotifier notifier, out FakeSessionProvider sessionProvider, out ItemFinder finder, out SchemaExport schemaCreator, out InterceptingProxyFactory proxyFactory, IWebContext context, DatabaseSection config, ConfigurationBuilderParticipator[] participators, params Type[] itemTypes)
		{
			IDefinitionProvider[] definitionProviders;
			Setup(out definitionProviders, out definitions, out activator, out notifier, out proxyFactory, itemTypes);

			var connectionStrings = (ConnectionStringsSection)ConfigurationManager.GetSection("connectionStrings");
			var map = new DefinitionMap();
			var configurationBuilder = new ConfigurationBuilder(definitionProviders, new ClassMappingGenerator(map), new ThreadContext(), participators, config, connectionStrings);
			var configurationSource = new ConfigurationSource(configurationBuilder);

			sessionProvider = new FakeSessionProvider(configurationSource, new NHInterceptorFactory(proxyFactory, notifier), context);
			sessionProvider.CurrentSession = null;

			finder = new ItemFinder(sessionProvider, map);

			schemaCreator = new SchemaExport(configurationSource.BuildConfiguration());
		}

		public static IDefinitionManager SetupDefinitions(params Type[] itemTypes)
		{
			IItemNotifier notifier;
			IDefinitionProvider[] definitionProviders;
			IDefinitionManager definitions;
			InterceptingProxyFactory proxyFactory;
			ContentActivator activator;
			Setup(out definitionProviders, out definitions, out activator, out notifier, out proxyFactory, itemTypes);
			return definitions;
		}

		public static void Setup(out IDefinitionProvider[] definitionProviders, out IDefinitionManager definitions, out ContentActivator activator, out IItemNotifier notifier, out InterceptingProxyFactory proxyFactory, params Type[] itemTypes)
        {
            ITypeFinder typeFinder = new Fakes.FakeTypeFinder(itemTypes.Select(t => t.Assembly).FirstOrDefault() ?? Assembly.GetExecutingAssembly(), itemTypes);

			var map = new DefinitionMap();
			var definitionBuilder = new DefinitionBuilder(map, typeFinder, new TransformerBase<IUniquelyNamed>[0], SetupEngineSection());
			notifier = new ItemNotifier();
			proxyFactory = new InterceptingProxyFactory();
			activator = new ContentActivator(new N2.Edit.Workflow.StateChanger(), notifier, proxyFactory);
			definitionProviders = new IDefinitionProvider[] { new DefinitionProvider(definitionBuilder) };
			definitions = new DefinitionManager(definitionProviders, new [] { new TemplateProvider(activator, map) }, activator, new StateChanger());
			((DefinitionManager)definitions).Start();
		}

		public static T Stub<T>()
			where T: class
		{
			return Rhino.Mocks.MockRepository.GenerateStub<T>();
		}

        public static void Setup(out N2.Edit.IEditManager editor, out IVersionManager versions, IDefinitionManager definitions, IPersister persister, IItemFinder finder)
        {
            var changer = new N2.Edit.Workflow.StateChanger();
			versions = new VersionManager(TestSupport.CreateVersionRepository(), persister.Repository, changer, new N2.Configuration.EditSection());
			editor = new EditManager(definitions, persister, versions, new SecurityManager(new ThreadContext(), new EditSection()), null, null, null, changer, new EditableHierarchyBuilder(new SecurityManager(new ThreadContext(), new EditSection()), SetupEngineSection()), null);
        }

        public static void Setup(out ContentPersister persister, ISessionProvider sessionProvider, IContentItemRepository itemRepository, IRepository<ContentDetail> linkRepository, SchemaExport schemaCreator)
        {
			var source = SetupContentSource(itemRepository);
			persister = new ContentPersister(source, itemRepository);
			new BehaviorInvoker(persister, new N2.Definitions.Static.DefinitionMap()).Start();

            schemaCreator.Execute(false, true, false, sessionProvider.OpenSession.Session.Connection, null);
        }

		public static ContentSource SetupContentSource(IContentItemRepository itemRepository)
		{
			return new ContentSource(MockRepository.GenerateStub<ISecurityManager>(), new SourceBase[] { new ActiveContentSource(), new DatabaseSource(MockRepository.GenerateStub<IHost>(), itemRepository) });
		}

        internal static void Setup(out ContentPersister persister, FakeSessionProvider sessionProvider, SchemaExport schemaCreator)
        {
            var itemRepository = new ContentItemRepository(sessionProvider);
            var linkRepository = new NHRepository<ContentDetail>(sessionProvider);

            Setup(out persister, sessionProvider, itemRepository, linkRepository, schemaCreator);
        }

		public static ContentPersister SetupFakePersister()
		{
			IContentItemRepository repository;
			return SetupFakePersister(out repository);
		}

		public static ContentPersister SetupFakePersister(out FakeContentItemRepository repository)
		{
			repository = new Fakes.FakeContentItemRepository();

			var sources = SetupContentSource(repository);
			return new ContentPersister(sources, repository);
		}

		public static ContentPersister SetupFakePersister(out IContentItemRepository repository)
		{
			repository = new Fakes.FakeContentItemRepository();

			var sources = SetupContentSource(repository);
			return new ContentPersister(sources, repository);
		}

		public static UrlParser Setup(IPersister persister, FakeWebContextWrapper wrapper, IHost host)
		{
			return new UrlParser(persister, wrapper, host, new N2.Plugin.ConnectionMonitor(), new HostSection());
		}

		public static EngineSection SetupEngineSection()
		{
			return new EngineSection { Definitions = new DefinitionCollection { DefineUnattributedTypes = true } };
		}

		public static ContentSource SetupContentSource(IWebContext webContext, IHost host, IContentItemRepository repository)
		{
			return new ContentSource(new SecurityManager(webContext, new N2.Configuration.EditSection()), new[] { new DatabaseSource(host, repository) });
		}

		public static WebAppTypeFinder TypeFinder()
		{
			var config = new EngineSection();
			config.Assemblies.Clear();
			config.Assemblies.Add(new AssemblyElement { Assembly = typeof(TestSupport).Assembly.FullName });

			return TypeFinder(config);
		}

		public static WebAppTypeFinder TypeFinder(EngineSection config)
		{
			var context = new ThreadContext();
			var finder = new WebAppTypeFinder(new TypeCache(new N2.Persistence.BasicTemporaryFileHelper(context)), config);
			finder.AssemblyRestrictToLoadingPattern = new System.Text.RegularExpressions.Regex("N2.Tests");
			return finder;
		}

		public static N2.Edit.Versioning.ContentVersionRepository CreateVersionRepository(params Type[] definedTypes)
		{
			IPersister persister = null;
			return CreateVersionRepository(ref persister, definedTypes);
		}

		public static N2.Edit.Versioning.DraftRepository CreateDraftRepository(ref IPersister persister, params Type[] definedTypes)
		{
			return new DraftRepository(CreateVersionRepository(ref persister, definedTypes), new CacheWrapper(persister, new ThreadContext(), new DatabaseSection()));
		}

		public static N2.Edit.Versioning.ContentVersionRepository CreateVersionRepository(ref IPersister persister, params Type[] definedTypes)
		{
			if (persister == null)
				persister = SetupFakePersister();
			var definitions = SetupDefinitions(definedTypes);
			var parser = new UrlParser(persister, new ThreadContext(), new Host(new ThreadContext(), new HostSection()), new ConnectionMonitor(), new HostSection());
			var importer = new Importer(persister,
				new ItemXmlReader(definitions,
					new ContentActivator(new StateChanger(), null, new EmptyProxyFactory()),
					persister.Repository),
				new Fakes.FakeMemoryFileSystem());
			var exporter = new Exporter(
				new ItemXmlWriter(
					definitions,
					parser,
					new FakeMemoryFileSystem()));
			return new ContentVersionRepository(
				new FakeRepository<ContentVersion>(),
				exporter,
				importer,
				parser,
				new EmptyProxyFactory());
		}

		public static ContentActivator SetupContentActivator()
		{
			return new ContentActivator(new N2.Edit.Workflow.StateChanger(), new ItemNotifier(), new N2.Persistence.Proxying.EmptyProxyFactory());
		}

		public static Host SetupHost()
		{
			return new Host(new ThreadContext(), new HostSection());
		}

		public static ContentSource SetupContentSource(params SourceBase[] sources)
		{
			return new ContentSource(new SecurityManager(new ThreadContext(), new EditSection()), sources);
		}

		public static VersionManager SetupVersionManager(IPersister persister, ContentVersionRepository versionRepository)
		{
			return new VersionManager(versionRepository, persister.Repository, new N2.Edit.Workflow.StateChanger(), new EditSection());
		}
		public static void InitRecursive(this Page page)
		{
			typeof(Page).GetMethod("InitRecursive", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(page, new[] { page });
		}

		public static IDisposable InitializeHttpContext(string appRelativeExecutionFilePath, string queryString)
		{
			var request = new HttpRequest("/Default.aspx", "http://localhost/", queryString);
			request.Browser = new HttpBrowserCapabilities();
			request.Browser.Capabilities = new Hashtable();
			request.Browser.Capabilities["ecmascriptversion"] = "1.7";
			request.Browser.Capabilities["w3cdomversion"] = "2.0";
			var response = new HttpResponse(new StringWriter(new StringBuilder()));
			HttpContext.Current = new HttpContext(request, response)
			{
				ApplicationInstance = new HttpApplication(),
				User = SecurityUtilities.CreatePrincipal("admin")
			};

			return new Scope(() => HttpContext.Current = null);
		}
	}
}
