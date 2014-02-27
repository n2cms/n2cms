namespace N2.Collections
{
    /// <summary>
    /// Creates a hierarchy without any items.
    /// </summary>
    public class NoHierarchyBuilder : HierarchyBuilder
    {
        public override HierarchyNode<ContentItem> Build()
        {
            return new HierarchyNode<ContentItem>(null);
        }
    }
}
