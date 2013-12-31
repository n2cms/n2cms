using System.Linq;
using N2.Tests.Fakes;
using N2.Tests.Web.Items;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Web
{
    [TestFixture]
    public class DynamicSitesProviderTests : ItemPersistenceMockingBase
    {
        ContentItem rootItem;
        private IHost host;
        private DynamicSitesProvider sitesProvider;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            rootItem = CreateTheItemTree();
            host = new Host(new Fakes.FakeWebContextWrapper(), rootItem.ID, rootItem.ID);
            mocks.ReplayAll();

            sitesProvider = new DynamicSitesProvider(new FakeDescendantItemFinder(), persister, host);
        }

        protected SiteProvidingItem CreateTheItemTree()
        {
            int id = 22;
            SiteProvidingItem rootItem = CreateOneItem<SiteProvidingItem>(++id, id.ToString(), null);
            SiteProvidingItem site1 = CreateOneItem<SiteProvidingItem>(++id, id.ToString(), rootItem);
            SiteProvidingItem site2 = CreateOneItem<SiteProvidingItem>(++id, id.ToString(), rootItem);
            PageItem item3 = CreateOneItem<PageItem>(++id, id.ToString(), rootItem);
            PageItem item4 = CreateOneItem<PageItem>(++id, id.ToString(), rootItem);
            PageItem item1_1 = CreateOneItem<PageItem>(++id, id.ToString(), site1);
            PageItem item1_2 = CreateOneItem<PageItem>(++id, id.ToString(), site1);
            PageItem item2_1 = CreateOneItem<PageItem>(++id, id.ToString(), site2);
            PageItem item3_1 = CreateOneItem<PageItem>(++id, id.ToString(), item3);
            PageItem item3_2 = CreateOneItem<PageItem>(++id, id.ToString(), item3);
            SiteProvidingItem site1_1 = CreateOneItem<SiteProvidingItem>(++id, id.ToString(), site1);
            SiteProvidingItem site4_1 = CreateOneItem<SiteProvidingItem>(++id, id.ToString(), item4);
            return rootItem;
        }

        [Test]
        public void CanFind_AllSites()
        {
            Assert.That(sitesProvider.GetSites().Count(), Is.EqualTo(5));
        }
    }
}
