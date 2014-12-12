
namespace N2.Engine
{
	using System;
	using N2.Persistence.Sources;

	[Service(typeof(IEventsManager))]
	public class EventsManager : IEventsManager
	{
		private readonly ContentSource contentSource;

		public EventsManager(ContentSource contentSource)
		{
			this.contentSource = contentSource;
		}

		#region Events

		public event EventHandler<CancellableItemEventArgs> ItemSaving;
		public event EventHandler<ItemEventArgs> ItemSaved;

		#endregion

		public void ItemSave(ContentItem item, object sender)
		{
			this.InvokeEvent(ItemSaving, item, sender, this.contentSource.Save, ItemSaved);
		}

		/// <summary>Invokes an event and and executes an action unless the event is cancelled.</summary>
		/// <param name="preHandlers">The event handler to signal.</param>
		/// <param name="item">The item affected by this operation.</param>
		/// <param name="sender">The source of the event.</param>
		/// <param name="finalAction">The default action to execute if the event didn't signal cancel.</param>
		/// <param name="postHandlers">The event handler to signal after action completes execution.</param>
		private void InvokeEvent(
			EventHandler<CancellableItemEventArgs> preHandlers, 
			ContentItem item, 
			object sender, 
			Action<ContentItem> finalAction, 
			EventHandler<ItemEventArgs> postHandlers)
		{
			if (preHandlers != null)
			{
				CancellableItemEventArgs args = new CancellableItemEventArgs(item, finalAction);

				preHandlers.Invoke(sender, args);

				if (!args.Cancel)
				{
					args.FinalAction(args.AffectedItem);
					if (postHandlers != null)
					{
						postHandlers(sender, args);
					}
				}
			}
			else
			{
				finalAction(item);

				if (postHandlers != null)
				{
					postHandlers(sender, new ItemEventArgs(item));
				}
			}
		}

		/// <summary>Invokes an event and and executes an action unless the event is cancelled.</summary>
		/// <param name="preHandlers">The event handler to signal.</param>
		/// <param name="source">The item affected by this operation.</param>
		/// <param name="destination">The destination of this operation.</param>
		/// <param name="sender">The source of the event.</param>
		/// <param name="finalAction">The default action to execute if the event didn't signal cancel.</param>
		/// <param name="postHandler">The event handler to signal after action completes execution.</param>
		/// <returns>The result of the action (if any).</returns>
		private ContentItem InvokeEvent(EventHandler<CancellableDestinationEventArgs> preHandlers, object sender, ContentItem source, ContentItem destination, Func<ContentItem, ContentItem, ContentItem> finalAction, EventHandler<DestinationEventArgs> postHandler)
		{
			if (preHandlers != null)
			{
				CancellableDestinationEventArgs args = new CancellableDestinationEventArgs(source, destination, finalAction);

				preHandlers.Invoke(sender, args);

				if (args.Cancel)
					return null;

				args.AffectedItem = args.FinalAction(args.AffectedItem, args.Destination);
				if (postHandler != null)
				{
					postHandler(sender, args);
				}
				return args.AffectedItem;
			}

			var result2 = finalAction(source, destination);
			if (postHandler != null)
			{
				postHandler(sender, new DestinationEventArgs(result2, destination));
			}
			return result2;
		}
	}
}
