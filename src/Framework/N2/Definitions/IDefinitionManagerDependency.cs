namespace N2.Definitions
{
    /// <summary>An interface used by N2 to inject content item's dependency to the definition manager (needed by the definition property).</summary>
    public interface IDefinitionManagerDependency
    {
        /// <summary>Sets the definition on the object.</summary>
        /// <param name="definitions">The definition manager to inject.</param>
        void SetDefinitions(IDefinitionManager definitions);
    }
}
