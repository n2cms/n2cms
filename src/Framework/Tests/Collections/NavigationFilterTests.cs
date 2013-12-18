using System;
using System.Security.Principal;
using N2.Collections;
using N2.Engine;
using N2.Security;
using NUnit.Framework;
using Rhino.Mocks;

namespace N2.Tests.Collections
{
    [TestFixture]
    public class NavigationFilterTests : FilterTestsBase
    {
        public delegate bool IsAuthorizedDelegate(ContentItem item, IPrincipal user);

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            ISecurityManager security = mocks.StrictMock<ISecurityManager>();
            Expect.On(security).Call(security.IsAuthorized((ContentItem)null, null)).IgnoreArguments().Repeat.Any().Do(
                new IsAuthorizedDelegate(delegate(ContentItem item, IPrincipal user)
                                            {
                                                return (item.AuthorizedRoles == null || item.AuthorizedRoles.Count == 0);
                                            }));
            IEngine engine = mocks.StrictMock<IEngine>();
            Expect.On(engine).Call(engine.SecurityManager).Return(security);
            mocks.ReplayAll();

            Context.Replace(engine);
        }
        protected ItemList CreateList()
        {
            ItemList list = new ItemList();
            list.Add(CreateOneItem<FirstItem>(1, "one", null));
            list.Add(CreateOneItem<SecondItem>(2, "two", null));
            list.Add(CreateOneItem<NonPageItem>(3, "three", null));
            return list;
        }
        
        [Test]
        public void CanFilterNonPages()
        {
            ItemList list = CreateList();

            (new NavigationFilter()).Filter(list);

            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void CanFilterNonVisible()
        {
            ItemList list = CreateList();
            list[0].Visible = false;

            (new NavigationFilter()).Filter(list);

            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void CanFilterNonAuthorized()
        {
            ItemList list = CreateList();
            list[0].AuthorizedRoles.Add(new AuthorizedRole(list[0], "Administrator"));

            (new NavigationFilter()).Filter(list);

            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void CanFilterUnpublished()
        {
            ItemList list = CreateList();
            list[0].Published = N2.Utility.CurrentTime().AddDays(1);

            (new NavigationFilter()).Filter(list);

            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void CanFilterExpired()
        {
            ItemList list = CreateList();
            list[0].Expires = N2.Utility.CurrentTime().AddDays(-1);

            (new NavigationFilter()).Filter(list);

            Assert.AreEqual(1, list.Count);
        }
    }
}
