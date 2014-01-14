using N2.Collections;
namespace N2.Definitions
{
    /// <summary>
    /// Classes implementing this interface have a name that must be unique 
    /// within the a certain scope.
    /// </summary>
    public interface IUniquelyNamed : INameable
    {
        /// <summary>Gets or sets the name of the prpoerty referenced by this attribute.</summary>
        new string Name { get; set; }
    }
}
