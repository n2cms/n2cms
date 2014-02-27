using N2.Edit;
using N2.Tests.Content;
using N2.Web;
using NUnit.Framework;
using N2.Persistence.Sources;
using N2.Persistence;
using N2.Security;

namespace N2.Tests.Edit
{
    [TestFixture]
    public class NavigatorTests : ItemPersistenceMockingBase
    {
        IWebContext webContext;
        Host host;
        ContentSource source;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            webContext = new ThreadContext();
            host = new Host(webContext, 1, 1);
            source = TestSupport.SetupContentSource(webContext, host, persister.Repository);
        }

        [Test]
        public void CanNavigate_FromRoot()
        {
            ContentItem root = CreateOneItem<AnItem>(1, "root", null);
            ContentItem item1 = CreateOneItem<AnItem>(2, "item1", root);
            ContentItem item1_item12 = CreateOneItem<AnItem>(2, "item1.2", item1);

            Navigator n = new Navigator(persister, host, new VirtualNodeFactory(), source);

            ContentItem navigatedItem = n.Navigate("/item1/item1.2");

            Assert.AreSame(item1_item12, navigatedItem);
        }

        [Test]
        public void CanNavigate_FromStartPage()
        {
            ContentItem root = CreateOneItem<AnItem>(1, "root", null);
            ContentItem start = CreateOneItem<AnItem>(2, "start", root);
            ContentItem item1 = CreateOneItem<AnItem>(3, "item1", start);

            Navigator n = new Navigator(persister, new Host(new ThreadContext(), 1, 2), new VirtualNodeFactory(), source);

            ContentItem navigatedItem = n.Navigate("~/item1");

            Assert.AreSame(item1, navigatedItem);
        }

        [Test]
        public void CanNavigate()
        {
            ContentItem root = CreateOneItem<AnItem>(1, "root", null);
            ContentItem item1 = CreateOneItem<AnItem>(2, "item1", root);
            ContentItem item1_item12 = CreateOneItem<AnItem>(2, "item1.2", item1);

            Navigator n = new Navigator(null, host, new VirtualNodeFactory(), source);

            ContentItem navigatedItem = n.Navigate(item1, "item1.2");

            Assert.AreSame(item1_item12, navigatedItem);
        }

        [Test]
        public void CanNavigate_ToRoot()
        {
            ContentItem root = CreateOneItem<AnItem>(1, "root", null);
            ContentItem item1 = CreateOneItem<AnItem>(2, "item1", root);

            Navigator n = new Navigator(persister, host, new VirtualNodeFactory(), source);

            ContentItem navigatedItem = n.Navigate("/");

            Assert.AreSame(root, navigatedItem);
        }

        [Test]
        public void Fallbacks_ToVirtualNodes()
        {
            ContentItem root = CreateOneItem<AnItem>(1, "root", null);
            ContentItem item1 = CreateOneItem<AnItem>(2, "item1", root);

            var factory = new VirtualNodeFactory();
            factory.Register(new FunctionalNodeProvider("/item1/hello/", (p) => new AnItem { Name = p }));
            Navigator n = new Navigator(persister, host, factory, source);

            ContentItem navigatedItem = n.Navigate("/item1/hello/world/");
            Assert.That(navigatedItem.Name, Is.EqualTo("world/"));
        }
    }
}
