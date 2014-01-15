namespace N2.Definitions
{
    /// <summary>
    /// Attributes implementing this interface can alter item definitions while
    /// they are beeing initiated. All classes in the inheritance chain are 
    /// queried for this interface when refining the definition.
    /// </summary>
    public interface IInheritableDefinitionRefiner : ISortableRefiner
    {
    }
}
