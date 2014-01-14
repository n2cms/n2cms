using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Management.Content.Export
{
    public class CsvRow
    {
        public IList<string> Columns { get; set; }
        private char separator;

        public CsvRow(IList<string> columns, char separator)
        {
            this.Columns = columns;
            this.separator = separator;
        }

        public override string ToString()
        {
            return string.Join(separator.ToString(), Columns.ToArray());
        }
    }
}
