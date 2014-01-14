using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions.Static;

namespace N2.Definitions.Runtime
{
    public interface IFluentRegisterer
    {
        Type RegisteredType { get; }
        IEnumerable<ItemDefinition> Register(DefinitionMap map);
    }
}
