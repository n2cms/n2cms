using System.Linq;
using N2.Collections;
using N2.Templates.Mvc.Models.Pages;
using N2.Web;
using N2.Web.Parts;
using N2.Engine;

namespace N2.Templates.Mvc.Services
{
    /// <summary>
    /// Implements "Recusive" zones functionality.
    /// </summary>
    [Adapts(typeof(PageBase))]
    public class TemplatesPartsAdapter : PartsAdapter
    {
        public override System.Collections.Generic.IEnumerable<ContentItem> GetParts(ContentItem parentItem, string zoneName, string @interface)
        {
            var items =  base.GetParts(parentItem, zoneName, @interface);
            ContentItem grandParentItem = parentItem;
            if (zoneName.StartsWith("Recursive") && grandParentItem is ContentPageBase && !(grandParentItem is LanguageRoot))
            {
                if (!parentItem.VersionOf.HasValue)
                    items = items.Union(GetParts(parentItem.Parent, zoneName, @interface));
                else
                    items = items.Union(GetParts(parentItem.VersionOf.Parent, zoneName, @interface));
            }
            return items;
        }
    }
}
