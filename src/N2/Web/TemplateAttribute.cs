using System;

namespace N2.Web
{
	/// <summary>
	/// Registers a tempalte to serve a certain content item. Optionally based 
	/// on url remaining after the item is found. Multiple attributes can be 
	/// combined to allow for multiple views.
	/// </summary>
	/// <example>
	/// // Would map /path/to/my/content.aspx to MyContent aspx.
	/// // Would map /path/to/my/content/details.aspx to MyContentDetails aspx.
	/// [Template("~/Templates/MyContent.aspx")]
	/// [Template("details", "~/Templates/MyContentDetails.aspx")]
	/// public class MyContent : ContentItem { }
	/// </example>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class TemplateAttribute : Attribute, ITemplateReference
	{
		readonly string action;
		readonly int nameLength;
		readonly string nameWithSlash;
		readonly string templateUrl;

		/// <summary>Registers a template for the defualt action. This is equivalent to overriding the TemplateUrl property on the content item.</summary>
		/// <param name="defaultActionTemplateUrl">The url to the template to register, e.g. "~/path/to/my/template.aspx".</param>
		public TemplateAttribute(string defaultActionTemplateUrl)
			: this(TemplateData.DefaultAction, defaultActionTemplateUrl)
		{
		}

		/// <summary>Registers a template for the supplied action. This means that when no page matching the remaining path is found this template would be used if the remaining path starts with the given action url segment.</summary>
		/// <param name="actionUrlSegment">The action segment to look for in the remaining url when determining template.</param>
		/// <param name="actionTemplateUrl">The url to the template to register, e.g. "~/path/to/my/template.aspx".</param>
		public TemplateAttribute(string actionUrlSegment, string actionTemplateUrl)
		{
			action = actionUrlSegment;
			nameLength = actionUrlSegment.Length;
			nameWithSlash = actionUrlSegment + "/";
			templateUrl = actionTemplateUrl;
		}

		/// <summary>Examines the remaining url to find the appropriate template.</summary>
		/// <param name="item">The item to determine template for.</param>
		/// <param name="remainingUrl">The remaining url used to match against action url segment.</param>
		/// <returns>The matching template data if found, otherwise null.</returns>
		public TemplateData GetTemplate(ContentItem item, string remainingUrl)
		{
			if (remainingUrl.Equals(action, StringComparison.InvariantCultureIgnoreCase) || remainingUrl.Equals(action + item.Extension))
				return new TemplateData(item, templateUrl, action, string.Empty);

			if (remainingUrl.StartsWith(nameWithSlash))
			{
				string extension = item.Extension;
				string arguments = remainingUrl.Substring(nameLength + 1);
				if (arguments.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase))
					arguments = arguments.Substring(0, arguments.Length - extension.Length);

				return new TemplateData(item, templateUrl, action, arguments);
			}
			
			return null;
		}
	}
}