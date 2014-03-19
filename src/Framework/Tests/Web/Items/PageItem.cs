using System;
using N2.Collections;
using N2.Web;

namespace N2.Tests.Web.Items
{
    [PageDefinition]
    public class PageItem : ContentItem
    {
        public override string Url
        {
            get { return N2.Web.Url.Parse("/" + Name + Extension).AppendQuery(PathData.VersionIndexQueryKey, VersionOf.HasValue ? VersionIndex.ToString() : null, unlessNull: true); }
        }

        [Obsolete]
        public override string RewrittenUrl
        {
            get { return TemplateUrl.TrimStart('~') + "?n2page=" + ID; }
        }

        public override ItemList GetChildren()
        {
            return GetChildren(new ItemFilter[0]);
        }

        [Obsolete]
		public override ItemList GetChildren(string childZoneName)
        {
            return GetChildren(new ZoneFilter(childZoneName));
        }
    }
}
