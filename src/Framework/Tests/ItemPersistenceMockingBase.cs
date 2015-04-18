
namespace N2.Tests
{
	using System;
	using N2.Persistence;
	using N2.Tests.Fakes;
	using NUnit.Framework;
	using Rhino.Mocks.Interfaces;
	using N2.Web;
	using N2.Engine;

	public abstract class ItemPersistenceMockingBase : ItemTestsBase
	{
		protected IPersister persister;
		protected FakeContentItemRepository repository;
		protected IEventRaiser saving;
		protected IEventsManager events;

		public delegate string ToAppRelativeDelegate(string path);
		public delegate string ToAbsoluteDelegate(string path);
		protected int greatestID = 0;
		private FakeWebContextWrapper webContext;
		private UrlParser urlParser;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			persister = CreatePersister();
			events = TestSupport.SetupEventsManager();
			urlParser = TestSupport.Setup(persister, webContext = new FakeWebContextWrapper(), new Host(webContext, 1, 1));
		}

		protected virtual IPersister CreatePersister()
		{
			persister = TestSupport.SetupFakePersister();
			repository = (FakeContentItemRepository)persister.Repository;
			
			return persister;
		}

		protected override T CreateOneItem<T>(int itemID, string name, ContentItem parent)
		{
			greatestID = Math.Max(greatestID, itemID);
			T item = base.CreateOneItem<T>(itemID, name, parent);
			((IInjectable<IUrlParser>)item).Set(urlParser);
			repository.Save(item);
			return item;
		}
	}
}
