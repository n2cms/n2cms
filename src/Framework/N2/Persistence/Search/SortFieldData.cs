using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Search
{
    public class SortFieldData
    {
        public string SortField { get; set; }
        public bool SortDescending { get; set; }

        public SortFieldData(string field)
        {
            SortField = field;
            SortDescending = true;
        }

        public SortFieldData(string field, bool sortDescending)
        {
            SortField = field;
            SortDescending = sortDescending;
        }
    }
}
