using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using MbUnit.Framework;
using Rhino.Mocks;
using N2.Web;

namespace N2.Tests
{
	public class ItemTestsBase
	{
		protected MockRepository mocks;

		[SetUp]
		public virtual void SetUp()
		{
			mocks = new MockRepository();
		}

		//[TearDown]
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
			T item = Activator.CreateInstance<T>();
			item.ID = id;
			item.Name = name;
			item.Title = name;
			item.AddTo(parent);
			return item;
		}

		protected IPrincipal CreatePrincipal(string name, params string[] roles)
		{
			return SecurityUtilities.CreatePrincipal(name, roles);
		}

		private Dictionary<string, object> requestItems;
		protected IWebContext CreateWebContext(bool replay)
		{
			requestItems = new Dictionary<string, object>();
			IWebContext context = mocks.CreateMock<N2.Web.IWebContext>();
			Expect.On(context).Call(context.RequestItems).Return(requestItems).Repeat.Any();
			
			if(replay)
				mocks.Replay(context);
			return context;
		}
	}
}
