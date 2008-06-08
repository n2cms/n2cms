using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using N2.Edit;
using N2.Persistence;
using N2.Web;
using N2.Tests.Content;
using Rhino.Mocks;

namespace N2.Tests.Edit
{
	[TestFixture]
	public class NavigatorTests : ItemTestsBase
	{
		[Test]
		public void CanNavigateFromRoot()
		{
			ContentItem root = CreateOneItem<AnItem>(1, "root", null);
			ContentItem item1 = CreateOneItem<AnItem>(2, "item1", root);
			ContentItem item1_item12 = CreateOneItem<AnItem>(2, "item1.2", item1);

			IPersister persister = mocks.StrictMock<IPersister>();
			Expect.Call(persister.Get(1)).Return(root);

			mocks.ReplayAll();

			Navigator n = new Navigator(persister, new Host(null, 1, 1));

			ContentItem navigatedItem = n.Navigate("/item1/item1.2");

			Assert.AreSame(item1_item12, navigatedItem);
		}

		[Test]
		public void CanNavigateFromStartPage()
		{
			ContentItem root = CreateOneItem<AnItem>(1, "root", null);
			ContentItem start = CreateOneItem<AnItem>(2, "start", root);
			ContentItem item1_item12 = CreateOneItem<AnItem>(2, "item1", start);

			IPersister persister = mocks.StrictMock<IPersister>();
			Expect.Call(persister.Get(2)).Return(start);

			mocks.ReplayAll();

			Navigator n = new Navigator(persister, new Host(null, 1, 2));

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

		[Test]
		public void CanNavigateToRoot()
		{
			ContentItem root = CreateOneItem<AnItem>(1, "root", null);
			ContentItem item1 = CreateOneItem<AnItem>(2, "item1", root);
			
			IPersister persister = mocks.StrictMock<IPersister>();
			Expect.Call(persister.Get(1)).Return(root);

			mocks.ReplayAll();

			Navigator n = new Navigator(persister, new Host(null, 1, 1));

			ContentItem navigatedItem = n.Navigate("/");

			Assert.AreSame(root, navigatedItem);
		}
	}
}
