using System;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Helps the N2's MVC route handler to deal with actions in url.
	/// </summary>
	[Obsolete("This isn't needed any more. It may in fact prevent you from using custom routes. Please remove it.", true)]
	public class RouteActionResolverAttribute : Attribute, IPathFinder
	{
		public PathData GetPath(ContentItem item, string remainingUrl)
		{
			if (string.IsNullOrEmpty(remainingUrl) || string.Equals(remainingUrl, "default", StringComparison.InvariantCultureIgnoreCase))
				remainingUrl = "index";
			return new PathData(item, string.Empty, remainingUrl, null);
		}
	}
}
