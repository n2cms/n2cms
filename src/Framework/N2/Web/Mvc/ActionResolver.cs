using System;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Helps the N2's MVC route handler to deal with actions in url.
	/// </summary>
	public class ActionResolver : IPathFinder
	{
		private readonly IControllerMapper controllerMapper;
		private readonly string[] methods;
		private readonly string controllerName;

		public ActionResolver(string controllerName, string[] methods)
		{
			this.controllerName = controllerName;
			this.methods = methods;
		}

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

			var controllerName = this.controllerName ?? controllerMapper.GetControllerName(item.GetContentType());
			if (string.IsNullOrEmpty(action) || string.Equals(action, "Default.aspx", StringComparison.InvariantCultureIgnoreCase))
				action = "Index";

			foreach (string method in methods)
			{
				if (string.Equals(method, action, StringComparison.InvariantCultureIgnoreCase))
				{
					var pd = new PathData(item) 
					{ 
						IsRewritable = false, 
						Controller = controllerName,
						Action = action,
						Argument = arguments,
						TemplateUrl = string.Format("~/{0}/{1}", controllerName, method, item.ID) // workaround for start pages
					};
					return pd;
				}
			}

			return null;
		}
	}
}