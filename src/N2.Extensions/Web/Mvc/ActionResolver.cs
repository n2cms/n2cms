using System;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Helps the N2's MVC route handler to deal with actions in url.
	/// </summary>
	public class ActionResolver : IPathFinder
	{
		readonly string[] methods;

		public ActionResolver(string[] methods)
		{
			this.methods = methods;
		}

		public PathData GetPath(ContentItem item, string remainingUrl)
		{
			if (string.IsNullOrEmpty(remainingUrl) || string.Equals(remainingUrl, "default", StringComparison.InvariantCultureIgnoreCase))
				remainingUrl = "index";
			int slashIndex = remainingUrl.IndexOf('/');
			
			string action = remainingUrl;
			string arguments = null;
			if(slashIndex > 0)
			{
				action = remainingUrl.Substring(0, slashIndex);
				arguments = remainingUrl.Substring(slashIndex + 1);
			}
			
			foreach(string method in methods)
				if(method.Equals(action, StringComparison.InvariantCultureIgnoreCase))
					return new PathData(item, string.Empty, action, arguments);

			return null;
		}
	}
}