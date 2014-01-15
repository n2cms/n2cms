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

namespace N2
{
    /// <summary>
    /// Event argument containing item and destination item.
    /// </summary>
    public class DestinationEventArgs : ItemEventArgs
    {
        private ContentItem destination;

        /// <summary>Creates a new instance of the DestinationEventArgs.</summary>
        /// <param name="affectedItem">The item associated with these arguments.</param>
        /// <param name="destination">The destination for the event with these arguments.</param>
        public DestinationEventArgs(ContentItem affectedItem, ContentItem destination)
            : base(affectedItem)
        {
            this.destination = destination;
        }

        /// <summary>Gets the destination for the event with these arguments.</summary>
        public ContentItem Destination
        {
            get { return destination; }
        }   
    }
}
