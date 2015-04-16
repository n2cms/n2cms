
namespace N2.Tests.Engine
{
	using System;
	using N2.Engine;
	using N2.Persistence;
	using N2.Tests.Content;
	using NUnit.Framework;
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

		#region Save Item

		[Test]
		public void Saving_Item_Fires_Events()
		{
			bool itemSavingRaised = false;
			bool itemSavedRaised = false;

			bool legacyItemSavingRaised = false;
			bool legacyItemSavedRaised = false;

			// EventManager events
			events.ItemSaving += (s, e) => itemSavingRaised = true;
			events.ItemSaved += (s, e) => itemSavedRaised = true;

			// Legacy IPersister events
			persister.ItemSaving += (s, e) => legacyItemSavingRaised = true;
			persister.ItemSaved += (s, e) => legacyItemSavedRaised = true;

			persister.Save(new AnItem());

			// EventsManagers events
			itemSavingRaised.ShouldBe(true);
			itemSavedRaised.ShouldBe(true);

			// Legacy IPersister events
			legacyItemSavingRaised.ShouldBe(true);
			legacyItemSavedRaised.ShouldBe(true);
		}

		[Test]
		public void Cancelled_Saving_Item_DoesNot_Fires_Post_Events()
		{
			bool itemSavingRaised = false;
			bool itemSavedRaised = false;

			bool legacyItemSavingRaised = false;
			bool legacyItemSavedRaised = false;

			// EventManager events
			events.ItemSaving += (s, e) => { e.Cancel = true; itemSavingRaised = true; };
			events.ItemSaved += (s, e) => itemSavedRaised = true;

			// Legacy IPersister events
			persister.ItemSaving += (s, e) => legacyItemSavingRaised = true;
			persister.ItemSaved += (s, e) => legacyItemSavedRaised = true;

			persister.Save(new AnItem());

			// EventsManagers events
			itemSavingRaised.ShouldBe(true);
			itemSavedRaised.ShouldBe(false);

			// Legacy IPersister events
			legacyItemSavingRaised.ShouldBe(true);
			legacyItemSavedRaised.ShouldBe(false);
		}

		[Test]
		public void Legacy_Cancelled_Saving_Item_DoesNot_Fires_Post_Events()
		{
			bool itemSavingRaised = false;
			bool itemSavedRaised = false;

			bool legacyItemSavingRaised = false;
			bool legacyItemSavedRaised = false;

			// EventManager events
			events.ItemSaving += (s, e) => itemSavingRaised = true;
			events.ItemSaved += (s, e) => itemSavedRaised = true;

			// Legacy IPersister events
			persister.ItemSaving += (s, e) => { e.Cancel = true; legacyItemSavingRaised = true; };
			persister.ItemSaved += (s, e) => legacyItemSavedRaised = true;

			persister.Save(new AnItem());

			// EventsManagers events
			itemSavingRaised.ShouldBe(true);
			itemSavedRaised.ShouldBe(false);

			// Legacy IPersister events
			legacyItemSavingRaised.ShouldBe(true);
			legacyItemSavedRaised.ShouldBe(false);
		}

		#endregion
	}
}
