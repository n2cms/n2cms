using System.Linq;
using N2.Engine;
using N2.Engine.MediumTrust;
using N2.Tests.Engine.Items;
using N2.Tests.Fakes;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Engine
{
    [TestFixture]
    public class WindsorContentAdapterProviderTest : ContentAdapterProviderTest
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            provider = new ContentAdapterProvider(new ContentEngine(), new AppDomainTypeFinder());
            provider.Start();
        }
    }

    [TestFixture]
    public class MediumTrustContentAdapterProviderTest : ContentAdapterProviderTest
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            provider = new ContentAdapterProvider(new ContentEngine(new MediumTrustServiceContainer(), EventBroker.Instance, new ContainerConfigurer()), new AppDomainTypeFinder());
            provider.Start();
        }

    }

    public abstract class ContentAdapterProviderTest : ItemPersistenceMockingBase
    {
        protected ContentItem aItem, aaItem, abItem, acItem, aaaItem, aabItem;
        FakeWebContextWrapper webContext;
        protected ContentAdapterProvider provider;
            
        public override void SetUp()
        {
            base.SetUp();

            webContext = new FakeWebContextWrapper("http://www.n2cms.com/");
            
            aItem = CreateOneItem<ItemA>(0, "root", null);
            aaItem = CreateOneItem<ItemAA>(0, "aa", aItem);
            abItem = CreateOneItem<ItemAB>(0, "ab", aItem);
            acItem = CreateOneItem<ItemAC>(0, "ac", aItem);
            aaaItem = CreateOneItem<ItemAAA>(0, "aaa", aItem);
            aabItem = CreateOneItem<ItemAAB>(0, "aab", aItem);
        }

        [Test]
        public void Adapters_AreSorted_AccordingTo_InheritanceDepth()
        {
            var adapters = provider.Adapters.ToList();
            int aIndex = adapters.FindIndex(d => d.AdaptedType == typeof(ItemA));
            int aaIndex = adapters.FindIndex(d => d.AdaptedType == typeof(ItemAA));
            int aaaIndex = adapters.FindIndex(d => d.AdaptedType == typeof(ItemAAA));

            Assert.That(aIndex, Is.GreaterThan(aaIndex));
            Assert.That(aaIndex, Is.GreaterThan(aaaIndex));
        }

        [Test]
        public void Adapters_AreSorted_AccordingTo_InheritanceDepth_InterfacesFirst()
        {
            var adapters = provider.Adapters.ToList();
            int aIndex = adapters.FindIndex(d => d.AdaptedType == typeof(ItemA));
            int aaIndex = adapters.FindIndex(d => d.AdaptedType == typeof(ItemAA));
            int aaaIndex = adapters.FindIndex(d => d.AdaptedType == typeof(ItemAAA));
            int iiIndex = adapters.FindIndex(d => d.AdaptedType == typeof(IInterfacedItem));

            Assert.That(aaIndex, Is.LessThan(aIndex));
            Assert.That(aaaIndex, Is.LessThan(aaIndex));
            Assert.That(iiIndex, Is.LessThan(aaaIndex));
        }

        [Test]
        public void ResolvesAdapter_OfItemA()
        {
            RequestAdapter adapter = provider.ResolveAdapter<RequestAdapter>(aItem);

            Assert.That(adapter, Is.TypeOf(typeof(AdapterA)));
        }

        [Test]
        public void ResolvesAdapter_OfItemAA()
        {
            RequestAdapter adapter = provider.ResolveAdapter<RequestAdapter>(aaItem);

            Assert.That(adapter, Is.TypeOf(typeof(AdapterAA)));
        }

        [Test]
        public void ResolvesAdapter_OfItemAAA()
        {
            RequestAdapter adapter = provider.ResolveAdapter<RequestAdapter>(aaaItem);

            Assert.That(adapter, Is.TypeOf(typeof(AdapterAAA)));
        }

        [Test]
        public void ResolvesAdapter_OfParentItemAdapter()
        {
            RequestAdapter adapter = provider.ResolveAdapter<RequestAdapter>(abItem);

            Assert.That(adapter, Is.TypeOf(typeof(AdapterA)));
        }

        [Test]
        public void ResolvesAdapter_OfMostRelevant_ParentItemAdapter()
        {
            RequestAdapter adapter = provider.ResolveAdapter<RequestAdapter>(aabItem);

            Assert.That(adapter, Is.TypeOf(typeof(AdapterAA)));
        }

        [Test]
        public void ResolvesAdapter_OfInterface_BeforeClassAdapter()
        {
            RequestAdapter adapter = provider.ResolveAdapter<RequestAdapter>(acItem);

            Assert.That(adapter, Is.TypeOf(typeof(AdapterIInterfaced)));
        }
    }
}
