using N2.Integrity;

namespace N2.Addons.UITests.Items
{
    [RestrictParents(typeof(AdaptiveItemPage))]
    public class AbstractItem : ContentItem
    {
        public override string TemplateUrl
        {
            get { return "~/Addons/UITests/UI/Item.ascx"; }
        }
        public string TypeName
        {
            get { return GetType().Name; }
        }
    }
}
