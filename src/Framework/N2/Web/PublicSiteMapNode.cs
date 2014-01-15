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

namespace N2.Web
{
    /// <summary>
    /// A site map node of a N2 site map.
    /// </summary>
    public class PublicSiteMapNode : System.Web.SiteMapNode
    {
        /// <summary>Creates a new instance of the PublicSiteMapNode.</summary>
        /// <param name="provider">The site map provider creating this node.</param>
        /// <param name="item">The content item referenced by this node.</param>
        public PublicSiteMapNode(System.Web.SiteMapProvider provider, ContentItem item) : base (provider, item.ID.ToString(), item.Url, item.Title)
        {
            contentItem = item;

            int roleCount = item.AuthorizedRoles.Count;
            if (roleCount > 0)
            {
                string[] roles = new string[item.AuthorizedRoles.Count];
                for (int i = 0; i < roleCount; i++)
                    roles[i] = item.AuthorizedRoles[i].Role;
                base.Roles = roles;
            }
        }

        private ContentItem contentItem;

        /// <summary>Gets the content item referenced by this node.</summary>
        public ContentItem ContentItem
        {
            get { return contentItem; }
        }
    }
}
