using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace N2.Engine
{
    public class ServiceInfo
    {
        public string Key { get; set; }
        public Type ServiceType { get; set; }
        public IEnumerable<Type> ServiceTypes { get; set; }
        public Type ImplementationType { get; set; }
        public Func<object> Resolve { get; set; }
        public Func<IEnumerable> ResolveAll { get; set; }
    }
}
