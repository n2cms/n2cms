using System;
using N2.Details;
using N2.Persistence;
using N2.Persistence.NH;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using N2.Tests.Fakes;

namespace N2.Tests
{
	public abstract class ItemPersistenceMockingBase : ItemTestsBase
	{
		protected IPersister persister;
		protected FakeRepository<ContentItem> repository;
		protected IEventRaiser saving;

		public delegate string ToAppRelativeDelegate(string path);
		public delegate string ToAbsoluteDelegate(string path);
		protected int greatestID = 0;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			persister = CreatePersister();
		}

		protected virtual IPersister CreatePersister()
		{
			persister = TestSupport.SetupFakePersister();
			repository = (FakeRepository<ContentItem>)persister.Repository;
			
			return persister;
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