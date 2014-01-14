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

namespace N2.Definitions
{
    /// <summary>
    /// This class is used to restrict access to item types in edit mode. Only 
    /// allowed roles can create new items decorated with this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [Obsolete("Use AuthorizedRoles/RequiredPermission property on [(Page/Part)Definition]")]
    public class ItemAuthorizedRolesAttribute : AbstractDefinitionRefiner, IInheritableDefinitionRefiner
    {
        public string[] Roles { get; set; }

        /// <summary>Initializes a new ItemAuthorizedRolesAttribute used to restrict permission to create items in edit mode.</summary>
        public ItemAuthorizedRolesAttribute()
        {
        }

        /// <summary>Initializes a new ItemAuthorizedRolesAttribute used to restrict permission to create items in edit mode.</summary>
        /// <param name="roles">The roles allowed to edit the decorated item.</param>
        public ItemAuthorizedRolesAttribute(params string[] roles)
        {
            Roles = roles;
        }

        public override void Refine(ItemDefinition definition, IList<ItemDefinition> allDefinitions)
        {
            definition.AuthorizedRoles = Roles;
        }
    }
}
