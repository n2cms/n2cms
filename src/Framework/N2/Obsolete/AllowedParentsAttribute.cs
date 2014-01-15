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
using System.ComponentModel;

namespace N2.Integrity
{
    /// <summary>
    /// A class decoration used to restrict which items may be placed under 
    /// which. When this attribute intersects with 
    /// <see cref="AllowedChildrenAttribute"/>, the union of these two are 
    /// considered to be allowed.</summary>
    [Obsolete("Name changed to RestrictParentsAttribute."), AttributeUsage(AttributeTargets.Class)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class AllowedParentsAttribute : RestrictParentsAttribute
    {
        /// <summary>Initializes a new instance of the RestrictParentsAttribute which is used to restrict which types of items may be added below which.</summary>
        public AllowedParentsAttribute()
        {
        }

        /// <summary>Initializes a new instance of the RestrictParentsAttribute which is used to restrict which types of items may be added below which.</summary>
        /// <param name="allowedTypes">Defines wether all types of items are allowed as parent items.</param>
        public AllowedParentsAttribute(AllowedTypes allowedTypes)
        {
            if (allowedTypes == AllowedTypes.All)
                Types = null;
            else
                Types = new Type[0];
        }

        /// <summary>Initializes a new instance of the RestrictParentsAttribute which is used to restrict which types of items may be added below which.</summary>
        /// <param name="allowedParentTypes">A list of allowed types. Null is interpreted as all types are allowed.</param>
        public AllowedParentsAttribute(params Type[] allowedParentTypes)
        {
            Types = allowedParentTypes;
        }
    }
}
