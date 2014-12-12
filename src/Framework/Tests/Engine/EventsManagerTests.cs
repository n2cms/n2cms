
namespace N2.Tests.Engine
{
	using System;
	using N2.Tests.Content;
	using NUnit.Framework;

	[TestFixture]
	public class EventsManagerTests : PersistenceAwareBase
	{
		[Test]
		public void Saving_Item_Fires_Events()
		{
			bool itemSavingRaised = false;
			bool itemSavedRaised = false;

			N2.Context.Current.EventsManager.ItemSaving += (s, e) => itemSavingRaised = true;
			N2.Context.Current.EventsManager.ItemSaving += (s, e) => itemSavedRaised = true;

			AnItem item = new AnItem();
			N2.Context.Persister.Save(item);

			Assert.IsTrue(itemSavingRaised);
			Assert.IsTrue(itemSavedRaised);
		}
	}
}
