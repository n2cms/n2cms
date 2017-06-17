using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Security;
using N2.Web;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
    /// <summary>
    /// Used internally to add the dicard preview button.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ControlPanelDiscardPreviewOrDeleteAttribute : ControlPanelLinkAttribute
    {
        public ControlPanelDiscardPreviewOrDeleteAttribute()
            : base("cpDelete", "{IconsUrl}/cross.png", "{ManagementUrl}/Content/Delete.aspx?{Selection.SelectedQueryKey}={Selected.Path}", "Delete", 60, ControlPanelState.Visible | ControlPanelState.Previewing)
        {
            RequiredPermission = Permission.Write;
            CssClass = "complementary";
            IconClass = "fa fa-trash-o";
            SortOrder = 100;
			Legacy = true;
        }

        public override Control AddTo(Control container, PluginContext context)
        {
            var requiredPermission = context.Selected.State == ContentState.Published ? Permission.Publish : Permission.Write;
            if (!context.Engine.SecurityManager.IsAuthorized(context.HttpContext.User, context.Selected, requiredPermission))
                return null;

            if(!ActiveFor(container, context.State)) return null;
            
            if (!context.Selected.VersionOf.HasValue) 
                return base.AddTo(container, context);

            HyperLink hl = new HyperLink();
            hl.Text = GetInnerHtml(context, "{IconsUrl}/cross_orange.png", ToolTip, Title);
            hl.NavigateUrl = Url.Parse("{ManagementUrl}/Content/DiscardPreview.aspx").ResolveTokens()
                .AppendQuery("selectedUrl", context.Selected.Url)
                .AppendQuery(PathData.ItemQueryKey, context.Selected.VersionOf.ID)
                .AppendQuery(PathData.VersionIndexQueryKey, context.Selected.VersionIndex);
            hl.CssClass = "cancel";
            hl.Attributes["onclick"] = "return confirm('Are you certain?');";

            hl.ToolTip = Utility.GetResourceString(GlobalResourceClassName, Name + ".ToolTip") ?? context.Format("Delete this version", false);

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
