using System;
using System.Collections.Generic;
using N2.Definitions;

namespace N2.Persistence.Proxying
{
	/// <summary>
	/// Creates a proxy that rewires auto-generated properties to detail get/set.
	/// </summary>
	public interface IProxyFactory
	{
		/// <summary>Initializes the proxy factory with the given types.</summary>
		/// <param name="interceptedTypes"></param>
		void Initialize(IEnumerable<ItemDefinition> interceptedTypes);
		
		/// <summary>Creates a proxied instance.</summary>
		/// <param name="typeName">The name of the type to create.</param>
		/// <param name="id">The instance identifyer.</param>
		/// <returns>A proxy intercepting access to certain properties.</returns>
		object Create(string typeName, object id);

		/// <summary>Optional modifications of a loaded instance.</summary>
		/// <param name="instance">The instance to modify.</param>
		bool OnLoaded(object instance);
		
		/// <summary>Optional modifications of a instance to be saved.</summary>
		/// <param name="instance">The instance to modify.</param>
		bool OnSaving(object instance);

		/// <summary>Gets the type name of a proxied instance.</summary>
		/// <param name="instance">The proxied instance.</param>
		/// <returns>The instance's full name.</returns>
		string GetTypeName(object instance);
	}
}
