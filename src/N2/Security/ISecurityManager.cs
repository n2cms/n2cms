using System;
using System.Security.Principal;
using System.Web;
using System.Collections.Generic;
namespace N2.Security
{
	/// <summary>
	/// Classes implementing this interface are responsible of maintaining 
	/// security by monitoring things like page access and permissions to save.
	/// </summary>
	public interface ISecurityManager
	{
		/// <summary>Checks wether a user is an editor.</summary>
		/// <param name="principal">The user to check.</param>
		/// <returns>True if the user is an editor.</returns>
		bool IsEditor(IPrincipal principal);

		/// <summary>Checks wether a user is an administrator.</summary>
		/// <param name="principal">The user to check.</param>
		/// <returns>True if the user is an administrator.</returns>
		bool IsAdmin(IPrincipal principal);

		/// <summary>Checks wether a user is authorized to access a certain item.</summary>
		/// <param name="item">The item to check for access.</param>
		/// <param name="principal">The user whose permissions to check.</param>
		/// <returns>True if the user is authorized.</returns>
		bool IsAuthorized(ContentItem item, IPrincipal principal);

		/// <summary>Check whether an item is published, i.e. it's published and isn't expired.</summary>
		/// <param name="item">The item to check.</param>
		/// <returns>A boolean indicating whether the item is published.</returns>
		bool IsPublished(ContentItem item);

		/// <summary>Gets or sets wether this security manager is enabled.</summary>
		bool Enabled { get; set; }

		/// <summary>Gets or sets wether this security manager is enabled in the current scope. In a web application this is equivalent to a request.</summary>
		bool ScopeEnabled { get; set; }

        /// <summary>Finds out wether a user has one of the given accesses.</summary>
        bool IsAuthorized(IPrincipal user, IEnumerable<string> roles);
    }
}
