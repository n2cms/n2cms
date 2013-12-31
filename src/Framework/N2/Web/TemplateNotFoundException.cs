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
    /// Exception thrown when a template associated with a content is not 
    /// found.
    /// </summary>
    public class TemplateNotFoundException : N2Exception
    {
        /// <summary>Creates a new instance of the TemplateNotFoundException.</summary>
        /// <param name="item">The item whose templates wasn't found.</param>
        public TemplateNotFoundException(ContentItem item)
            : base("Item template not found, id:{0}, template:{1}", item.ID, item.TemplateUrl)
        {
            this.item = item;
        }

        private ContentItem item;
        /// <summary>Gets the content item associated with this exception.</summary>
        public ContentItem Item
        {
            get { return item; }
        }
    }
}
