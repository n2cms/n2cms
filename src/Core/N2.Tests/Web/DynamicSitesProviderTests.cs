using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using N2.Web;
using N2.Persistence;

namespace N2.Tests.Web
{
	[TestFixture]
	public class DynamicSitesProviderTests : ItemPersistenceMockingBase
	{
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
		public void CanFindSiteOnRootItem()
		{
			ContentItem rootItem = CreateTheItemTree();
			mocks.ReplayAll();

			DynamicSitesProvider sitesProvider = new DynamicSitesProvider(persister, rootItem.ID);
			sitesProvider.RecursionDepth = 0;
			Assert.AreEqual(1, sitesProvider.GetSites().Count);
		}

		[Test]
		public void CanFindSitesOneDepthDown()
		{
			ContentItem rootItem = CreateTheItemTree();
			mocks.ReplayAll();

			DynamicSitesProvider sitesProvider = new DynamicSitesProvider(persister, rootItem.ID);
			sitesProvider.RecursionDepth = 1;
			Assert.AreEqual(3, sitesProvider.GetSites().Count);
		}

		[Test]
		public void CanFindSitesTwoDepthsDown()
		{
			ContentItem rootItem = CreateTheItemTree();
			mocks.ReplayAll();

			DynamicSitesProvider sitesProvider = new DynamicSitesProvider(persister, rootItem.ID);
			sitesProvider.RecursionDepth = 2;
			Assert.AreEqual(5, sitesProvider.GetSites().Count);
		}
	}
}
