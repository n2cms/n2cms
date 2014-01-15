using System.Collections.Generic;

namespace N2.Collections
{
    /// <summary>
    /// Filters based on the <see cref="N2.ContentItem.ZoneName"/> property.
    /// </summary>
    public class ZoneFilter : ItemFilter
    {
        public ZoneFilter(string zoneName)
        {
            this.zoneName = zoneName;
        }

        private string zoneName;
        public string ZoneName
        {
            get { return zoneName; }
            set { zoneName = value; }
        }

        public override bool Match(ContentItem item)
        {
            return item.ZoneName == ZoneName;
        }

        public static void Filter(IList<ContentItem> items, string zoneName)
        {
            ItemFilter.Filter(items, new ZoneFilter(zoneName));
        }

        public override string ToString()
        {
            return "Zone=" + zoneName;
        }
    }
}
