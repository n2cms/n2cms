namespace N2.Tests.Definitions.Static
{
    [PageDefinition(IconUrl = "/N2/Resources/icons/page(description).png")]
    public class DescribablePage : ContentItem
    {
        public override string IconUrl
        {
            get { return "/N2/Resources/icons/page(iconUrl).png"; }
        }
    }
}
