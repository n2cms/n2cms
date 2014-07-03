using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Security;
using N2.Web;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
    /// <summary>
    /// Used internally to apply the preview publish button.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ControlPanelPreviewPublishAttribute : ControlPanelLinkAttribute
    {
        public ControlPanelPreviewPublishAttribute(string toolTip, int sortOrder)
            : base("cpPreviewPublish", "{ManagementUrl}/Resources/icons/disk_green.png", null, toolTip, sortOrder, ControlPanelState.Previewing)
        {
            IconClass = "fa fa-play-circle";
        }

        public override Control AddTo(Control container, PluginContext context)
        {
            if (!context.Engine.SecurityManager.IsAuthorized(this, context.HttpContext.User, context.Selected))
                return null;

            if (!ActiveFor(container, context.State))
                return null;

            HyperLink hl = new HyperLink();
            hl.Text = GetInnerHtml(context, IconUrl, ToolTip, Title);
            hl.NavigateUrl = Url.Parse("{ManagementUrl}/Content/PublishPreview.aspx").ResolveTokens()
                .AppendQuery("selectedUrl", context.Selected.Url)
                .AppendQuery(PathData.ItemQueryKey, context.Selected.VersionOf.ID)
                .AppendQuery(PathData.VersionIndexQueryKey, context.Selected.VersionIndex);
            hl.ToolTip = Utility.GetResourceString(GlobalResourceClassName, Name + ".ToolTip") ?? context.Format(ToolTip, false);
            hl.CssClass = "publish";
            container.Controls.Add(hl);

            return hl;
        }

        protected void RedirectTo(Page page, ContentItem item)
        {
            string url = page.Request["returnUrl"];
            if (string.IsNullOrEmpty(url))
                url = Engine.GetContentAdapter<NodeAdapter>(item).GetPreviewUrl(item);

            page.Response.Redirect(url);
        }
    }
}
