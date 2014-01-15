namespace N2.Tests.Collections
{
    public class FirstItem : N2.ContentItem
    {
        /// <summary>Ignores security.</summary>
        public override N2.Collections.ItemList GetChildren()
        {
            return GetChildren(new N2.Collections.ItemFilter[0]);
        }
    }
}
