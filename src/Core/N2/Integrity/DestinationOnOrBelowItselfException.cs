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

namespace N2.Integrity
{
	/// <summary>
	/// Exception thrown when an attempt to move an item onto or below itself is made.
	/// </summary>
	public class DestinationOnOrBelowItselfException : N2Exception
	{
		public DestinationOnOrBelowItselfException(ContentItem source, ContentItem destination)
			: base("Cannot move item to a destination onto or below itself.")
		{
			this.sourceItem = source;
			this.destinationItem = destination;
		}

		private ContentItem sourceItem;
		private ContentItem destinationItem;

		/// <summary>Gets the source item that is causing the conflict.</summary>
		public ContentItem SourceItem
		{
			get { return sourceItem; }
		}

		/// <summary>Gets the parent item already containing an item with the same name.</summary>
		public ContentItem DestinationItem
		{
			get { return destinationItem; }
		}

	}
}
