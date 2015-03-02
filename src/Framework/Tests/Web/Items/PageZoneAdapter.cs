using N2.Collections;
using N2.Engine;
using N2.Web.Parts;
using System.Linq;

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
                return belowParentItem.Children.Where(new DelegateFilter(ci => ci.ZoneName != null));

			return belowParentItem.GetChildPartsUnfiltered(inZoneNamed).WhereAccessible(WebContext.User, Security);
        }
    }
}
