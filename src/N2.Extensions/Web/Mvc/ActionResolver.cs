using System;
using System.Linq;

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

			string templateUrl = GetTemplateUrl(item);

			foreach(string method in methods)
				if(method.Equals(action, StringComparison.InvariantCultureIgnoreCase))
					return new PathData(item, templateUrl, action, arguments);

			return null;
		}

		private string GetTemplateUrl(ContentItem item)
		{
			var templateUrl = String.Empty;
			var pathData = PathDictionary.GetFinders(item.GetType())
				.Where(finder => !(finder is ActionResolver))
				.Select(finder => finder.GetPath(item, null))
				.FirstOrDefault(path => path != null && !path.IsEmpty());

			if(pathData != null)
				templateUrl = pathData.TemplateUrl;
			return templateUrl;
		}
	}
}