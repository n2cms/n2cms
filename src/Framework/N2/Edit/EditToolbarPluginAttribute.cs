using System;
using System.Web.UI;

namespace N2.Edit
{
    /// <summary>
    /// A plugin added to the edit item toolbar area.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EditToolbarPluginAttribute : AdministrativePluginAttribute
    {
        private readonly string userControlUrl;

        public EditToolbarPluginAttribute(string userControlUrl)
        {
            this.userControlUrl = userControlUrl;
        }

        public override Control AddTo(Control container, PluginContext context)
        {
            Control c = container.Page.LoadControl(context.Rebase(userControlUrl));
            container.Controls.Add(c);
            return c;
        }
    }
}
