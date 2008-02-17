using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using N2.Edit;
using N2.Persistence;
using N2.Web;
using N2.Tests.Content;
using Rhino.Mocks;

namespace N2.Tests.Edit
{
	[TestFixture]
	class NavigatorTests : ItemTestsBase
	{
		[Test]
		public void CanNavigateFromRoot()
		{
			ContentItem root = CreateOneItem<AnItem>(1, "root", null);
			ContentItem item1 = CreateOneItem<AnItem>(2, "item1", root);
			ContentItem item1_item12 = CreateOneItem<AnItem>(2, "item1.2", item1);

			IPersister persister = mocks.CreateMock<IPersister>();
			Expect.Call(persister.Get(1)).Return(root);

			mocks.ReplayAll();

			Navigator n = new Navigator(persister, new Site(1));

			ContentItem navigatedItem = n.Navigate("/item1/item1.2");

			Assert.AreSame(item1_item12, navigatedItem);
		}

		[Test]
		public void CanNavigateFromStartPage()
		{
			ContentItem root = CreateOneItem<AnItem>(1, "root", null);
			ContentItem start = CreateOneItem<AnItem>(2, "start", root);
			ContentItem item1_item12 = CreateOneItem<AnItem>(2, "item1", start);

			IPersister persister = mocks.CreateMock<IPersister>();
			Expect.Call(persister.Get(2)).Return(start);

			mocks.ReplayAll();

			Navigator n = new Navigator(persister, new Site(1, 2));

			ContentItem navigatedItem = n.Navigate("~/item1");

			Assert.AreSame(item1_item12, navigatedItem);
		}

		[Test]
		public void CanNavigate()
		{
			ContentItem root = CreateOneItem<AnItem>(1, "root", null);
			ContentItem item1 = CreateOneItem<AnItem>(2, "item1", root);
			ContentItem item1_item12 = CreateOneItem<AnItem>(2, "item1.2", item1);

			Navigator n = new Navigator(null, null);

			ContentItem navigatedItem = n.Navigate(item1, "item1.2");

			Assert.AreSame(item1_item12, navigatedItem);
		}
	}
}
