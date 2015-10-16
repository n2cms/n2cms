using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Definitions;
using N2.Web;
using N2.Web.Mvc.Html;
using N2.Web.UI.WebControls;
using System.Web.UI;
using N2.Edit.Versioning;
using N2.Web.UI;
using System.Web;

namespace N2.Edit
{
    public static class EditExtensions
    {
        /// <summary>Checks access and the drag'n'drop state before adding the creator node to the given collection.</summary>
        /// <param name="items"></param>
        /// <param name="engine"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static IEnumerable<ContentItem> TryAppendCreatorNode(this IEnumerable<ContentItem> items, IEngine engine, ContentItem parent)
        {
            var state = N2.Web.UI.WebControls.ControlPanel.GetState(engine);
            if (!state.IsFlagSet(ControlPanelState.DragDrop))
                return items;

            return items.AppendCreatorNode(engine, parent);
        }

        /// <summary>Appends the creator node to the given collection.</summary>
        /// <param name="items"></param>
        /// <param name="engine"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static IEnumerable<ContentItem> AppendCreatorNode(this IEnumerable<ContentItem> items, IEngine engine, ContentItem parent)
        {
            if (parent.ID == 0)
                return items;

            return items.Union(new[] { new CreatorItem(engine, parent) });
        }

        public static void RefreshManagementInterface(this Page page, ContentItem item)
        {
            string previewUrl = N2.Context.Current.ManagementPaths.GetEditInterfaceUrl(item);
            string script = string.Format("window.top.location = '{0}';", previewUrl);

            page.ClientScript.RegisterClientScriptBlock(
                typeof(EditExtensions),
                "RefreshScript",
                script, true);
        }

        private const string RefreshBothFormat = @"if(window.n2ctx) n2ctx.refresh({{ navigationUrl:'{1}', previewUrl:'{2}', path:'{4}', permission:'{5}', force:{6}, versionIndex:{7}, versionKey:'{8}' }});";
        private const string RefreshNavigationFormat = @"if(window.n2ctx) n2ctx.refresh({{ navigationUrl:'{1}', path:'{4}', permission:'{5}', force:{6}, versionIndex:{7}, versionKey:'{8}' }});";
        private const string RefreshPreviewFormat = @"if(window.n2ctx) n2ctx.refresh({{ previewUrl: '{2}', path:'{4}', permission:'{5}', force:{6}, versionIndex:{7}, versionKey:'{8}' }});";

        public static void RefreshPreviewFrame(this Page page, ContentItem item, string previewUrl)
        {
            var engine = page.GetEngine();
            string script = string.Format(RefreshBothFormat,
                engine.ManagementPaths.GetEditInterfaceUrl(), // 0
                engine.ManagementPaths.GetNavigationUrl(item), // 1
                Url.ToAbsolute(previewUrl), // 2
                item.ID, // 3
                item.Path, // 4
                engine.ResolveAdapter<NodeAdapter>(item).GetMaximumPermission(item), // permission:'{5}',
                "true", // force:{6}
                item.VersionIndex,
                item.GetVersionKey()
            );
            
            page.ClientScript.RegisterClientScriptBlock(
                typeof(EditExtensions),
                "RefreshFramesScript",
                script, true);
        }

        /// <summary>Referesh the selected frames after loading the page.</summary>
        /// <param name="item"></param>
        /// <param name="area"></param>
        public static void RefreshFrames(this Page page, ContentItem item, ToolbarArea area, bool force = true)
        {
            string script = GetRefreshFramesScript(page, item, area, force);

            page.ClientScript.RegisterClientScriptBlock(
                typeof(EditExtensions),
                "RefreshFramesScript",
                script, true);
        }

        public static string GetRefreshFramesScript(this Page page, ContentItem item, ToolbarArea area, bool force = true)
        {
            var engine = N2.Context.Current;

            string format;
            if (area == ToolbarArea.Both)
                format = EditExtensions.RefreshBothFormat;
            else if (area == ToolbarArea.Preview)
                format = RefreshPreviewFormat;
            else
                format = RefreshNavigationFormat;

            string script = string.Format(format,
                engine.ManagementPaths.GetEditInterfaceUrl(), // 0
                engine.ManagementPaths.GetNavigationUrl(item), // 1
                GetPreviewUrl(page, engine, item), // 2
                item.ID, // 3
                item.Path, // 4
                engine.ResolveAdapter<NodeAdapter>(item).GetMaximumPermission(item), // 5
                force.ToString().ToLower(), // 6
                item.VersionIndex,
                item.GetVersionKey()
                );
            return script;
        }

        internal static string GetPreviewUrl(this Page page, IEngine engine, ContentItem item)
        {
            string returnUrl = page.Request["returnUrl"];
            if (!string.IsNullOrEmpty(returnUrl))
                return Url.Parse(returnUrl)
                    .SetQueryParameter(PathData.VersionIndexQueryKey, item.VersionIndex)
                    .SetQueryParameter(PathData.VersionKeyQueryKey, item.GetVersionKey());
            return engine.ResolveAdapter<NodeAdapter>(item).GetPreviewUrl(item);
        }

        internal static string GetNavigationUrl(this Page page, IEngine engine, ContentItem item)
        {
            return engine.ResolveAdapter<NodeAdapter>(item).GetNavigationUrl(item);
        }

        public static SelectionUtility GetSelection(this Page page)
        {
            if (page is Web.EditPage)
                return (page as Web.EditPage).Selection;

            var selection = page.Items["SelectionUtility"] as SelectionUtility;
            if(selection == null)
                page.Items["SelectionUtility"] = selection = new SelectionUtility(page, N2.Context.Current);

            return selection;
        }

        public static void InsertChildBefore(this ContentItem parent, ContentItem child, int beforeSortOrder)
        {
            bool wasAdded = false;
            for (int i = 0; i < parent.Children.Count; i++)
            {
                var sibling = parent.Children[i];
                if (sibling.SortOrder >= beforeSortOrder)
                {
                    parent.Children.Insert(i, child);
                    child.Parent = parent;
                    Utility.UpdateSortOrder(parent.Children);
                    wasAdded = true;
                    break;
                }
            }
            if (!wasAdded)
            {
                child.AddTo(parent);
                Utility.UpdateSortOrder(parent.Children);
            }
        }

        public static SelectionUtility GetSelectionUtility(this HttpContext context, IEngine engine = null)
        {
            return GetSelectionUtility(context.GetHttpContextBase(), engine);
        }

        public static SelectionUtility GetSelectionUtility(this HttpContextBase context, IEngine engine = null)
        {
            var selection = context.Items["CachedSelectionUtility"] as SelectionUtility;
            if (selection == null)
                context.Items["CachedSelectionUtility"] = selection = new SelectionUtility(context, engine ?? Context.Current);
            return selection;
        }

		public static string GetDataType(this HttpRequestBase request)
		{
			return request["dataType"];
		}

		public static string GetDiscriminator(this HttpRequestBase request)
		{
			return request["discriminator"];
		}

		public static string GetTemplate(this HttpRequestBase request)
		{
			return request["template"];
		}
	}

    internal class CreatorItem : ContentItem, ISystemNode, IStyleable
    {
        public CreatorItem()
        {
            State = ContentState.Published;
        }

        public CreatorItem(IEngine engine, ContentItem parent)
            : this()
        {
            this.url = engine.ManagementPaths.GetSelectNewItemUrl(parent).ToUrl().AppendQuery("returnUrl", engine.Resolve<IWebContext>().HttpContext.Request.RawUrl);
            this.Title = Utility.GetGlobalResourceString("Management", "Add") ?? "Add...";
        }

        string url;
        public override string Url
        {
            get { return url; }
        }

		public ElementStyle Style
		{
			get { return ElementStyle.Empty.Prefix("<b class='fa fa-plus-circle'></b> "); }
		}
	}
}
