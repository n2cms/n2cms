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
using N2.Definitions;

namespace N2.Integrity
{
    /// <summary>
    /// A class decoration used to define which items are allowed below this 
    /// item. When this attribute intersects with 
    /// <see cref="RestrictParentsAttribute"/>, the union of these two are 
    /// considered to be allowed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AllowedChildrenAttribute : TypeIntegrityAttribute, IInheritableDefinitionRefiner, IAllowedDefinitionFilter
    {
        /// <summary>Initializes a new instance of the AllowedChildrenAttribute which is used to restrict which types of items may be added below which.</summary>
        public AllowedChildrenAttribute()
        {
            RefinementOrder = RefineOrder.After;
        }

        /// <summary>Initializes a new instance of the AllowedChildrenAttribute which is used to restrict which types of items may be added below which.</summary>
        /// <param name="allowedChildTypes">A list of allowed types. Null is interpreted as all types are allowed.</param>
        public AllowedChildrenAttribute(params Type[] allowedChildTypes)
            : this()
        {
            Types = allowedChildTypes;
        }

        public override void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions)
        {
            currentDefinition.AllowedChildFilters.Add(this);
        }

        #region IAllowedDefinitionFilter Members

        public AllowedDefinitionResult IsAllowed(AllowedDefinitionQuery context)
        {
            return IsAssignable(context.ChildDefinition.ItemType) ? AllowedDefinitionResult.Allow : AllowedDefinitionResult.DontCare;
        }

        #endregion
    }
}
