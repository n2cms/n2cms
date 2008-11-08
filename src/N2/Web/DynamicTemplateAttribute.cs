using System;

namespace N2.Web
{
	/// <summary>
	/// Register a template reference that uses the content item's 
	/// TemplateUrl property as template.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class DynamicTemplateAttribute : Attribute, ITemplateReference
	{
		public TemplateData GetTemplate(ContentItem item, string remainingUrl)
		{
			if(string.IsNullOrEmpty(remainingUrl))
				return new TemplateData(item, item.Path, item.TemplateUrl);
			return null;
		}
	}
}