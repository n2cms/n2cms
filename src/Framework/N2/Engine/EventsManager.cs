
namespace N2.Engine
{
	using System;
	using System.Collections.Generic;
	using N2.Persistence;

	[Service]
	[Service(typeof(IEventsManager))]
	public class EventsManager : IEventsManager
	{
		#region Events

		public event EventHandler<CancellableItemEventArgs> ItemSaving;
		public event EventHandler<ItemEventArgs> ItemSaved;

		#endregion

		public void TriggerItemSave(
			EventHandler<CancellableItemEventArgs> legacyPreHandler,
			ContentItem item, 
			Action<ContentItem> finalAction,
			EventHandler<ItemEventArgs> legacyPostHandler,
			ITransaction commitTriggersPostEvent = null)
		{
			var preHandlers = new List<EventHandler<CancellableItemEventArgs>> { this.ItemSaving };
			if (legacyPreHandler != null) preHandlers.Add(legacyPreHandler);

			var postHandlers = new List<EventHandler<ItemEventArgs>> { this.ItemSaved };
			if (legacyPostHandler != null) postHandlers.Add(legacyPostHandler);

			this.InvokeEvent(preHandlers, item, finalAction, postHandlers, commitTriggersPostEvent);
		}

		/// <summary>Invokes an event and executes an action unless the event is cancelled</summary>
		/// <param name="preHandlers">Event handlers to signal</param>
		/// <param name="item">Item affected by this operation</param>
		/// <param name="finalAction">Default action to execute if the event didn't signal cancel</param>
		/// <param name="postHandlers">Event handlers to signal after action completes execution</param>
		/// <param name="tx">Transaction</param>
		private void InvokeEvent(
			IEnumerable<EventHandler<CancellableItemEventArgs>> preHandlers, 
			ContentItem item, 
			Action<ContentItem> finalAction, 
			IEnumerable<EventHandler<ItemEventArgs>> postHandlers,
			ITransaction tx)
		{
			var args = new CancellableItemEventArgs(item, finalAction);

			InvokeEventHandlers(preHandlers, args);

			if (args.Cancel)
				return;

			args.FinalAction(args.AffectedItem);

			if (postHandlers != null)
			{
				if (tx != null)
				{
					// delaying post-event until commit is neccessary for some reason (probably id-generation and/or cascaded entities)
					tx.Committed += (s, e) => InvokeEventHandlers(postHandlers, args);				
				}
				else
				{
					InvokeEventHandlers(postHandlers, args);
				}
			}
		}

		private void InvokeEventHandlers(
			IEnumerable<EventHandler<CancellableItemEventArgs>> handlers,
			CancellableItemEventArgs args)
		{
			if (handlers != null)
			{
				foreach (var handler in handlers)
				{
					if (handler != null)
						handler.Invoke(this, args);
				}
			}
		}

		private void InvokeEventHandlers(
			IEnumerable<EventHandler<ItemEventArgs>> handlers,
			CancellableItemEventArgs args)
		{
			if (handlers != null)
			{
				foreach (var handler in handlers)
				{
					if (handler != null)
						handler.Invoke(this, args);
				}
			}
		}

		/*
		/// <summary>Invokes an event and and executes an action unless the event is cancelled.</summary>
		/// <param name="preHandlers">The event handler to signal.</param>
		/// <param name="source">The item affected by this operation.</param>
		/// <param name="destination">The destination of this operation.</param>
		/// <param name="sender">The source of the event.</param>
		/// <param name="finalAction">The default action to execute if the event didn't signal cancel.</param>
		/// <param name="postHandler">The event handler to signal after action completes execution.</param>
		/// <returns>The result of the action (if any).</returns>
		private ContentItem InvokeEvent(EventHandler<CancellableDestinationEventArgs> preHandlers, ContentItem source, ContentItem destination, Func<ContentItem, ContentItem, ContentItem> finalAction, EventHandler<DestinationEventArgs> postHandler)
		{
			if (preHandlers != null)
			{
				CancellableDestinationEventArgs args = new CancellableDestinationEventArgs(source, destination, finalAction);

				preHandlers.Invoke(this, args);

				if (args.Cancel)
					return null;

				args.AffectedItem = args.FinalAction(args.AffectedItem, args.Destination);
				if (postHandler != null)
				{
					postHandler(this, args);
				}
				return args.AffectedItem;
			}

			var result2 = finalAction(source, destination);
			if (postHandler != null)
			{
				postHandler(this, new DestinationEventArgs(result2, destination));
			}
			return result2;
		}
		*/
	}
}
