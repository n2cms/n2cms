#region License
/* Copyright (C) 2006 Cristian Libardo
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 */
#endregion

using System;
using System.Collections.Generic;
using N2.Persistence;

namespace N2.Definitions
{
	/// <summary>
	/// Stores item definitions and constructs new items.
	/// </summary>
	public class DefaultDefinitionManager : IDefinitionManager
	{
		#region Fields
		private readonly IDictionary<Type, ItemDefinition> definitions;
		readonly IItemNotifier notifier;
		#endregion 

		#region Constructors (1)
		public DefaultDefinitionManager(DefinitionBuilder builder, IItemNotifier notifier)
		{
			definitions = builder.GetDefinitions();
			this.notifier = notifier;
		}
		#endregion 

		#region Methods (10)
		/// <summary>Creates an instance of a certain type of item. It's good practice to create new items through this method so the item's dependencies can be injected by the engine.</summary>
		/// <returns>A new instance of an item.</returns>
		public T CreateInstance<T>(ContentItem parentItem) where T : ContentItem
		{
			T item = Activator.CreateInstance<T>();
			OnItemCreating(item, parentItem);
			return item;
		}

		/// <summary>Creates an instance of a certain type of item. It's good practice to create new items through this method so the item's dependencies can be injected by the engine.</summary>
		/// <returns>A new instance of an item.</returns>
		public ContentItem CreateInstance(Type itemType, ContentItem parentItem)
		{
			ContentItem item = Activator.CreateInstance(itemType) as ContentItem;
			OnItemCreating(item, parentItem);
			return item;
		}

		protected virtual void OnItemCreating(ContentItem item, ContentItem parentItem)
		{
			if(parentItem != null)
			{
				ItemDefinition parentDefinition = GetDefinition(parentItem.GetType());
				ItemDefinition itemDefinition = GetDefinition(item.GetType());

				if(!parentDefinition.IsChildAllowed(itemDefinition))
					throw new NotAllowedParentException(itemDefinition, parentItem.GetType());

				item.Parent = parentItem;
			}
			notifier.Notifiy(item);
		}

		/// <summary>Gets the definition for a certain item type.</summary>
		/// <param name="itemType">The type of item whose definition we want.</param>
		/// <returns>The definition matching a certain item type.</returns>
		public virtual ItemDefinition GetDefinition(Type itemType)
		{
			if(definitions.ContainsKey(itemType))
				return definitions[itemType];
			else
				return null;
		}

		/// <summary>Gets item definition for a certain discriminator.</summary>
		/// <param name="discriminator">The discriminator/name that uniquely identifies a certain type.</param>
		/// <returns>The definition matching the string.</returns>
		public ItemDefinition GetDefinition(string discriminator)
		{
			foreach(ItemDefinition definition in definitions.Values)
				if(definition.Discriminator == discriminator)
					return definition;
			return null;
		}

		/// <summary>Gets all item definitions.</summary>
		/// <returns>A collection of item definitoins.</returns>
		public virtual ICollection<ItemDefinition> GetDefinitions()
		{
			return definitions.Values;
		}
		#endregion
	}
}
