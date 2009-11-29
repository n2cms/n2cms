using System;
using System.Collections.Generic;
using System.Security.Principal;
using NUnit.Framework;
using Rhino.Mocks;
using N2.Web;
using N2.Tests.Fakes;

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

		protected virtual T CreateOneItem<T>(int id, string name, ContentItem parent) where T : ContentItem
		{
			T item = (T) Activator.CreateInstance(typeof (T), true);
			item.ID = id;
			item.Name = name;
			item.Title = name;
			item.AddTo(parent);
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
