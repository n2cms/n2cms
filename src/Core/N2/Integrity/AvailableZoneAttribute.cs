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
using N2.Definitions;

namespace N2.Integrity
{
	/// <summary>
	/// Class decoration that lets N2 know that a class has zones where to 
	/// which data items can be bound (ZoneName).
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class AvailableZoneAttribute : Attribute, IInheritableDefinitionRefiner
	{
		public AvailableZoneAttribute(string title, string zoneName)
		{
			this.title = title;
			this.zoneName = zoneName;
		}

		#region Private Members

		private string zoneName;
		private string title;

		#endregion

		#region Properties
		/// <summary>Gets or sets the name of the zone in question.</summary>
		public string ZoneName
		{
			get { return zoneName; }
			set { zoneName = value; }
		}

		/// <summary>Gets or sets the title displayed to editors.</summary>
		public string Title
		{
			get { return title; }
			set { title = value; }
		}

		#endregion

		#region Equals & GetHashCode

		public override bool Equals(object obj)
		{
			if (obj is AvailableZoneAttribute)
				return zoneName.Equals(((AvailableZoneAttribute) obj).ZoneName);
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return zoneName.GetHashCode();
		}

		#endregion

		public void Refine(ItemDefinition definition, IList<ItemDefinition> allDefinitions)
		{
			definition.AvailableZones.Add(this);
		}
	}
}