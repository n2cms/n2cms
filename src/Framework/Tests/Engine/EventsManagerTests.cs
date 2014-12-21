
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
	using Shouldly;

	[TestFixture]
	public class EventsManagerTests
	{
		private EventsManager events;
		private ContentPersister persister;

		[SetUp]
		public void SetUp()
		{
			var repository = new Fakes.FakeContentItemRepository();
			var sources = TestSupport.SetupContentSource(repository);
			events = new EventsManager();
			persister = new ContentPersister(sources, repository, events);
		}

		[Test]
		public void Saving_Item_Fires_PreEvents()
		{
			bool preEventWasCalled = false;

			events.ItemSaving += (s, e) => preEventWasCalled = true;
			persister.Save(new AnItem());

			preEventWasCalled.ShouldBe(true);
		}

		[Test]
		public void Saving_Item_Fires_PostEvents()
		{
			bool postEventWasCalled = false;

			events.ItemSaved += (s, e) => postEventWasCalled = true;
			persister.Save(new AnItem());

			postEventWasCalled.ShouldBe(true);
		}

		[Test]
		public void Cancelled_Saving_Item_DoesNot_Fires_Events()
		{
			bool postEventWasCalled = false;

			events.ItemSaving += (s, e) => e.Cancel = true;
			events.ItemSaved += (s, e) => postEventWasCalled = true;
			persister.Save(new AnItem());

			postEventWasCalled.ShouldBe(false);
		}
	}
}
