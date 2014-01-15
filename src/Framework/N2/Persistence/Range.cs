using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence
{
    public class Range
    {
        public Range(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}
