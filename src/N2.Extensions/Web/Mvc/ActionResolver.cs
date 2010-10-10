using System;
using System.Linq;
using System.Web.Routing;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Helps the N2's MVC route handler to deal with actions in url.
	/// </summary>
	public class ActionResolver : IPathFinder
	{
		private readonly IControllerMapper controllerMapper;
		private readonly string[] methods;

		public ActionResolver(IControllerMapper controllerMapper, string[] methods)
		{
			this.controllerMapper = controllerMapper;
			this.methods = methods;
		}

		public string[] Methods
		{
			get{ return methods; }
		}

		public PathData GetPath(ContentItem item, string remainingUrl)
		{
			int slashIndex = remainingUrl.IndexOf('/');
			
			string action = remainingUrl;
			string arguments = null;
			if(slashIndex > 0)
			{
				action = remainingUrl.Substring(0, slashIndex);
				arguments = remainingUrl.Substring(slashIndex + 1);
			}

			var controllerName = controllerMapper.GetControllerName(item.GetContentType());
			if (string.IsNullOrEmpty(action) || string.Equals(action, "default.aspx", StringComparison.InvariantCultureIgnoreCase))
				action = "index";

			foreach (string method in methods)
			{
				if (string.Equals(method, action, StringComparison.InvariantCultureIgnoreCase))
				{
					return new PathData(item, null, action, arguments) 
					{ 
						IsRewritable = false, 
						TemplateUrl = string.Format("~/{0}/{1}", controllerName, method, item.ID) // workaround for start pages
					};
				}
			}

			return null;
		}
	}
}