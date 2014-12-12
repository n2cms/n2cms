
namespace N2.Engine
{
	using System;

	public interface IEventsManager
	{
		// todo : introduce ItemVersionSaving and ItemVersionSaved 

		/// <summary>Invoked before item has been saved</summary>
		event EventHandler<CancellableItemEventArgs> ItemSaving;

		/// <summary>Invoked after item has been saved</summary>
		event EventHandler<ItemEventArgs> ItemSaved;

		void ItemSave(ContentItem item, object sender);
	}
}
