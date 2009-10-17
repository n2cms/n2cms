using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Web
{
	/// <summary>
	/// This interface is used to inject rewriter dependency on content item 
	/// objects upon creation.
	/// </summary>
	public interface IUrlParserDependency
	{
		/// <summary>Sets the objects urlParser dependency.</summary>
		/// <param name="parser">The url parser to inject.</param>
		void SetUrlParser(IUrlParser parser);
	}
}
