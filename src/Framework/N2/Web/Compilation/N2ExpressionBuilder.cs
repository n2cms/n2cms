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

using System.CodeDom;
using System.Web.Compilation;
using System.Web.UI;

namespace N2.Web.Compilation
{
    /// <summary>The base class for N2 expression builders. Defines methods to compile time create code expressions.</summary>
    public abstract class N2ExpressionBuilder : ExpressionBuilder
    {
        /// <summary>The expression format base classes can override.</summary>
        protected abstract string ExpressionFormat
        {
            get;
        }

        /// <summary>Gets the code expresion evaluated at page compile time.</summary>
        public override CodeExpression GetCodeExpression(BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        {
            return new CodeSnippetExpression(string.Format(ExpressionFormat, entry.Expression, entry.DeclaringType));
        }
    }
}
