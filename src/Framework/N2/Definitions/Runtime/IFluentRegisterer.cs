using System;
using System.Collections.Generic;
using N2.Definitions.Static;

namespace N2.Definitions.Runtime
{
	public interface IFluentRegisterer
	{
		Type RegisteredType { get; }
		IEnumerable<ItemDefinition> Register(DefinitionMap map);
	}
}
