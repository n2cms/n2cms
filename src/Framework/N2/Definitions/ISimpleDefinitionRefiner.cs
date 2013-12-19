namespace N2.Definitions
{
    /// <summary>
    /// Refines a single definition.
    /// </summary>
    public interface ISimpleDefinitionRefiner
    {
        /// <summary>Alters the item definition.</summary>
        /// <param name="currentDefinition">The definition to alter.</param>
        void Refine(ItemDefinition currentDefinition);
    }
}
