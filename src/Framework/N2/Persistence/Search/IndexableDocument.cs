using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace N2.Persistence.Search
{
    public class IndexableField
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Analyzed { get; set; }
        public bool Stored { get; set; }
    }

    public class IndexableDocument
    {
        public IndexableDocument()
        {
            Values = new List<IndexableField>();
        }

        public int ID { get; set; }
        public ICollection<IndexableField> Values { get; set; }

        public void Add(string name, string value, bool store, bool analyze)
        {
            Values.Add(new IndexableField { Name = name, Value = value ?? "", Stored = store, Analyzed = analyze });
        }

        public void Add(string name, DateTime? value, bool store, bool analyze)
        {
            Values.Add(new IndexableField { Name = name, Value = value.HasValue ? value.Value.ToUniversalTime().ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) : "", Stored = store, Analyzed = analyze });
        }

        public void Add(string name, int? value, bool store, bool analyze)
        {
            Add(name, value.HasValue ? value.Value.ToString() : "", store, analyze);
        }
    }
}
