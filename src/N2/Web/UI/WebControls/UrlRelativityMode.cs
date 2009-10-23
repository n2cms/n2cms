namespace N2.Web.UI.WebControls
{
	/// <summary>
	/// Defines whether the urls should be app- or server relative.
	/// </summary>
	public enum UrlRelativityMode
	{
		/// <summary>Server relative urls, i.e. /myapp/upload/myfile.gif.</summary>
		Absolute = 0,
		/// <summary>Application relative urls, i.e. ~/upload/myfile.gif.</summary>
		Application = 1
	}
}