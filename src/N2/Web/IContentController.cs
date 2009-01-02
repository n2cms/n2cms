namespace N2.Web
{
	/// <summary>
	/// Base interface for user extendable controllers in N2 CMS. 
	/// </summary>
	public interface IContentController
	{
		/// <summary>The path associated with this controller instance.</summary>
		PathData Path { get; set; }
	}
}