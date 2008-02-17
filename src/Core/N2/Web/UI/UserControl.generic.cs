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
using System.Web.UI;

namespace N2.Web.UI
{
	/// <summary>A user control base used to for quick access to content data.</summary>
	/// <typeparam name="TPage">The type of page item this user control will have to deal with.</typeparam>
	public class UserControl<TPage> : UserControl, IPageItemContainer
		where TPage : N2.ContentItem
	{
		/// <summary>Gets the current CMS Engine.</summary>
		public N2.Engine.IEngine Engine
		{
			get { return N2.Context.Instance; }
		}

		private TPage currentPage = null;
		/// <summary>Gets the current page item.</summary>
		public virtual TPage CurrentPage
		{
			get 
			{
				if (currentPage == null)
					currentPage = (TPage)ItemUtility.FindCurrentItem(this.Parent) ?? (TPage)N2.Context.CurrentPage;
				return (TPage)N2.Context.CurrentPage; 
			}
		}

		/// <summary>Gets the current page item.</summary>
		public TPage CurrentItem
		{
			get { return this.CurrentPage; }
		}
	
		ContentItem IPageItemContainer.CurrentPage
		{
			get { return this.CurrentPage; }
		}

		ContentItem IItemContainer.CurrentItem
		{
			get { return this.CurrentItem; }
		}
	}
}
