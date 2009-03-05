using N2.Web;
using System;

namespace N2.Engine
{
	/// <summary>
	/// Base interface for user overridable controllers of various aspects. 
	/// </summary>
	public interface IAspectController
	{
		/// <summary>The path associated with this controller instance.</summary>
		PathData Path { get; set; }

		/// <summary>The content engine requesting external control. TODO: support dependency injection.</summary>
		/// <remarks>This may be removed if dependency injection is enabled.</remarks>
		IEngine Engine { get; set; }
	}
}