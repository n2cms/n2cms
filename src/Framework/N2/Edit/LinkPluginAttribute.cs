using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Edit
{
    /// <summary>
    /// Base class for plugins in the navigation screen and toolbar.
    /// </summary>
    public abstract class LinkPluginAttribute : AdministrativePluginAttribute
    {
        private string globalResourceClassName;
        private string target = Targets.Preview;
        private string title;
        private string toolTip;
        private string urlFormat;

        public bool IsDivider { get; set; }

        /// <summary>The target frame for the plugn link.</summary>
        public string Target
        {
            get { return target; }
            set { target = value; }
        }

        /// <summary>The plugin's text.</summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        /// The plugin's url format. These magic strings are interpreted by the 
        /// client and inserted in the url before the frame is loaded: 
        /// {selected}, {memory}, {action}
        /// </summary>
        public string UrlFormat
        {
            get { return urlFormat; }
            set { urlFormat = value; }
        }

        /// <summary>The plugin's tool tip.</summary>
        public string ToolTip
        {
            get { return toolTip; }
            set { toolTip = value; }
        }

        /// <summary>Alternative text for the icon.</summary>
        public string AlternativeText { get; set; }

        /// <summary>Used for translating the plugin's texts from a global resource.</summary>
        public string GlobalResourceClassName
        {
            get { return globalResourceClassName; }
            set { globalResourceClassName = value; }
        }

        public override Control AddTo(Control container, PluginContext context)
        {
            HyperLink a = AddAnchor(container, context);
            return a;
        }

        protected virtual HyperLink AddAnchor(Control container, PluginContext context)
        {
            string tooltip = Utility.GetResourceString(GlobalResourceClassName, Name + ".ToolTip") ?? ToolTip;
            string title = Utility.GetResourceString(GlobalResourceClassName, Name + ".Title") ?? Title;
            string alternative = Utility.GetResourceString(GlobalResourceClassName, Name + ".AlternativeText") ?? AlternativeText;

            HyperLink a = new HyperLink();
            a.ID = "h" + Name;
            a.NavigateUrl = context.Rebase(context.Format(UrlFormat, true));
            a.SkinID = "ToolBarLink_" + Name;

            a.Target = Target;
            a.Attributes["class"] = "templatedurl " + Name + " " + RequiredPermission.ToString() + (string.IsNullOrEmpty(IconUrl) ? "" : " iconed");
            a.Attributes["data-url-template"] = context.Rebase(UrlFormat);
            a.Text = tooltip;
            a.ToolTip = tooltip;
            a.Text = title;
            ApplyStyles(context, a);

            container.Controls.Add(a);
            return a;
        }
    }
}
