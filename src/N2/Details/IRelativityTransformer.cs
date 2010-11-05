using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Details
{
	/// <summary>
	/// Used by the export/import system to transform to and from absolute paths 
	/// when the applications reside in a virtual directory. 
	/// </summary>
	public interface IRelativityTransformer
	{
		/// <summary>Tells the system when to make the path relative or absolute.</summary>
		RelativityMode RelativeWhen { get; }

		/// <summary>Makes a relative url absolute (used when importing).</summary>
		/// <param name="value">The content to rebase</param>
		/// <param name="fromAppPath">The current path.</param>
		/// <param name="toAppPath">The target path.</param>
		/// <returns>An absolute url.</returns>
		string Rebase(string value, string fromAppPath, string toAppPath);
	}
}
