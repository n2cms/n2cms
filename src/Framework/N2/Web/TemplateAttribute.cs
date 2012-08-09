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
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class TemplateAttribute : Attribute, IPathFinder
	{
		readonly string action;
		readonly int nameLength;
		readonly string nameWithSlash;
		readonly string templateUrl;

		/// <summary>Registers a template for the default action. This is equivalent to overriding the TemplateUrl property on the content item.</summary>
		/// <param name="defaultActionTemplateUrl">The url to the template to register, e.g. "~/path/to/my/template.aspx".</param>
		public TemplateAttribute(string defaultActionTemplateUrl)
			: this(PathData.DefaultAction, defaultActionTemplateUrl)
		{
		}

		/// <summary>Registers a template for the supplied action. This means that when no page matching the remaining path is found this template would be used if the remaining path starts with the given action url segment.</summary>
		/// <param name="urlSegmentOrTemplateKey">The action segment to look for in the remaining url when determining template.</param>
		/// <param name="actionTemplateUrl">The url to the template to register, e.g. "~/path/to/my/template.aspx".</param>
		public TemplateAttribute(string urlSegmentOrTemplateKey, string actionTemplateUrl)
		{
			action = urlSegmentOrTemplateKey;
			nameLength = urlSegmentOrTemplateKey.Length;
			nameWithSlash = urlSegmentOrTemplateKey + "/";
			templateUrl = actionTemplateUrl;
		}

		public string Action
		{
			get { return action; }
		}
		public string TemplateUrl
		{
			get { return templateUrl; }
		}

		/// <summary>
		/// Indicates that this template is selectable as the default template via the management ui using the [EditableTemplate] attribute.
		/// </summary>
		public bool SelectableAsDefault { get; set; }

		/// <summary>
		/// The title of this template if selectable as default.
		/// </summary>
		public string TemplateTitle { get; set; }

		/// <summary>
		/// The description of this template if selectable as default.
		/// </summary>
		public string TemplateDescription { get; set; }

		/// <summary>Examines the remaining url to find the appropriate template.</summary>
		/// <param name="item">The item to determine template for.</param>
		/// <param name="remainingUrl">The remaining url used to match against action url segment.</param>
		/// <returns>The matching template data if found, otherwise null.</returns>
		public PathData GetPath(ContentItem item, string remainingUrl)
		{
			if (remainingUrl == null)
				return null;
			else if (SelectableAsDefault && remainingUrl == "" && item.TemplateKey == Action)
				return new PathData(item, TemplateUrl, null, string.Empty);

			string extension = item.Extension;
			if(!string.IsNullOrEmpty(extension) && remainingUrl.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase))
			{
				remainingUrl = remainingUrl.Substring(0, remainingUrl.Length - extension.Length);
			}

			if (remainingUrl.Equals(Action, StringComparison.InvariantCultureIgnoreCase) || remainingUrl.Equals(Action + item.Extension))
				return new PathData(item, TemplateUrl, Action, string.Empty);

			if (remainingUrl.StartsWith(nameWithSlash))
			{
				string arguments = remainingUrl.Substring(nameLength + 1);
				return new PathData(item, TemplateUrl, Action, arguments);
			}
			
			return null;
		}
	}
}