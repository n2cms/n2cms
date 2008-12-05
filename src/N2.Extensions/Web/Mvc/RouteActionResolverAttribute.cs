using System;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Helps the N2's MVC route handler to deal with actions in url.
	/// </summary>
	public class RouteActionResolverAttribute : Attribute, ITemplateReference
	{
		public TemplateData GetTemplate(ContentItem item, string remainingUrl)
		{
			if (string.IsNullOrEmpty(remainingUrl) || string.Equals(remainingUrl, "default", StringComparison.InvariantCultureIgnoreCase))
				remainingUrl = "index";
			return new TemplateData(item, null, remainingUrl, null);
		}
	}
}
