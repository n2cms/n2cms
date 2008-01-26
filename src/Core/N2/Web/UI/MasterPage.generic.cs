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
using System.Collections.Generic;
using System.Text;

namespace N2.Web.UI
{
    /// <summary>MasterPage base class providing easy access to current page item.</summary>
    /// <typeparam name="T">The type of content item for this masterpage</typeparam>
    public class MasterPage<TPage> : System.Web.UI.MasterPage, IPageItemContainer where TPage : N2.ContentItem
    {
		public virtual TPage CurrentPage
		{
			get { return (TPage)N2.Context.CurrentPage; }
		}
		public virtual TPage CurrentItem
		{
			get { return CurrentPage; }
		}

		#region IItemContainer Members

		ContentItem IPageItemContainer.CurrentPage
		{
			get { return this.CurrentPage; }
		}
		ContentItem IItemContainer.CurrentItem
		{
			get { return this.CurrentPage; }
		}

		#endregion
    }
}
