using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.NH.Finder
{
    public static class FinderExtensions
    {
        public static T EnsureMasterVersion<T>(this T value)
        {
            var item = value as ContentItem;
            if (item != null && item.VersionOf.HasValue)
                value = (T)(object)item.VersionOf.Value;
            return value;
        }
    }
}
