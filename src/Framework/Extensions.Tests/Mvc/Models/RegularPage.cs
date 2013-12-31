namespace N2.Extensions.Tests.Mvc.Models
{
    [PageDefinition]
    public class RegularPage : ContentItem
    {
        public override bool IsPage
        {
            get { return true; }
        }

        public override string Url
        {
            get { return Parent != null ? Parent.Url + "/" + Name : "/"; }
        }
    }
}
