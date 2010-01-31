using N2.Web;
using System;

namespace N2.Engine
{
	/// <summary>
	/// Base interface for user overridable adapters. 
	/// </summary>
	public interface IContentAdapter
	{
		/// <summary>The content engine requesting external control. TODO: support dependency injection.</summary>
		/// <remarks>This may be removed if dependency injection is enabled.</remarks>
		IEngine Engine { get; set; }
	}
}