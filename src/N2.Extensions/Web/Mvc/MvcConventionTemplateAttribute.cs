using System;

namespace N2.Web.Mvc
{
	public class MvcConventionTemplateAttribute : Attribute, IPathFinder
	{
		readonly string _otherTemplateName;

		public MvcConventionTemplateAttribute()
		{
		}

		/// <summary>
		/// Uses the provided template name instead of the class name to
		/// find the template's location.
		/// </summary>
		/// <param name="otherTemplateName">The name used to find the template.</param>
		public MvcConventionTemplateAttribute(string otherTemplateName)
		{
			_otherTemplateName = otherTemplateName;
		}

		public PathData GetPath(ContentItem item, string remainingUrl)
		{
			if (String.IsNullOrEmpty(remainingUrl))
			{
				Type itemType = item.GetType();
				string virtualDirectory = ConventionTemplateDirectoryAttribute.GetDirectory(itemType);

				string templateName = _otherTemplateName ?? itemType.Name;

				return new PathData(item, String.Format(virtualDirectory.TrimEnd('/'), templateName));
			}
			return null;
		}
	}
}