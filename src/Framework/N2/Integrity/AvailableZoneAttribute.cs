using System;
using System.Collections.Generic;
using N2.Definitions;
using N2.Collections;

namespace N2.Integrity
{
	/// <summary>
	/// Class decoration that lets N2 know that a class has zones where to 
	/// which data items can be bound (ZoneName).
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class AvailableZoneAttribute : AbstractDefinitionRefiner, IInheritableDefinitionRefiner, IUniquelyNamed
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

		public override void Refine(ItemDefinition definition, IList<ItemDefinition> allDefinitions)
		{
			definition.AvailableZones.AddOrReplace(this);
		}

		#region IUniquelyNamed Members

		string IUniquelyNamed.Name
		{
			get { return ZoneName; }
			set { ZoneName = value; }
		}

		#endregion

		#region INameable Members

		string INameable.Name { get { return ZoneName; } }

		#endregion
	}
}