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
using System.Web.Compilation;

namespace N2.Web.Compilation
{
    /// <summary>Expression builder used for initializing web controls with the current item's detils or properties with the expression syntax. This is a convenient to bind content data to controls on a template. The expression is evaluated at compile time.</summary>
    /// <example>
    /// &lt;asp:Label text="&lt;%$ CurrentItem: Title %&gt;" runat="server" /&gt;
    /// </example>
    [ExpressionPrefix("CurrentItem")]
    public class CurrentItemExpressionBuilder : N2ExpressionBuilder
    {
        /// <summary>The item associated with the currently created control.</summary>
        private static ContentItem CurrentItemInContext
        {
            get { return UI.ItemUtility.CurrentContentItem; }
        }

        /// <summary>Gets the value of an expression.</summary>
        /// <param name="expression">The expression whose value to get.</param>
        /// <returns>The value a given expression.</returns>
        public static object GetCurrentItemValue(string expression)
        {
	        if (UI.ItemUtility.ItemStack.Count > 0)
	        {
		        var item = CurrentItemInContext;
		        if (item != null)
			        return item[expression];
	        }
	        return Context.CurrentPage[expression];
        }

        /// <summary>Gets the expression format for this expression.</summary>
        protected override string ExpressionFormat
        {
            get { return @"N2.Web.Compilation.CurrentItemExpressionBuilder.GetCurrentItemValue(""{0}"")"; }
        }
    } 
}
