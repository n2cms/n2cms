using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Security.Principal;
using System.Collections;

namespace N2.Tests.Fakes
{
	/// <summary>
    /// A wrapper for static web methods.
    /// </summary>
	public class FakeWebContextWrapper : N2.Web.ThreadContext
	{
		public IPrincipal currentUser = SecurityUtilities.CreatePrincipal("admin");

		public override IPrincipal User
		{
			get { return currentUser; }
		}
		public override string ToAbsolute(string virtualPath)
		{
			return virtualPath.TrimStart('~');
		}
		public override string ToAppRelative(string virtualPath)
		{
			return virtualPath;
		}

		IDictionary items = new Dictionary<string, object>();
		public override IDictionary RequestItems
		{
			get
			{
				return items;
			}
		}
	}
}
