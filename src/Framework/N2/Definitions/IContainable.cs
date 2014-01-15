using System;
using System.Web.UI;
using N2.Security;

namespace N2.Definitions
{
    /// <summary>
    /// Classes implementing this interface can add a graphical representation to 
    /// a control hierarchy.
    /// </summary>
    public interface IContainable : IUniquelyNamed, IComparable<IContainable>, ISecurableBase
    {
        /// <summary>Gets or sets the name of a container containing this container.</summary>
        string ContainerName { get; set;}

        /// <summary>The order of this container compared to other containers and editors. Editors within the container are sorted according to their sort order.</summary>
        int SortOrder { get; set; }

        /// <summary>Adds a containable control to a container and returns it.</summary>
        /// <param name="container">The container onto which to add the containable control.</param>
        /// <returns>The newly added control.</returns>
        Control AddTo(Control container);
    }
}
