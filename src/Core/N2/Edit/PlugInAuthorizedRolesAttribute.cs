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
using System.Text;

namespace N2.Edit
{
	/// <summary>This class is used to restrict access to plugins in edit mode. When available plugins are displayed to a user only those with the roles specified with this attribute is displayed.</summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class PlugInAuthorizedRolesAttribute : Security.AuthorizedRolesAttribute
	{
		/// <summary>Initializes a new instance of the PlugInAuthorizedRolesAttribute class.</summary>
		public PlugInAuthorizedRolesAttribute()
		{
		}

		/// <summary>Initializes a new instance of the PlugInAuthorizedRolesAttribute class.</summary>
		/// <param name="roles">The roles authorzed to use this plugin.</param>
		/// <remarks>Only plugins displayed can be controlled with this attribute. Users can still access them direcly using the url.</remarks>
		public PlugInAuthorizedRolesAttribute(params string[] roles)
			: base(roles)
		{
		}
	}
}
