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
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace N2.Edit.Web
{
    public class EditSiteMapNode : SiteMapNode
    {
        public EditSiteMapNode(SiteMapProvider provider, ContentItem item)
            : base(provider, item.ID.ToString())
        {
            this.CurrentItem = item;
        }

        public EditSiteMapNode(SiteMapProvider provider, string url)
			: this(provider, Engine.UrlParser.Parse(url))
        {
        }

        private ContentItem currentItem;
        public ContentItem CurrentItem
        {
            get { return currentItem; }
			set
			{
				this.currentItem = value;
				if (value != null)
				{
					this.Url = value.RewrittenUrl;
					this.Title = value.Title;
					this.Description = value.ID + ", " + value.Name
						+ " (" + value.Published + " - " + (value.Expires.HasValue ? value.Expires.ToString() : "") + ") "
						+ Engine.Definitions.GetDefinition(value.GetType()).Title;
				}
				else
				{
					this.Url = null;
					this.Title = null;
					this.Description = null;
				}
			}
        }

		protected static Engine.IEngine Engine
		{
			get { return N2.Context.Current; }
		}
    }
}
