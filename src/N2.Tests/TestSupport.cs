using System;
using System.Configuration;
using N2.Configuration;
using N2.Definitions;
using N2.Details;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Persistence.NH.Finder;
using N2.Tests.Fakes;
using NHibernate.Tool.hbm2ddl;
using N2.Edit;
using N2.Persistence.Finder;
using N2.Security;
using N2.Web;

namespace N2.Tests
{
    public static class TestSupport
    {
        public static void Setup(out IDefinitionManager definitions, out IItemNotifier interceptor, out FakeSessionProvider sessionProvider, out ItemFinder finder, out SchemaExport schemaCreator, params Type[] itemTypes)
        {
            Setup(out definitions, out interceptor, itemTypes);

            DatabaseSection config = (DatabaseSection)ConfigurationManager.GetSection("n2/database");
            ConnectionStringsSection connectionStrings = (ConnectionStringsSection)ConfigurationManager.GetSection("connectionStrings");
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder(definitions, new ClassMappingGenerator(), config, connectionStrings);

            FakeWebContextWrapper context = new Fakes.FakeWebContextWrapper();
            sessionProvider = new FakeSessionProvider(new ConfigurationSource(configurationBuilder), interceptor, context);

            finder = new ItemFinder(sessionProvider, definitions);

            schemaCreator = new SchemaExport(configurationBuilder.BuildConfiguration());
        }

        public static void Setup(out IDefinitionManager definitions, out IItemNotifier notifier, params Type[] itemTypes)
        {
            ITypeFinder typeFinder = new Fakes.FakeTypeFinder(itemTypes[0].Assembly, itemTypes);

            DefinitionBuilder definitionBuilder = new DefinitionBuilder(typeFinder, new EngineSection());
            notifier = new NotifyingInterceptor();
            definitions = new DefinitionManager(definitionBuilder, new N2.Workflow.StateChanger(), notifier);
        }

        public static void Setup(out N2.Edit.IEditManager editor, out IVersionManager versions, IDefinitionManager definitions, IPersister persister, IItemFinder finder)
        {
            var changer = new N2.Workflow.StateChanger();
            versions = new VersionManager(persister.Repository, finder, changer);
            editor = new EditManager(definitions, persister, versions, new SecurityManager(new ThreadContext(), new EditSection()), null, null, changer, null);
        }

        public static void Setup(out ContentPersister persister, ISessionProvider sessionProvider, N2.Persistence.IRepository<int, ContentItem> itemRepository, INHRepository<int, N2.Details.LinkDetail> linkRepository, ItemFinder finder, SchemaExport schemaCreator)
        {
            persister = new ContentPersister(itemRepository, linkRepository, finder);

#if NH2_1
            schemaCreator.Execute(false, true, false, sessionProvider.OpenSession.Session.Connection, null);
#else
			schemaCreator.Execute(false, true, false, false, sessionProvider.OpenSession.Session.Connection, null);
#endif
        }

        internal static void Setup(out ContentPersister persister, FakeSessionProvider sessionProvider, ItemFinder finder, SchemaExport schemaCreator)
        {
            IRepository<int, ContentItem> itemRepository = new ContentItemRepository(sessionProvider);
            INHRepository<int, LinkDetail> linkRepository = new NHRepository<int, LinkDetail>(sessionProvider);

            Setup(out persister, sessionProvider, itemRepository, linkRepository, finder, schemaCreator);
        }
    }
}
