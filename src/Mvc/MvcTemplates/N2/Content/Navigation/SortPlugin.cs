using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace N2.Edit.Navigation
{
    public class SortPluginAttribute : ToolbarPluginAttribute
    {
        public SortPluginAttribute()
            : base("", "sort", "#", ToolbarArea.Operations, "navigation", "", -40)
        {
            GlobalResourceClassName = "Toolbar";
            Legacy = true;
        }

        public override Control AddTo(Control container, PluginContext context)
        {
            HtmlGenericControl div = new HtmlGenericControl("div");
            div.Attributes["class"] = "sort";
            container.Controls.Add(div);

            HtmlAnchor up = AddSortAnchor(div, context,
                                          context.Format("{ManagementUrl}/Content/Navigation/sortUp.ashx?{Selection.SelectedQueryKey}={Selected.Path}", true),
                                          "{ManagementUrl}/Resources/icons/bullet_arrow_up.png", "up");
            up.Attributes["data-url-template"] = context.Rebase("{ManagementUrl}/Content/Navigation/sortUp.ashx?" + SelectionUtility.SelectedQueryKey + "={selected}");
            up.Title = "Move up";

            HtmlAnchor down = AddSortAnchor(div, context,
                                            context.Format("{ManagementUrl}/Content/Navigation/sortDown.ashx?{Selection.SelectedQueryKey}={Selected.Path}", true),
                                            "{ManagementUrl}/Resources/icons/bullet_arrow_down.png", "down");
            down.Attributes["data-url-template"] = context.Rebase("{ManagementUrl}/Content/Navigation/sortDown.ashx?" + SelectionUtility.SelectedQueryKey + "={selected}");
            down.Title = "Move down";

            return div;
        }

        protected virtual HtmlAnchor AddSortAnchor(Control container, PluginContext context, string url, string iconUrl, string key)
        {
            HtmlAnchor a = new HtmlAnchor();
            a.ID = key + Name;
            a.HRef = context.Rebase(url);

            a.Target = Target;
            a.Attributes["class"] = "templatedurl " + key;
            a.Title = Utility.GetResourceString(GlobalResourceClassName, key + Name + ".ToolTip") ?? ToolTip;
            a.Style[HtmlTextWriterStyle.BackgroundImage] = context.Rebase(iconUrl);

            container.Controls.Add(a);
            return a;
        }
    }
}
