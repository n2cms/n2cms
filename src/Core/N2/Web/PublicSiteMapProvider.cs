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

namespace N2.Web
{
    /// <summary>
	/// Default site map provider for N2 CMS.
	/// </summary>
    public class PublicSiteMapProvider : SiteMapProvider
    {
        protected virtual SiteMapNode Convert(ContentItem item)
        {
            if (item != null)
                return new PublicSiteMapNode(this, item);
            return null;
        }

        protected virtual bool IsDisplayable(ContentItem item)
        {
            return item.IsPage // display only pages...
                && item.Visible // ... that are visible...
                && item.Published < DateTime.Now // ...and are published...
                && (!item.Expires.HasValue || item.Expires > DateTime.Now); // ...and arn't expired.
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
			if(item != null)
				foreach (ContentItem child in item.GetChildren())
					if(IsDisplayable(child))
						nodes.Add(Convert(child));

            return nodes;
        }

        public override SiteMapNode GetParentNode(SiteMapNode node)
        {
            ContentItem item = Context.Persister.Get(int.Parse(node.Key));
            if(item != null && item.Parent != null && !Context.UrlParser.IsRootOrStartpage(item))
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
