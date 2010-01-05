using System;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Tells the system to look for the template associated with the
	/// attributed content item in the default location as specified
	/// by the N2ViewEngine's internal ViewEngine (by default a WebFormsViewEngine)
	/// </summary>
	public class MvcConventionTemplateAttribute : Attribute, IPathFinder
	{
		private readonly string _otherTemplateName;
		private IControllerMapper _controllerMapper;

		public string DefaultAction { get; set; }

		public MvcConventionTemplateAttribute()
		{
			DefaultAction = "index";
		}

		/// <summary>
		/// Uses the provided template name instead of the class name to
		/// find the template's location.
		/// </summary>
		/// <param name="otherTemplateName">The name used to find the template.</param>
		public MvcConventionTemplateAttribute(string otherTemplateName) : this()
		{
			_otherTemplateName = otherTemplateName;
		}

		#region IPathFinder Members

		public PathData GetPath(ContentItem item, string remainingUrl)
		{
			if (remainingUrl != null && (remainingUrl.ToLowerInvariant() == "default.aspx" || remainingUrl.Contains("/")))
				return null;

			if (item == null)
				throw new ArgumentNullException("item");

			Type itemType = item.GetType();

			string templateName = _otherTemplateName ?? itemType.Name;

			string action = remainingUrl ?? DefaultAction;

			if (ActionDoesNotExistOnController(item, action))
				return null;

			return new PathData(item, templateName, action, String.Empty);
		}

		#endregion

		// For unit tests
		public IControllerMapper ControllerMapper
		{
			get { return _controllerMapper = _controllerMapper ?? N2.Context.Current.Resolve<IControllerMapper>(); }
			set { _controllerMapper = value; }
		}

		private bool ActionDoesNotExistOnController(ContentItem item, string action)
		{
			var controllerName = ControllerMapper.GetControllerName(item.GetType());

			return !ControllerMapper.ControllerHasAction(controllerName, action);
		}
	}
}