namespace N2.Definitions
{
    /// <summary>
    /// Hints whether the definition should be removed or just disabled. Removing 
    /// it might cause error if there is already created items, e.g. lying around
    /// in the trash but allows for replacing the definition with a new one
    /// that overrides already created items (they need to use the same discriminator).
    /// </summary>
    public enum DefinitionReplacementMode
    {
        Disable,
        Remove
    }
}
