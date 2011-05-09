using System.Security.Principal;
using N2.Definitions;
using N2.Plugin;

namespace N2.Security
{
	public static class SecurableExtensions
	{
		/// <summary>Checks whether the user is authorized to use a plugin and has sufficient permissions.</summary>
		/// <param name="security">The security manager to query permissions on.</param>
		/// <param name="plugin">The plugin containing security information.</param>
		/// <param name="user">The user to check permissions for.</param>
		/// <param name="item">The item to check permissions for.</param>
		/// <returns>True if the user is authorized for the given plugin.</returns>
		public static bool IsAuthorized(this ISecurityManager security, IPlugin plugin, IPrincipal user, ContentItem item)
		{
			return user != null && IsAuthorized(plugin, user, item) && IsPermitted(security, plugin, user, item);
		}

		/// <summary>Checks whether the user is authorized to use a editable and has sufficient permissions.</summary>
		/// <param name="security">The security manager to query permissions on.</param>
		/// <param name="editorOrContainer">The editable containing security information.</param>
		/// <param name="user">The user to check permissions for.</param>
		/// <param name="item">The item to check permissions for.</param>
		/// <returns>True if the user is authorized for the given editable.</returns>
		public static bool IsAuthorized(this ISecurityManager security, IContainable editorOrContainer, IPrincipal user, ContentItem item)
		{
			return user != null && IsAuthorized(editorOrContainer, user, item) && IsPermitted(security, editorOrContainer, user, item);
		}

		private static bool IsPermitted(ISecurityManager security, object possiblyPermittable, IPrincipal user, ContentItem item)
		{
			var permittable = possiblyPermittable as IPermittable;
			if (permittable != null && permittable.RequiredPermission > Permission.Read && !security.IsAuthorized(user, item, permittable.RequiredPermission))
				return false;
			return true;
		}

		private static bool IsAuthorized(object possiblySecurable, IPrincipal user, ContentItem item)
		{
			var securable = possiblySecurable as ISecurable;
			if (securable != null && securable.AuthorizedRoles != null && !PermissionMap.IsInRoles(user, securable.AuthorizedRoles))
				return false;
			return true;
		}
	}
}
