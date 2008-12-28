using System.Linq;
using N2.Tests.Web.Items;
using NUnit.Framework;
using N2.Web;
using N2.Configuration;
using NUnit.Framework.SyntaxHelpers;

namespace N2.Tests.Web
{
	[TestFixture]
	public class DynamicSitesProviderTests : ItemPersistenceMockingBase
	{
		ContentItem rootItem;
	    private IHost host;
	    private HostSection config;
	    private DynamicSitesProvider sitesProvider;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			rootItem = CreateTheItemTree();
            host = new Host(new Fakes.FakeWebContextWrapper(), rootItem.ID, rootItem.ID);
			mocks.ReplayAll();

		    config = new HostSection {RootID = rootItem.ID, StartPageID = rootItem.ID};
            sitesProvider = new DynamicSitesProvider(persister, host, config);
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
		public void CanFindSite_OnRootItem()
		{
			sitesProvider.RecursionDepth = 0;
            Assert.That(sitesProvider.GetSites().Count(), Is.EqualTo(1));
		}

		[Test]
		public void CanFindSites_One_DepthDown()
		{
			sitesProvider.RecursionDepth = 1;
			EnumerableAssert.Count(3, sitesProvider.GetSites());
		}

		[Test]
		public void CanFindSites_Two_DepthsDown()
		{
			sitesProvider.RecursionDepth = 2;
			EnumerableAssert.Count(5, sitesProvider.GetSites());
		}
	}
}
