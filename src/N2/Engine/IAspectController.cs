using N2.Web;

namespace N2.Engine.Aspects
{
	/// <summary>
	/// Base interface for user extendable controllers in N2 CMS. 
	/// </summary>
	public interface IAspectController
	{
		/// <summary>The path associated with this controller instance.</summary>
		PathData Path { get; set; }

		/// <summary>The content engine requesting control.</summary>
		IEngine Engine { get; set; }
	}
}