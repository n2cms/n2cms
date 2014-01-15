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

namespace N2.Details
{
    /// <summary>This class is used to restrict access to certain details in edit mode. Only users in the specified roles can edit details decorated with this attribute.</summary>
    [AttributeUsage(AttributeTargets.Property)]
    [Obsolete("Use AuthorizedRoles/RequiredPermission property on Editable instead.", true)]    
    public class DetailAuthorizedRolesAttribute : Attribute
    {
        public string[] Roles { get; set; }

        /// <summary>Initializes a new instance of the DetailAuthorizedRolesAttribute used to restrict access to details in edit mode.</summary>
        public DetailAuthorizedRolesAttribute()
        {
        }

        /// <summary>Initializes a new instance of the DetailAuthorizedRolesAttribute used to restrict access to details in edit mode.</summary>
        /// <param name="roles">The roles allowed to edit the decorated detail.</param>
        public DetailAuthorizedRolesAttribute(params string[] roles)
        {
            Roles = roles;
        }
    }
}
