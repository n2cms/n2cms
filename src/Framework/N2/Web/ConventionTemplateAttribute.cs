using System;

namespace N2.Web
{
	/// <summary>
	/// Tells the system to look for the ASPX template associated with the
	/// attribute content item in the default location. This is typically
	/// "~/UI/Views/" or the location defined by any [ConventionTemplateDirectory]
	/// attribute in the same assembly.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ConventionTemplateAttribute : Attribute, IPathFinder
	{
		readonly string otherTemplateName;

		public ConventionTemplateAttribute()
		{
		}

		/// <summary>
		/// Uses the provided template name instead of the class name to
		/// find the template's location.
		/// </summary>
		/// <param name="otherTemlpateName">The name used to find the template.</param>
		public ConventionTemplateAttribute(string otherTemlpateName)
		{
			this.otherTemplateName = otherTemlpateName;
		}

		#region IPathFinder Members

		public PathData GetPath(ContentItem item, string remainingUrl)
		{
			if(string.IsNullOrEmpty(remainingUrl))
			{
				Type itemType = item.GetContentType();
				string virtualDirectory = ConventionTemplateDirectoryAttribute.GetDirectory(itemType);

				string templateName = otherTemplateName ?? itemType.Name;
				return new PathData(item, virtualDirectory + templateName + ".aspx");
			}
			return null;
		}

		#endregion
	}
}
