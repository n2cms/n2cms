namespace N2.Collections
{
    /// <summary>
    /// Filters non-visible items.
    /// </summary>
    public class VisibleFilter : ItemFilter
    {
        public override bool Match(ContentItem item)
        {
            return item.Visible;
        }

        public override string ToString()
        {
            return "IsVisible";
        }
    }
}
