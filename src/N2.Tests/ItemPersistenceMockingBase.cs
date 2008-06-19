using System;
using System.Collections.Generic;
using System.Text;
using Rhino.Mocks;
using N2.Persistence;
using NUnit.Framework;
using N2.Web;
using Rhino.Mocks.Interfaces;
using System.Web;

namespace N2.Tests
{
	public class ItemPersistenceMockingBase : ItemTestsBase
	{
		protected IPersister persister;
        protected IRepository<int, ContentItem> repository;
        protected IEventRaiser saving;

		delegate void SaveDelegate(ContentItem item);
		public delegate string ToAppRelativeDelegate(string path);
		public delegate string ToAbsoluteDelegate(string path);

		[SetUp]
		public override void  SetUp()
		{
 			base.SetUp();
			persister = CreatePersister();
		}

		protected virtual IPersister CreatePersister()
		{
            repository = mocks.Stub<IRepository<int, ContentItem>>();
            var transaction = mocks.Stub<ITransaction>();
            Expect.Call(repository.BeginTransaction()).Return(transaction).Repeat.Any();
            var linkRepository = mocks.Stub<N2.Persistence.NH.INHRepository<int, N2.Details.LinkDetail>>();
            var finder = mocks.Stub<N2.Persistence.Finder.IItemFinder>();
            persister = new N2.Persistence.NH.ContentPersister(repository, linkRepository, finder);

			return persister;
		}

		protected override T CreateOneItem<T>(int itemID, string name, ContentItem parent)
		{
			T item = base.CreateOneItem<T>(itemID, name, parent);
            Expect.On(repository).Call(repository.Get(itemID)).Return(item).Repeat.Any();
            Expect.On(repository).Call(repository.Load(itemID)).Return(item).Repeat.Any();
            return item;
		}

		#region WebContext
		protected virtual IWebContext CreateWrapper(bool replay)
		{
            IWebContext wrapper = mocks.DynamicMock<IWebContext>();
			Expect.On(wrapper).Call(wrapper.ToAppRelative(null)).IgnoreArguments().Do(new ToAppRelativeDelegate(ToAppRelative)).Repeat.Any();
			Expect.On(wrapper).Call(wrapper.ToAbsolute(null)).IgnoreArguments().Do(new ToAbsoluteDelegate(ToAbsolute)).Repeat.Any();
			Expect.On(wrapper).Call(wrapper.ApplicationUrl).Return("/").Repeat.Any();

			if (replay)
				mocks.Replay(wrapper);
			return wrapper;
		}

		public string ToAppRelative(string path)
		{
			if (path.StartsWith("mailto:"))
				throw new HttpException(path + " is not a valid virtual path.");
			return path;
		}

		public string ToAbsolute(string path)
		{
			return path.Replace("~/", "/");
		} 
		#endregion

	}
}
