using System;
using System.Collections.Generic;
using N2.Engine;

namespace N2.Definitions.Static
{
	/// <summary>
	/// Provides access to statically available descriptions.
	/// </summary>
	public class DescriptionDictionary
	{
		private static IDictionary<Type, Description> Dictionary
		{
			get { return SingletonDictionary<Type, Description>.Instance; }
		}

		/// <summary>Gets or creates a description from a type. The description for a type is stored throughout the lifetime of the application.</summary>
		/// <param name="type">The type to describe.</param>
		/// <returns>A description associated with the type.</returns>
		public static Description GetDescription(Type type)
		{
			Description typeDescription;
			if (Dictionary.TryGetValue(type, out typeDescription))
				return typeDescription;

			lock(Dictionary)
			{
				if (Dictionary.TryGetValue(type, out typeDescription))
					return typeDescription;
				
				typeDescription = CreateDescription(type);
				Dictionary[type] = typeDescription;
				return typeDescription;
			}
		}

		private static Description CreateDescription(Type type)
		{
			Description d = new Description(type);
			foreach(IDescriptionRefiner refiner in type.GetCustomAttributes(typeof(IDescriptionRefiner), true))
				refiner.Describe(type, d);
			return d;
		}
	}
}
