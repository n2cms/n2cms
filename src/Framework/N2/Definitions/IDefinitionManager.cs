#region License
/* Copyright (C) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, write to the Free
 * Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
 * 02110-1301 USA, or see the FSF site: http://www.fsf.org.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace N2.Definitions
{
	/// <summary>Classes implementing this interface are responsible for item definitions used throughout the application to query the properties of a certain item.</summary>
	public interface IDefinitionManager
	{
		/// <summary>Gets the definition for a certain item type.</summary>
		/// <param name="itemType">The type of item whose definition we want.</param>
		/// <returns>The definition matching a certain item type.</returns>
		ItemDefinition GetDefinition(Type itemType);

		/// <summary>Gets item definition for a certain discriminator.</summary>
		/// <param name="discriminator">The discriminator/name that uniquely identifies a certain type.</param>
		/// <returns>The definition matching the string.</returns>
		ItemDefinition GetDefinition(string discriminator);

		/// <summary>Gets the definition for a certain item.</summary>
		/// <param name="item">The item whose definition we want.</param>
		/// <returns>The definition matching a certain item.</returns>
		ItemDefinition GetDefinition(ContentItem item);

		/// <summary>Gets all item definitions.</summary>
		/// <returns>A collection of item definitoins.</returns>
		IEnumerable<ItemDefinition> GetDefinitions();

		/// <summary>Creates an instance of a certain type of item. It's good practice to create new items through this method so the item's dependencies can be injected by the engine.</summary>
		/// <returns>A new instance of an item.</returns>
		[Obsolete("Use ContentActivator", false)]
		T CreateInstance<T>(ContentItem parentItem) where T : ContentItem;

		/// <summary>Creates an instance of a certain type of item. It's good practice to create new items through this method so the item's dependencies can be injected by the engine.</summary>
		/// <returns>A new instance of an item.</returns>
		[Obsolete("Use ContentActivator", false)]
		ContentItem CreateInstance(Type itemType, ContentItem parent);

		/// <summary>Gets a list of children allowed below a certain type of item and zone by a user.</summary>
		/// <param name="parentItem">The parent item whose allowed children to get.</param>
		/// <param name="zoneName">The zone name.</param>
		/// <param name="user">The user to use for filtering by access rights.</param>
		/// <returns>A list of definitions allowed by the given criterias.</returns>
		IList<ItemDefinition> GetAllowedChildren(ContentItem parentItem, string zoneName, IPrincipal user);

		/// <summary>Notifies subscriber that an item was created through a <see cref="CreateInstance"/> method.</summary>
		[Obsolete("Use ContentActivator", false)]
		event EventHandler<ItemEventArgs> ItemCreated;

		/// <summary>Gets all templates for a certain content type.</summary>
		/// <param name="contentType">The type of a content item.</param>
		/// <returns>An enumeration of templates.</returns>
		IEnumerable<TemplateDefinition> GetTemplates(Type contentType);

		/// <summary>Gets a tmeplate by name and content type.</summary>
		/// <param name="contentType">The type of content item.</param>
		/// <param name="templateKey">The name of the template.</param>
		/// <returns>A matching template or null if no template is available.</returns>
		TemplateDefinition GetTemplate(Type contentType, string templateKey);

		/// <summary>Gets the template of a content item.</summary>
		/// <param name="item">The item whose template to get.</param>
		/// <returns>The item's template.</returns>
		TemplateDefinition GetTemplate(ContentItem item);		
	}
}
