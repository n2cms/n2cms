using System;
using System.Configuration;
using N2.Configuration;
using N2.Definitions;
using N2.Definitions.Static;
using N2.Details;
using N2.Edit;
using N2.Edit.Workflow;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Persistence.NH;
using N2.Persistence.NH.Finder;
using N2.Persistence.Proxying;
using N2.Security;
using N2.Tests.Fakes;
using N2.Web;
using NHibernate.Tool.hbm2ddl;
using Rhino.Mocks;
using N2.Persistence.Sources;
using N2.Persistence.Behaviors;

namespace N2.Tests
{
    public static class TestSupport
    {
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
            ITypeFinder typeFinder = new Fakes.FakeTypeFinder(itemTypes[0].Assembly, itemTypes);

			DefinitionBuilder definitionBuilder = new DefinitionBuilder(new DefinitionMap(), typeFinder, new TransformerBase<IUniquelyNamed>[0], SetupEngineSection());
			notifier = new ItemNotifier();
			proxyFactory = new InterceptingProxyFactory();
			activator = new ContentActivator(new N2.Edit.Workflow.StateChanger(), notifier, proxyFactory);
			definitionProviders = new IDefinitionProvider[] { new DefinitionProvider(definitionBuilder) };
			definitions = new DefinitionManager(definitionProviders, new ITemplateProvider[0], activator, new StateChanger());
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
			versions = new VersionManager(persister.Repository, finder, changer, new N2.Configuration.EditSection());
			editor = new EditManager(definitions, persister, versions, new SecurityManager(new ThreadContext(), new EditSection()), null, null, null, changer, new EditableHierarchyBuilder(new SecurityManager(new ThreadContext(), new EditSection()), SetupEngineSection()), null);
        }

        public static void Setup(out ContentPersister persister, ISessionProvider sessionProvider, N2.Persistence.IRepository<ContentItem> itemRepository, INHRepository<ContentDetail> linkRepository, SchemaExport schemaCreator)
        {
            persister = new ContentPersister(itemRepository, linkRepository);
			new BehaviorInvoker(persister, new N2.Definitions.Static.DefinitionMap()).Start();

            schemaCreator.Execute(false, true, false, sessionProvider.OpenSession.Session.Connection, null);
        }

        internal static void Setup(out ContentPersister persister, FakeSessionProvider sessionProvider, SchemaExport schemaCreator)
        {
            var itemRepository = new ContentItemRepository(sessionProvider);
            var linkRepository = new NHRepository<ContentDetail>(sessionProvider);

            Setup(out persister, sessionProvider, itemRepository, linkRepository, schemaCreator);
        }

		public static ContentPersister SetupFakePersister()
		{
			FakeRepository<ContentItem> repository;
			FakeRepository<ContentDetail> linkRepository;
			return SetupFakePersister(out repository, out linkRepository);
		}

		public static ContentPersister SetupFakePersister(out FakeRepository<ContentItem> repository, out FakeRepository<ContentDetail> linkRepository)
		{
			repository = new Fakes.FakeRepository<ContentItem>();
			linkRepository = new Fakes.FakeRepository<ContentDetail>();
			
			return new ContentPersister(repository, linkRepository);
		}

		public static UrlParser Setup(IPersister persister, FakeWebContextWrapper wrapper, IHost host)
		{
			return new UrlParser(persister, wrapper, host, new N2.Plugin.ConnectionMonitor(), new HostSection());
		}

		public static EngineSection SetupEngineSection()
		{
			return new EngineSection { Definitions = new DefinitionCollection { DefineUnattributedTypes = true } };
		}

		public static N2.Persistence.Sources.ContentSource SetupContentSource(IWebContext webContext, IHost host, IPersister persister)
		{
			return new ContentSource(new SecurityManager(webContext, new N2.Configuration.EditSection()), new[] { new DatabaseSource(host, persister.Repository) });
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
	}
}
