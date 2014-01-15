using N2.Integrity;

namespace N2.Addons.Tagging.Items
{
    [PartDefinition("Tag Box",
        IconUrl = "~/Addons/Tagging/UI/tag_yellow.png", 
        TemplateUrl = "~/Addons/Tagging/UI/TagBox.ascx")]
    [AllowedZones(AllowedZones.AllNamed)]
    public class TagBox : ContentItem
    {
    }
}
