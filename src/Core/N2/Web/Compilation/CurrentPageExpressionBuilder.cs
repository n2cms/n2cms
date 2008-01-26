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
using System.CodeDom;
using System.Web.UI;
using System.ComponentModel;
using System.Web.Compilation;

namespace N2.Web.Compilation
{
    /// <summary>Expression builder used for initializing web controls with the current page's detils or properties with the expression syntax. This is a convenient to bind content data to controls on a template. The expression is evaluated at compile time.</summary>
    /// <example>
    /// <asp:Label text="&lt;%$ CurrentPage: Title %&gt;" runat="server" />
    /// </example>
    [ExpressionPrefix("CurrentPage")]
	public class CurrentPageExpressionBuilder : N2ExpressionBuilder
	{
		public static object GetCurrentPageValue(string expression)
		{
			return N2.Context.CurrentPage[expression.Trim()];
		}

		protected override string ExpressionFormat
		{
			get { return @"N2.Web.Compilation.CurrentPageExpressionBuilder.GetCurrentPageValue(""{0}"")"; }
		}
    } 
}