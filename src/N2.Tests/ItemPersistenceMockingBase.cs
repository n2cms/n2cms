using System;
using System.Collections.Generic;
using System.Text;
using N2.Details;
using Rhino.Mocks;
using N2.Persistence;
using NUnit.Framework;
using N2.Web;
using Rhino.Mocks.Interfaces;
using System.Web;
using N2.Persistence.NH;

namespace N2.Tests
{
	public abstract class ItemPersistenceMockingBase : ItemTestsBase
	{
        protected IPersister persister;
        protected IRepository<int, ContentItem> repository;
        protected IEventRaiser saving;

		public delegate string ToAppRelativeDelegate(string path);
		public delegate string ToAbsoluteDelegate(string path);
		protected int greatestID = 0;

		[SetUp]
		public override void  SetUp()
		{
 			base.SetUp();
			persister = CreatePersister();
		}

		protected virtual IPersister CreatePersister()
		{
			repository = new Fakes.FakeRepository<ContentItem>();

			var linkRepository = mocks.Stub<INHRepository<int, LinkDetail>>();
            linkRepository.Replay();
            var finder = mocks.Stub<N2.Persistence.Finder.IItemFinder>();
            finder.Replay();

			return persister = new ContentPersister(repository, linkRepository, finder);
		}

		protected override T CreateOneItem<T>(int itemID, string name, ContentItem parent)
		{
			greatestID = Math.Max(greatestID, itemID);
            T item = base.CreateOneItem<T>(itemID, name, parent);
            repository.Save(item);
            return item;
		}

	}
}
