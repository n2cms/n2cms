namespace N2.Tests.Web.Items
{
    public class CustomExtensionItem : PageItem
    {
        public string extension = ".aspx";
        public override string Extension
        {
            get { return extension; }
        }
    }
}
