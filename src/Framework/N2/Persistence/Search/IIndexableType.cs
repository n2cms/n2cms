using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Search
{
    public interface IIndexableType
    {
        bool IsIndexable { get; }
    }
}
