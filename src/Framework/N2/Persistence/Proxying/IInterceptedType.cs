using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Proxying
{
	/// <summary>
	/// Assigned to proxied objects.
	/// </summary>
	public interface IInterceptedType
	{
		/// <summary>The type name of the intercepted type.</summary>
		/// <returns>The type name.</returns>
		string GetTypeName();
	}
}
