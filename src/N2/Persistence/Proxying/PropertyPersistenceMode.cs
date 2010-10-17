using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Proxying
{
	/// <summary>
	/// Instructs the system how to handle a property.
	/// </summary>
	public enum PropertyPersistenceMode
	{
		/// <summary>Auto-implemented virtual properties will be intercepted and stored as details. Other properties are ignored.</summary>
		InterceptedDetails,

		///// <summary>Detail values are retrieved on load and restored before save.</summary>
		//AssignedDetails,
		
		/////// <summary>The property is stored in the item table. This option requires the database schema to be updated.</summary>
		//InTable,
		
		/// <summary>The property is untouched. The getter and setter implementations can choose to store as details.</summary>
		Ignore
	}
}
