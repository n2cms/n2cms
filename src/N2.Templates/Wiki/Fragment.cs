using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Templates.Wiki
{
    public class Fragment
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int StartIndex { get; set; }
        public int Length { get; set; }
        public Fragment Previous { get; set; }
        public Fragment Next { get; set; }
    }
}
