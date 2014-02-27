using System;
using N2.Definitions;

namespace N2.Templates.Items
{
    /// <summary>
    /// A base class for item parts in the templates project.
    /// </summary>
    public abstract class AbstractItem : ContentItem, IPart
    {
        public override bool IsPage
        {
            get { return false; }
        }

        /// <summary>The name without extension .png of an icon file located in /Templates/UI/Img/. Defaults to "page_white".</summary>
        [Obsolete("No longer useful, sorry.")]
        protected virtual string IconName
        {
            get { return "page_white"; }
        }

        /// <summary>Defaults to ~/Templates/UI/Views/{TemplateName}.ascx</summary>
        public override string TemplateUrl
        {
            get { return "~/Templates/UI/Parts/" + TemplateName + ".ascx"; }
        }

        /// <summary>The name without extension .aspx of an icon file located in /Templates/UI/Views/. Defaults to ClassName.</summary>
        protected virtual string TemplateName
        {
            get { return GetContentType().Name; }
        }
    }
}
