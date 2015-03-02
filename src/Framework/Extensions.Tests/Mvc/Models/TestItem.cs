namespace N2.Extensions.Tests.Mvc.Models
{
    public class TestItem : ContentItem
    {
        public override bool IsPage
        {
            get { return false; }
        }

        public override string Url
        {
            get { return Parent.Url + "?n2item=" + ID; }
        }
    }
}
