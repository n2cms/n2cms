using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;

namespace N2.Security
{
	/// <summary>
	/// An exeption thrown when a user tries to access an unauthorized item.
	/// </summary>
	public class PermissionDeniedException : N2Exception
	{
		public PermissionDeniedException(ContentItem item, IPrincipal user)
			: base("Permission denied")
		{
			this.user = user;
			this.item = item;
		}

		#region Private Members
		private ContentItem item;
		private IPrincipal user; 
		#endregion

		#region Properties
		/// <summary>Gets the user which caused the exception.</summary>
		public IPrincipal User
		{
			get { return user; }
		}

		/// <summary>Gets the item that caused the exception.</summary>
		public ContentItem Item
		{
			get { return item; }
		} 
		#endregion
	}
}
