using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence
{
	/// <summary>
	/// Instructs the system on where to store a property.
	/// </summary>
	public enum PropertyPersistenceLocation
	{
		/// <summary>Auto-implemented virtual properties will be intercepted and stored as details. Other properties are ignored.</summary>
		Detail,

		///// <summary>Detail values are retrieved on load and restored before save.</summary>
		//AssignedDetail,
		
		/// <summary>The property is stored in the item table. This option requires the database schema to be updated.</summary>
		Column,
		
		/// <summary>The property is untouched. The getter and setter implementations can choose to store as details.</summary>
		Ignore
	}
}
