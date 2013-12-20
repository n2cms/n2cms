namespace N2.Tests.Definitions.Static
{
    [PartDefinitionAttribute("My Describable Part", TemplateUrl = "~/My/Template.ascx", IconUrl="/N2/Resources/icons/page_white(description).png")]
    public class DescribablePart : ContentItem
    {
        public override string IconUrl
        {
            get
            {
                return "/N2/Resources/icons/page_white(iconUrl).png";
            }
        }
    }
}
