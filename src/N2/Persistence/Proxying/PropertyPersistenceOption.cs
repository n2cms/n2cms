using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Proxying
{
	/// <summary>
	/// Instructs the system how to handle a property.
	/// </summary>
	public enum PropertyPersistenceOption
	{
		/// <summary>Auto-implemented properties will be intercepted and stored as details. Other properties are ignored.</summary>
		Default,
		
		/// <summary>Acess to the property is intercepted and the value is stored as a content detail in a separate table.</summary>
		Detail,
		
		///// <summary>The property is stored in the item table. This option requires the database schema to be updated.</summary>
		//ItemProperty,
		
		/// <summary>The property is untouched. The getter and setter implementations can choose to store as details.</summary>
		Ignore
	}
}
