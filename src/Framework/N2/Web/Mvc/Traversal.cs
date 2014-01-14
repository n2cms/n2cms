namespace N2.Web.Mvc
{
    public static class Traversal
    {
        /// <summary>Retrieves the closest ancestor that is a page.</summary>
        /// <param name="item">The item whose closest page to get.</param>
        /// <returns>The closest page, either the item itself or the closest ancestor that is a page.</returns>
        public static ContentItem ClosestPage(this ContentItem item)
        {
            if (item == null) return null;
            if (item.IsPage) return item;
            
            return item.Parent.ClosestPage();
        }
    }
}
