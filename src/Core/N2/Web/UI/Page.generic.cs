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
using System.Web;

namespace N2.Web.UI
{
    /// <summary>Page base class that provides easy access to the current page item.</summary>
	/// <typeparam name="PageT">The type of content item served by the page inheriting this class.</typeparam>
    public abstract class Page<TPage> : System.Web.UI.Page, IPageItemContainer 
        where TPage : N2.ContentItem
    {
		/// <summary>Gets the current CMS Engine.</summary>
		public N2.Engine.IEngine Engine
		{
			get { return N2.Context.Instance; }
		}

		/// <summary>Gets the content item associated with this page.</summary>
        public virtual TPage CurrentPage
        {
            get { return (TPage)N2.Context.CurrentPage; }
        }
		/// <summary>Gets the content item associated with this page.</summary>
		public virtual TPage CurrentItem
		{
			get { return CurrentPage; }
		}

		ContentItem IPageItemContainer.CurrentPage
        {
            get { return this.CurrentPage; }
        }
		ContentItem IItemContainer.CurrentItem
		{
			get { return this.CurrentPage; }
		}
    }
}
