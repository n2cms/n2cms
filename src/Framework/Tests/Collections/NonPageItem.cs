namespace N2.Tests.Collections
{
    public class NonPageItem : N2.ContentItem
    {
        public override bool IsPage
        {
            get
            {
                return false;
            }
        }
    }
}
