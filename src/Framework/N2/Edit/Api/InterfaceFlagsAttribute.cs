using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Management.Api
{
    public class InterfaceFlagsAttribute : Attribute
    {
        public InterfaceFlagsAttribute(params string[] additionalFlags)
        {
            AdditionalFlags = additionalFlags;
        }

        public string[] AdditionalFlags { get; private set; }
        public string[] RemovedFlags { get; set; }
    }
}
