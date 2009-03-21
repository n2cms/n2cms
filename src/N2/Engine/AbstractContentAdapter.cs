using N2.Web;

namespace N2.Engine
{
	/// <summary>
	/// Convenience base class for content adapters. One instance 
	/// of the inheriting class is created per request upon usage.
	/// </summary>
	public abstract class AbstractContentAdapter : IContentAdapter
	{		
		#region IAspectController Members

		/// <summary>The path associated with this controller instance.</summary>
		public PathData Path { get; set; }

		/// <summary>The content engine requesting external control.</summary>
		public IEngine Engine { get; set; }

		#endregion

	}
}
