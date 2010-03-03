using System;
using NHibernate;
using NHibernate.Type;
using System.Diagnostics;
using N2.Engine;

namespace N2.Persistence.NH
{
	/// <summary>
	/// This class is used to notify subscribers about loaded items.
	/// </summary>
	[Service(typeof(IItemNotifier))]
	public class NotifyingInterceptor : EmptyInterceptor, IItemNotifier
	{
		/// <summary>Sets rewriter and definition manager on a content item object at load time.</summary>
		/// <param name="entity">The potential content item whose definition manager and rewriter will be set.</param>
		/// <param name="id">Ignored.</param>
		/// <param name="state">Ignored.</param>
		/// <param name="propertyNames">Ignored.</param>
		/// <param name="types">Ignored.</param>
		/// <returns>True if the entity was a content item.</returns>
		public override bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
			return NotifiyCreated(entity as ContentItem);
		}

		public override object Instantiate(string clazz, EntityMode entityMode, object id)
		{
			Debug.WriteLine("Instantiate: " + clazz + " " + entityMode + " " + id);
			return base.Instantiate(clazz, entityMode, id);
		}

		/// <summary>Invokes the notifier's saving event.</summary>
		/// <param name="entity">The potential content item to notify about.</param>
		/// <param name="id">Ignored.</param>
		/// <param name="currentState">Ignored.</param>
		/// <param name="previousState">Ignored.</param>
		/// <param name="propertyNames">Ignored.</param>
		/// <param name="types">Ignored.</param>
		/// <returns>True if the entity was a content item.</returns>
		public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
		{
			return HandleSaveOrUpdate(entity as ContentItem, propertyNames, currentState);
		}

		/// <summary>Invokes the notifier's saving event.</summary>
		/// <param name="entity">The potential content item to notify about.</param>
		/// <param name="id">Ignored.</param>
		/// <param name="state">Ignored.</param>
		/// <param name="propertyNames">Ignored.</param>
		/// <param name="types">Ignored.</param>
		/// <returns>True if the entity was a content item.</returns>
		public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
			return HandleSaveOrUpdate(entity as ContentItem, propertyNames, state);
		}

		private bool HandleSaveOrUpdate(ContentItem item, string[] propertyNames, object[] state)
		{
			if(item != null)
			{
				if (ItemSaving != null)
					ItemSaving(this, new NotifiableItemEventArgs(item));

				for (int i = 0; i < propertyNames.Length; i++)
				{
					if(propertyNames[i] == "AncestralTrail")
					{
						string trail = Utility.GetTrail(item.Parent);
						if(trail != (string)state[i])
						{
							state[i] = trail;
							return true;
						}
					}
				}
			}
			return false;
		}

		#region IItemNotifier Members

		/// <summary>Notify subscribers that an item was loaded or created.</summary>
		/// <param name="newlyCreatedItem">The item that was loaded or created.</param>
		/// <returns>True if the item was modified.</returns>
		public bool NotifiyCreated(ContentItem newlyCreatedItem)
		{
			if(newlyCreatedItem != null && ItemCreated != null)
			{
				NotifiableItemEventArgs e = new NotifiableItemEventArgs(newlyCreatedItem);
				ItemCreated(this, e);
				return e.WasModified;
			}
			return false;
		}

		/// <summary>Is triggered when an item was created or loaded from the database.</summary>
		public event EventHandler<NotifiableItemEventArgs> ItemCreated;

		/// <summary>Is triggered when an item is to be saved the database.</summary>
		public event EventHandler<NotifiableItemEventArgs> ItemSaving;

		#endregion
	}
}
