using System;
using N2.Web;

namespace N2.Templates.Items
{
    [Obsolete("ConventionTemplate is now the preferred way to reference the template aspx.")]
    public class DefaultTemplateAttribute : TemplateAttribute
    {
        public const string DefaultTemplateFolderPath = "~/Templates/UI/Views/";

        public DefaultTemplateAttribute(string viewName)
            : base(DefaultTemplateFolderPath + viewName + ".aspx")
        {
        }
    }
}
