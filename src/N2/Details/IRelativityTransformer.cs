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
		/// <param name="applicationPath">The path of the web application.</param>
		/// <param name="absoluteOrRelativePath">The path to make absolute.</param>
		/// <returns>An absolute url.</returns>
		string ToAbsolute(string applicationPath, string absoluteOrRelativePath);

		/// <summary>Makes an absolute url relative (used when exporting).</summary>
		/// <param name="applicationPath">The path of the web application.</param>
		/// <param name="absoluteOrRelativePath">The path to make relative.</param>
		/// <returns>An application relative url.</returns>
		string ToRelative(string applicationPath, string absoluteOrRelativePath);
	}
}
