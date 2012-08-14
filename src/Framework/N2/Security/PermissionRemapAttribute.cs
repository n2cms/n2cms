using System;

namespace N2.Security
{
	/// <summary>
	/// When decorating a content item this attribute is used by the security manager
	/// to remap required permissions of the item.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class PermissionRemapAttribute : Attribute
	{
		public Permission From { get; set; }
		public Permission To { get; set; }

		public Permission Remap(Permission permission)
		{
			if ((permission & From) == From)
				return (permission ^ From) | To;
			
			return permission;
		}
	}
}
