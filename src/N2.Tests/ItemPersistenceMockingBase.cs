using System;
using System.Collections.Generic;
using System.Text;
using Rhino.Mocks;
using N2.Persistence;
using NUnit.Framework;
using N2.Web;
using Rhino.Mocks.Interfaces;
using System.Web;
using N2.Persistence.NH;

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
        protected List<ContentItem> database = new List<ContentItem>();

		[SetUp]
		public override void  SetUp()
		{
 			base.SetUp();
			persister = CreatePersister();
            database = new List<ContentItem>();
		}

        protected ContentItem FindById(int id)
        {
            return database.Find(
                delegate(ContentItem item)
                {
                    return item.ID == id;
                });
        }

		protected virtual IPersister CreatePersister()
		{
            repository = mocks.Stub<IRepository<int, ContentItem>>();
            Expect.Call(repository.Get(0)).IgnoreArguments().Do(new Func<int, ContentItem>(FindById)).Repeat.Any();
            Expect.Call(repository.Load(0)).IgnoreArguments().Do(new Func<int, ContentItem>(FindById)).Repeat.Any();
            var transaction = mocks.Stub<ITransaction>();
            transaction.Replay();
            Expect.Call(repository.BeginTransaction()).Return(transaction).Repeat.Any();
            repository.Replay();

            var linkRepository = mocks.Stub<N2.Persistence.NH.INHRepository<int, N2.Details.LinkDetail>>();
            linkRepository.Replay();
            var finder = mocks.Stub<N2.Persistence.Finder.IItemFinder>();
            finder.Replay();
            return persister = new ContentPersister(repository, linkRepository, finder);
		}

		protected override T CreateOneItem<T>(int itemID, string name, ContentItem parent)
		{
            T item = base.CreateOneItem<T>(itemID, name, parent);
            database.Add(item);
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
