using N2.Web;

namespace N2.Templates.Items
{
	public class DefaultTemplateAttribute : TemplateAttribute
	{
		public const string DefaultTemplateFolderPath = "~/Templates/UI/Views/";

		public DefaultTemplateAttribute(string viewName)
			: base(DefaultTemplateFolderPath + viewName + ".aspx")
		{
		}
	}
}
