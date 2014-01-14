using System;
using System.Collections.Generic;
using System.Security.Principal;
using N2.Tests.Fakes;
using N2.Web;
using NUnit.Framework;
using Rhino.Mocks;

namespace N2.Tests
{
    public abstract class ItemTestsBase
    {
        protected MockRepository mocks;

        [SetUp]
        public virtual void SetUp()
        {
            RequestItem.Accessor = new StaticContextAccessor();
            mocks = new MockRepository();
        }

        [TearDown]
        public virtual void TearDown()
        {
            if (mocks != null)
            {
                mocks.ReplayAll();
                mocks.VerifyAll();
            }
        }

        protected virtual T CreateItem<T>(string name, ContentItem parent = null, string zoneName = null) where T : ContentItem
        {
            T item = (T)Activator.CreateInstance(typeof(T), true);
            item.Name = name;
            item.Title = name;
            item.AncestralTrail = N2.Utility.GetTrail(parent);
            item.AddTo(parent);
            item.Published = N2.Utility.CurrentTime();
            item.State = ContentState.Published;
            item.ZoneName = zoneName;
            return item;
        }

        protected virtual T CreateOneItem<T>(int id, string name, ContentItem parent) where T : ContentItem
        {
            T item = (T) Activator.CreateInstance(typeof (T), true);
            item.ID = id;
            item.Name = name;
            item.Title = name;
            item.AncestralTrail = N2.Utility.GetTrail(parent);
            item.AddTo(parent);
            item.Published = N2.Utility.CurrentTime();
            item.State = ContentState.Published;
            return item;
        }

        protected static IPrincipal CreatePrincipal(string name, params string[] roles)
        {
            return SecurityUtilities.CreatePrincipal(name, roles);
        }

        private Dictionary<string, object> requestItems;
        protected IWebContext CreateWebContext(bool replay)
        {
            requestItems = new Dictionary<string, object>();
            IWebContext context = mocks.StrictMock<N2.Web.IWebContext>();
            Expect.On(context).Call(context.RequestItems).Return(requestItems).Repeat.Any();
            
            if(replay)
                mocks.Replay(context);
            return context;
        }
    }
}
