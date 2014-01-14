using System.Linq;
using N2.Details;
using N2.Management.Externals;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Tests;
using N2.Tests.Fakes;
using N2.Web;
using NUnit.Framework;
using Rhino.Mocks;
using N2.Persistence.Behaviors;

namespace N2.Management.Tests.ExternalContent
{
    [TestFixture]
    public class ExternalContentRepositoryTests
    {
        Externals.ExternalContentRepository externalRepository;
        ContentItem root;
        ContentItem start;

        [SetUp]
        public void SetUp()
        {
            externalRepository = SetupRepository(out root, out start);
        }

        public static Externals.ExternalContentRepository SetupRepository(out ContentItem root, out ContentItem start)
        {
            IContentItemRepository itemRepository;
            var persister = TestSupport.SetupFakePersister(out itemRepository);
            var activator = new Persistence.ContentActivator(new Edit.Workflow.StateChanger(), MockRepository.GenerateStub<IItemNotifier>(), new Persistence.Proxying.EmptyProxyFactory());
            itemRepository.SaveOrUpdate(root = new ExternalItem { ID = 1, Name = "root" });
            itemRepository.SaveOrUpdate(start = new ExternalItem { ID = 2, Name = "start" });

            new BehaviorInvoker(persister, new N2.Definitions.Static.DefinitionMap()).Start();

            //return new Externals.ExternalContentRepository(new Edit.ContainerRepository<Externals.ExternalItem>(persister, MockRepository.GenerateStub<IItemFinder>(), new Host(new ThreadContext(), 1, 2), activator) { Navigate = true }, persister, activator, new Configuration.EditSection());
            return new Externals.ExternalContentRepository(new Edit.ContainerRepository<Externals.ExternalItem>(itemRepository, new Host(new ThreadContext(), 1, 2), activator, new Definitions.Static.DefinitionMap()) { Navigate = true }, persister, activator, new Configuration.EditSection());
        }

        [Test]
        public void Container_IsCreated()
        {
            externalRepository.GetOrCreate("Family", "Key", "/hello/world");

            var container = start.Children.Single();
            Assert.That(container, Is.InstanceOf<ExternalItem>());
            Assert.That(container.Name, Is.EqualTo(ExternalItem.ExternalContainerName));
            Assert.That(container.IsPage, Is.False);
        }

        [Test]
        public void FamilyContainer_IsCreated()
        {
            externalRepository.GetOrCreate("Family", "Key", "/hello/world");

            var container = start.Children.Single();
            var family = container.Children.Single();
            Assert.That(family, Is.InstanceOf<ExternalItem>());
            Assert.That(family.Name, Is.EqualTo("Family"));
            Assert.That(family.ZoneName, Is.EqualTo(ExternalItem.ExternalContainerName));
            Assert.That(family.IsPage, Is.False);
        }

        [Test]
        public void FamilyContainerKey_MayDifferInCasing()
        {
            var item1 = externalRepository.GetOrCreate("Family", "Key", "/hello/world");
            var item2 = externalRepository.GetOrCreate("family", "Key", "/hello/world");
            
            Assert.That(item2, Is.SameAs(item1));
        }

        [Test]
        public void ExternalItem_IsCreated()
        {
            externalRepository.GetOrCreate("Family", "Key", "/hello/world");

            var container = start.Children.Single();
            var family = container.Children.Single();
            var item = family.Children.Single();
            Assert.That(item, Is.InstanceOf<ExternalItem>());
            Assert.That(item.Name, Is.EqualTo("Key"));
            Assert.That(item.ZoneName, Is.EqualTo("Family"));
            Assert.That(item.IsPage, Is.True);
            Assert.That(item.Url, Is.EqualTo("/hello/world"));
        }

        [Test]
        public void ExternalItemKey_MayDifferInCasing()
        {
            var item1 = externalRepository.GetOrCreate("Family", "Key", "/hello/world");
            var item2 = externalRepository.GetOrCreate("Family", "key", "/hello/world");

            Assert.That(item2, Is.SameAs(item1));
        }

        [Test]
        public void ExternalItem_MayHaveEmptyKey()
        {
            var item1 = externalRepository.GetOrCreate("Family", "", "/hello/world");
            var item2 = externalRepository.GetOrCreate("Family", "", "/hello/world");

            Assert.That(item2, Is.SameAs(item1));
        }

        [Test]
        public void ExternalItem_MayHaveNullKey()
        {
            var item1 = externalRepository.GetOrCreate("Family", null, "/hello/world");
            var item2 = externalRepository.GetOrCreate("Family", null, "/hello/world");

            Assert.That(item2, Is.SameAs(item1));
        }
    }
}
