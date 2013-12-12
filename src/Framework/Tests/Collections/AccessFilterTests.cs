using System.Security.Principal;
using N2.Collections;
using N2.Configuration;
using N2.Security;
using NUnit.Framework;

namespace N2.Tests.Collections
{
    [TestFixture]
    public class AccessFilterTests : FilterTestsBase
    {
        ISecurityManager security;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            security = new SecurityManager(base.CreateWebContext(true), new EditSection());
            mocks.ReplayAll();
        }

        [Test]
        public void CanFilter_TwoItems_WithStaticMethod()
        {
            ItemList items = Create4Items();

            IPrincipal user = CreatePrincipal("JustAnotherUser", "Role1");

            AccessFilter.Filter(items, user, security);

            Assert.AreEqual(2, items.Count);
        }

        [Test]
        public void CanFilter_TwoItems_WithClassInstance()
        {
            ItemList items = Create4Items();

            IPrincipal user = CreatePrincipal("JustAnotherUser", "Role1");
            mocks.ReplayAll();

            AccessFilter filter = new AccessFilter(user, security);
            filter.Filter(items);

            Assert.AreEqual(2, items.Count);
        }

        private ItemList Create4Items()
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
            DynamicPermissionMap.SetRoles(item, Permission.Read, authorizedRoles);
            return item;
        }
    }
}
