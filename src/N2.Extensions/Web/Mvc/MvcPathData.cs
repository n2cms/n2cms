using System;

namespace N2.Web.Mvc
{
	public class MvcPathData : PathData
	{
		private static string _defaultControllerAction = "index";

		public static string DefaultControllerAction
		{
			get { return _defaultControllerAction; }
			set { _defaultControllerAction = value; }
		}

		private readonly string _controllerName;

		public MvcPathData(ContentItem item, string templateUrl, string action, string arguments, string controllerName)
			: base(item, templateUrl, action, arguments)
		{
			_controllerName = controllerName;
		}

		public override Url RewrittenUrl
		{
			get
			{
				string urlString = "~/" + _controllerName;
				
				if (Action.ToLowerInvariant() != DefaultControllerAction.ToLowerInvariant())
					urlString = String.Format("~/{0}/{1}", _controllerName, Action);

				var url = new Url(urlString).AppendQuery(PageQueryKey, ID);

				return url;
			}
		}
	}
}