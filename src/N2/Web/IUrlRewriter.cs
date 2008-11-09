using System;
using System.Collections.Generic;
using System.Web;
using System.Diagnostics;

namespace N2.Web
{
	/// <summary>
	/// Classes implementing this interface can rewrite a generated url to an actual template.
	/// </summary>
	public interface IUrlRewriter
	{
		/// <summary>Rewrites a dynamic/computed url to an actual template url.</summary>
		void RewriteRequest();

		/// <summary>Makes sure the page handler is given the content item associated with the request.</summary>
		void InjectContentPage();

		void InitializeRequest();
	}
}
