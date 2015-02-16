namespace N2.Definitions
{
    public class AllowedDefinitionQuery
    {
        public IDefinitionManager Definitions { get; set; }
        public ContentItem Parent { get; set; }
        public ItemDefinition ParentDefinition { get; set; }
        public ContentItem Child { get; set; }
        public ItemDefinition ChildDefinition { get; set; }
    }
}
