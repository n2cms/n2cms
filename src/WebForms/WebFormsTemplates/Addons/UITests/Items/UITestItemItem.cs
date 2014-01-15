using N2;
using N2.Integrity;
using N2.Web;

namespace N2.Addons.UITests.Items
{
    [Definition("UI Test Item")]
    [Template("~/Addons/UITests/UI/UITestItem.ascx")]
    [RestrictParents(typeof(AdaptiveItemPage))]
    public class UITestItemItem : ContentItem
    {
        public override bool IsPage
        {
            get { return false; }
        }
    }

    [Definition]
    [Template("~/Addons/UITests/UI/ContentCreator.ascx")]
    [AllowedZones(AllowedZones.AllNamed)]
    public class UITestContentCreator : ContentItem
    {
        public override bool IsPage
        {
            get { return false; }
        }
    }
}
