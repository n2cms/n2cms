using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Collections;
using N2.Edit;

namespace N2.Persistence.Sources
{
    public class Query
    {
        public Query()
        {
            Interface = Interfaces.Viewing;
        }

        public ContentItem Parent { get; set; }
        public string Interface { get; set; }
        public bool? OnlyPages { get; set; }
        public bool SkipAuthorization { get; set; }
        public Range Limit { get; set; }

        public ItemFilter Filter { get; set; }

        public ParameterCollection AsParameters()
        {
            var p = new ParameterCollection(Parameter.Equal("Parent", Parent));

            if (Limit != null)
                p.Range = Limit;

            if (OnlyPages.HasValue)
                return p & (OnlyPages.Value ? Parameter.IsNull("ZoneName") : Parameter.IsNotNull("ZoneName"));

            return p;
        }

        public static Query From(ContentItem parent)
        {
            return new Query { Parent = parent };
        }

        public void Skip(int skip)
        {
            this.Limit = new Range(skip, Limit != null ? Limit.Take : 0);
        }

        public void Take(int take)
        {
            this.Limit = new Range(Limit != null ? Limit.Skip : 0, take);
        }

        public bool IsMatch(ContentItem item)
        {
            if (Parent != null && item.Parent != Parent)
                return false;
            if (OnlyPages.HasValue)
            {
                if (OnlyPages.Value && !item.IsPage)
                    return false;
                else if (!OnlyPages.Value && item.IsPage)
                    return false;
            }
            if (Filter != null && !Filter.Match(item))
                return false;
            return true;
        }
    }
}
