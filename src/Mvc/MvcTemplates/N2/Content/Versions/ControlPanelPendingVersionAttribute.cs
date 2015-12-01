using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Engine;
using N2.Persistence;
using N2.Web;
using N2.Web.UI.WebControls;
using N2.Edit.Versioning;
using System.Linq;

namespace N2.Edit.Versions
{
    /// <summary>
    /// Used internally to add the dicard preview button.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ControlPanelPendingVersionAttribute : ControlPanelLinkAttribute
    {
        public ControlPanelPendingVersionAttribute(string toolTip, int sortOrder)
            : base("cpPendingVersion", "{IconsUrl}/book_next_orange.png", null, toolTip, sortOrder, ControlPanelState.Visible)
        {
			IconClass = "fa fa-clock-o";
			Legacy = true;
        }

        public override Control AddTo(Control container, PluginContext context)
        {
            if(!ActiveFor(container, context.State)) return null;
            if (context.Selected == null) return null;
            if (context.Selected.VersionOf.HasValue) return null;

            IEngine engine = Context.Current;
            //TODO: fixme, cause items to be deserialized always
            //ContentItem latestVersion = engine.Resolve<IVersionManager>().GetVersionsOf(context.Selected, 0, 1)[0];
            var draft = engine.Resolve<DraftRepository>().GetDraftInfo(context.Selected);
            if (draft == null)
                return null;
			var drafts = engine.Resolve<DraftRepository>();
			var latestVersion = drafts.FindDrafts(context.Selected).Select(v => drafts.Versions.DeserializeVersion(v)).FirstOrDefault();
            if (latestVersion == null)
                return null;

            Url versionPreviewUrl = engine.GetContentAdapter<NodeAdapter>(latestVersion).GetPreviewUrl(latestVersion);
            versionPreviewUrl = versionPreviewUrl.SetQueryParameter("edit", context.HttpContext.Request["edit"]);

            HyperLink hl = new HyperLink();
            hl.NavigateUrl = versionPreviewUrl;
            hl.Text = GetInnerHtml(context, latestVersion.State == ContentState.Waiting ? "{IconsUrl}/clock_play.png".ResolveUrlTokens() : IconUrl, ToolTip, Title);
            hl.CssClass = "preview";
            hl.ToolTip = Utility.GetResourceString(GlobalResourceClassName, Name + ".ToolTip") ?? context.Format(ToolTip, false);
            container.Controls.Add(hl);

            return hl;
        }
    }
}
