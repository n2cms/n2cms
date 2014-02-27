using N2.Collections;
using N2.Engine;
using N2.Web.Parts;

namespace N2.Tests.Web.Items
{
    [Adapts(typeof(PageItem))]
    public class PageZoneAdapter : PartsAdapter
    {
        public override System.Collections.Generic.IEnumerable<ContentItem> GetParts(ContentItem belowParentItem, string inZoneNamed, string filteredForInterface)
        {
            if(inZoneNamed.EndsWith("None"))
                return new ItemList();
            if (inZoneNamed.EndsWith("All"))
                return belowParentItem.GetChildren(new DelegateFilter(ci => ci.ZoneName != null));

			return belowParentItem.GetChildren(new ZoneFilter(inZoneNamed), new AccessFilter() /* TODO: remove AccessFilter if it is causing a problem */ );
        }
    }
}
