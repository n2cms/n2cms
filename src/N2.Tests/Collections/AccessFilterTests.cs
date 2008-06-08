using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using N2.Collections;
using System.Security.Principal;
using N2.Security;
using Rhino.Mocks;

namespace N2.Tests.Collections
{
	[TestFixture]
	public class AccessFilterTests : FilterTestsBase
	{
		N2.Persistence.IPersister persister;
		ISecurityManager security;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			persister = mocks.DynamicMock<N2.Persistence.IPersister>();
			security = new SecurityManager(base.CreateWebContext(true));
		}

		[Test]
		public void CanFilterTwoItemsWithStaticMethod()
		{
			ItemList items = CreateTheItems();

			using (mocks.Record())
			{
				IPrincipal user = CreateUser();
				mocks.ReplayAll();

				AccessFilter.Filter(items, user, security);

				Assert.AreEqual(2, items.Count);
			}

			Assert.AreEqual(2, items.Count);
		}

		[Test]
		public void CanFilterTwoItemsWithClassInstance()
		{
			ItemList items = CreateTheItems();

			using (mocks.Record())
			{
				IPrincipal user = CreateUser();
				mocks.ReplayAll();

				AccessFilter filter = new AccessFilter(user, security);
				filter.Filter(items);

				Assert.AreEqual(2, items.Count);
			}

			Assert.AreEqual(2, items.Count);
		}

		private ItemList CreateTheItems()
		{
			ItemList items = new ItemList();
			items.Add(CreateItem<FirstItem>(1, "Role1"));
			items.Add(CreateItem<FirstItem>(2, "Role1", "Role2"));
			items.Add(CreateItem<FirstItem>(3, "Role2"));
			items.Add(CreateItem<FirstItem>(4, "Role2", "Role3"));
			return items;
		}
		protected ContentItem CreateItem<T>(int id, params string[] authorizedRoles) where T : ContentItem
		{
			T item = CreateOneItem<T>(id, id.ToString(), null);
			foreach (string role in authorizedRoles)
				item.AuthorizedRoles.Add(new N2.Security.AuthorizedRole(item, role));
			return item;
		}

		protected IPrincipal CreateUser()
		{
			IPrincipal user = mocks.StrictMock<IPrincipal>();
			IIdentity identity = mocks.StrictMock<IIdentity>();

			Expect.On(user).Call(user.Identity).Return(identity).Repeat.Any();
			Expect.On(identity).Call(identity.Name).Return("JustAnotherUser").Repeat.Any();
			Expect.On(user).Call(user.IsInRole("Administrators")).Return(false).Repeat.Any();
			Expect.On(user).Call(user.IsInRole("Editors")).Return(false).Repeat.Any();
			Expect.On(user).Call(user.IsInRole("Role1")).Return(true).Repeat.Any();
			Expect.On(user).Call(user.IsInRole("Role2")).Return(false).Repeat.Any();
			Expect.On(user).Call(user.IsInRole("Role3")).Return(false).Repeat.Any();
			return user;
		}
	}
}
