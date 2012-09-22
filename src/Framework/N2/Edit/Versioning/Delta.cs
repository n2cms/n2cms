using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Edit.Versioning
{
    public class Delta
    {
        public string Name { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }
}
