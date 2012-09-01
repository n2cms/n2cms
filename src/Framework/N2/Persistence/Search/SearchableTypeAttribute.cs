using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Search
{
	/// <summary>
	/// Marks a class or interface as being searchable by type name. Names of interfaces marked with 
	/// this attribute will be stored in the index.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	public sealed class SearchableTypeAttribute : Attribute
	{
	}
}
