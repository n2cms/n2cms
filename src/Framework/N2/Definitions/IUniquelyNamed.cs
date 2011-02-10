using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Definitions
{
	/// <summary>
	/// Classes implementing this interface have a name that must be unique 
	/// within the a certain scope.
	/// </summary>
	public interface IUniquelyNamed
	{
		/// <summary>Gets or sets the name of the prpoerty referenced by this attribute.</summary>
		string Name { get; set; }
	}
}
