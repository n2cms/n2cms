using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Engine
{
    public class AttributedType<TAttribute> where TAttribute : class
    {
        public TAttribute Attribute { get; set; }
        public Type Type { get; set; }
    }
}
