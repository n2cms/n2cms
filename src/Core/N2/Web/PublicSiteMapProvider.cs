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
using System.Web;
using System.Collections.Generic;
using System.Text;
using N2.Collections;

namespace N2.Web
{
    /// <summary>
	/// A site map provider implementation for N2 CMS content pages.
	/// </summary>
    public class PublicSiteMapProvider : SiteMapProvider
    {
		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection attributes)
		{
			if (attributes["securityTrimmingEnabled"] == null)
				attributes["securityTrimmingEnabled"] = "true";

			base.Initialize(name, attributes);
		}

        protected virtual SiteMapNode Convert(ContentItem item)
        {
            if (item != null)
                return new PublicSiteMapNode(this, item);
            return null;
        }

        public override SiteMapNode FindSiteMapNode(string rawUrl)
        {
            if (string.IsNullOrEmpty(rawUrl)) throw new ArgumentNullException("rawUrl");

            // If the first letter of the url is a number then the rawUrl probably is
            // the key of a previously generated SiteMapNode. This is an odd behaviour 
            // of the site map provider model
            ContentItem item = (rawUrl[0]>'0' && rawUrl[0]<='9') ?
                Context.Persister.Get(int.Parse(rawUrl)) :
                Context.UrlParser.Parse(rawUrl);

            return Convert(item);
        }

        public override SiteMapNodeCollection GetChildNodes(SiteMapNode node)
        {
            SiteMapNodeCollection nodes = new SiteMapNodeCollection();
			ContentItem item = (node != null) ? Context.Persister.Get(int.Parse(node.Key)) : null; 
			
            // Add published nodes that are pages
			if (item != null)
			{
				IEnumerable<ItemFilter> filters = GetFilters();

				foreach (ContentItem child in item.GetChildren(filters))
					nodes.Add(Convert(child));
			}

            return nodes;
        }

		protected virtual IEnumerable<ItemFilter> GetFilters()
		{
			IList<ItemFilter> filters = new List<ItemFilter>();
			filters.Add(new PageFilter());
			filters.Add(new VisibleFilter());
			filters.Add(new PublishedFilter());
			if (SecurityTrimmingEnabled)
				filters.Add(new AccessFilter());
			return filters;
		}

        public override SiteMapNode GetParentNode(SiteMapNode node)
        {
            ContentItem item = Context.Persister.Get(int.Parse(node.Key));
            if(item != null && item.Parent != null && !Context.UrlParser.IsRootOrStartPage(item))
                return Convert((ContentItem)item.Parent);
            else
                return null;
        }

        protected override SiteMapNode GetRootNodeCore()
        {
			return Convert(Context.UrlParser.StartPage);
        }
    }
}
