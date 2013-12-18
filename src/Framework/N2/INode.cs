using System.Security.Principal;
using System;

namespace N2
{
    /// <summary>
    /// Represents a node in the tree in edit mode.
    /// </summary>
    [Obsolete("The role of this interface has been replaced by NodeAdapter.GetNode(item)")]
    public interface INode : N2.Web.ILink
    {
        /// <summary>The name of the node.</summary>
        string Name { get; }
        
        /// <summary>The logical path to the node from the root node.</summary>
        string Path { get; }
        
        /// <summary>The url used to preview the node in edit mode.</summary>
        string PreviewUrl { get; }
        
        /// <summary>Url to an icon image.</summary>
        string IconUrl { get; }

        /// <summary>Gets a whitespace separated list of class names used to decorate the node for editors.</summary>
        string ClassNames { get; }

        /// <summary>Finds out wether a certain user is authorized to read the node.</summary>
        /// <param name="user">The user to authorize.</param>
        /// <returns>True if the user is authorized.</returns>
        bool IsAuthorized(IPrincipal user);
    }
}
