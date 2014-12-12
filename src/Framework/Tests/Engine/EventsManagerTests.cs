
namespace N2.Tests.Engine
{
	using System;

	using N2.Definitions;
	using N2.Edit;
	using N2.Edit.Versioning;
	using N2.Engine;
	using N2.Persistence;
	using N2.Persistence.Sources;
	using N2.Plugin;
	using N2.Tests.Content;
	using N2.Tests.Details.Models;

	using NUnit.Framework;

	using Rhino.Mocks;

	[TestFixture]
	public class EventsManagerTests
	{
		private Fakes.FakeEngine engine;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			TestSupport.InitializeHttpContext("/", "");

			N2.Context.Replace(engine = new Fakes.FakeEngine());
			engine.Initialize();
			engine.AddComponentInstance<IPersister>(new ContentPersister(
					MockRepository.GenerateStub<ContentSource>(),
					MockRepository.GenerateStub<IContentItemRepository>(),
					MockRepository.GenerateStub<IEventsManager>()
				));
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			N2.Context.Replace(null);
		}

		[Test]
		public void Saving_Item_Fires_Events()
		{
			//bool itemSavingRaised = false;
			//bool itemSavedRaised = false;

			//N2.Context.Current.EventsManager.ItemSaving += (s, e) => itemSavingRaised = true;
			//N2.Context.Current.EventsManager.ItemSaving += (s, e) => itemSavedRaised = true;

			//MockRepository mocks = new MockRepository();
			//IPersister persister = mocks.DynamicMock<IPersister>();
			//IEngine eng = mocks.DynamicMock<IEngine>(persister);
			//mocks.ReplayAll();
			
			AnItem item = new AnItem();
			this.engine.Persister.Save(item);

			this.engine.Persister.AssertWasCalled(x => x.Save(item));
			this.engine.Persister.VerifyAllExpectations();

			//Assert.IsTrue(itemSavingRaised);
			//Assert.IsTrue(itemSavedRaised);
		}

		[Test]
		public void Cancelled_Saving_Item_DoesNot_Fires_Events()
		{
			//N2.Context.Current.Persister = MockRepository.GenerateStub<IPersister>();

			//N2.Context.Current.EventsManager.ItemSaving += (s, e) =>
			//{
			//	e.Cancel = true; 
			//	itemSavingRaised = true; 
			//};

			//N2.Context.Current.EventsManager.ItemSaving += (s, e) => itemSavedRaised = true;

			//AnItem item = new AnItem();
			//N2.Context.Persister.Save(item);

			//Assert.IsTrue(itemSavingRaised);
			//Assert.IsTrue(itemSavedRaised);
		}
	}
}
