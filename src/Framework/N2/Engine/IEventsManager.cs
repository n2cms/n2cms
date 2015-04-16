
namespace N2.Engine
{
	using System;
	using N2.Persistence;

	public interface IEventsManager
	{
		event EventHandler<CancellableItemEventArgs> ItemSaving;
		event EventHandler<ItemEventArgs> ItemSaved;

		//// todo : introduce ItemVersionSaving and ItemVersionSaved 

		void TriggerItemSave(
			EventHandler<CancellableItemEventArgs> legacyPreHandler,
			ContentItem item, 
			Action<ContentItem> finalAction,
			EventHandler<ItemEventArgs> legacyPostHandler,
			ITransaction commitTriggersPostEvent = null);
	}
}
