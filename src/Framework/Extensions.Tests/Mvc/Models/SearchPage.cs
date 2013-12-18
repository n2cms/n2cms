namespace N2.Extensions.Tests.Mvc.Models
{
    public class SearchPage : ContentItem
    {
        public override string Url
        {
            get { return Parent != null ? Parent.Url + "/" + Name : "/"; }
        }
    }
}
