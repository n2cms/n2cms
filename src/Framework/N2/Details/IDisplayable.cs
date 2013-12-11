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

using System.Web.UI;
using N2.Definitions;

namespace N2.Details
{
    /// <summary>
    /// Classes implementing this interface defines a control used to display 
    /// the value of a content detail.
    /// </summary>
    public interface IDisplayable : IUniquelyNamed
    {
        /// <summary>Creates, initializes adds and returns the displayer.</summary>
        /// <param name="item">The item from which to get it's value.</param>
        /// <param name="detailName"></param>
        /// <param name="container">The container onto which to add the displayer.</param>
        /// <returns>The displayer control that was added.</returns>
        Control AddTo(ContentItem item, string detailName, Control container);
    }
}
