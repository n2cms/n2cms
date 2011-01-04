using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence
{
	/// <summary>
	/// Sets a dependency on a loaded entity.
	/// </summary>
	/// <typeparam name="T">The type of dependency to set.</typeparam>
	public interface IDependentEntity<T>
	{
		/// <summary>Assigns the dependency to the enityty.</summary>
		/// <param name="dependency">The dependency to assign.</param>
		void Set(T dependency);
	}
}
