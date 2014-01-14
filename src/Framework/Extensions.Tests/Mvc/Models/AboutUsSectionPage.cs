namespace N2.Extensions.Tests.Mvc.Models
{


    [PageDefinition]
    public class AboutUsSectionPage : ContentItem
    {
        public override string Url
        {
            get { return Parent != null ? Parent.Url + "/" + Name : "/"; }
        }
    }
}
