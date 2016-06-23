using System;
using System.Collections.Specialized;
using System.Linq;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using N2.Collections;
using N2.Definitions;
using N2.Edit.Versioning;
using N2.Engine;
using N2.Persistence;
using N2.Web.Mvc.Html;
using N2.Web.UI;
using N2.Security;
using N2.Edit;
using N2.Web.UI.WebControls;
using System.Web.Mvc;
using System.IO;
using N2.Web.Mvc;
using System.Web.Routing;
using System.Web.Mvc.Html;

namespace N2.Web.Parts
{
    /// <summary>
    /// Adapts operations related to zones, zone definitions, and items to display in a zone.
    /// </summary>
    [Adapts(typeof(ContentItem))]
    public class PartsAdapter : AbstractContentAdapter
    {
        IContentAdapterProvider adapters;
        IWebContext webContext;
        IPersister persister;
        IDefinitionManager definitions;
        IEnumerable<ITemplateProvider> templates;
        ISecurityManager security;
        ContentVersionRepository versionRepository;
        Rendering.ContentRendererSelector rendererSelector;
        private ITemplateAggregator templateAggregator;

        Logger<PartsAdapter> logger;

        public Rendering.ContentRendererSelector RendererSelector
        {
            get { return rendererSelector ?? Engine.Resolve<Rendering.ContentRendererSelector>(); }
            set { rendererSelector = value; }
        }

        public ContentVersionRepository VersionRepository
        {
            get { return versionRepository ?? Engine.Resolve<ContentVersionRepository>(); }
            set { versionRepository = value; }
        }

        public IPersister Persister
        {
            get { return persister ?? Engine.Resolve<IPersister>(); }
            set { persister = value; }
        }

        public IWebContext WebContext
        {
            get { return webContext ?? Engine.Resolve<IWebContext>(); }
            set { webContext = value; }
        }

        public IContentAdapterProvider Adapters
        {
            get { return adapters ?? Engine.Resolve<IContentAdapterProvider>(); }
            set { adapters = value; }
        }

        public IDefinitionManager Definitions
        {
            get { return definitions ?? Engine.Definitions; }
            set { definitions = value; }
        }

        public ITemplateAggregator TemplateAggregator
        {
            get { return templateAggregator ?? Engine.Resolve<ITemplateAggregator>(); }
            set { templateAggregator = value; }
        }

        public IEnumerable<ITemplateProvider> Templates
        {
            get { return templates ?? Engine.Container.ResolveAll<ITemplateProvider>(); }
            set { templates = value; }
        }

        public ISecurityManager Security
        {
            get { return security ?? Engine.Container.Resolve<ISecurityManager>(); }
            set { security = value; }
        }

        /// <summary>Retrieves content items added to a zone of the parnet item.</summary>
        /// <param name="parentItem">The item whose items to get.</param>
        /// <param name="zoneName">The zone in which the items should be contained.</param>
        /// <returns>A list of items in the zone.</returns>
        [Obsolete("Use overload with interface parameter")]
        public virtual ItemList GetItemsInZone(ContentItem parentItem, string zoneName)
        {
            return new ItemList(GetParts(parentItem, zoneName, Interfaces.Viewing));
        }
        
        /// <summary>Retrieves content items added to a zone of the parnet item.</summary>
        /// <param name="belowParentItem">The item whose items to get.</param>
        /// <param name="inZoneNamed">The zone in which the items should be contained.</param>
        /// <param name="filteredForInterface">Interface where the parts are displayed.</param>
        /// <returns>A list of items in the zone.</returns>
        public virtual IEnumerable<ContentItem> GetParts(ContentItem belowParentItem, string inZoneNamed, string filteredForInterface)
        {
            var state = WebContext.HttpContext == null 
                ? ControlPanelState.Unknown 
                : ControlPanel.GetState(Engine);

            return GetParts(belowParentItem, inZoneNamed, filteredForInterface, state);
        }

        /// <summary>Retrieves content items added to a zone of the parnet item.</summary>
        /// <param name="belowParentItem">The item whose items to get.</param>
        /// <param name="inZoneNamed">The zone in which the items should be contained.</param>
        /// <param name="filteredForInterface">Interface where the parts are displayed.</param>
        /// <param name="state">The control panel state to consider.</param>
        /// <returns>A list of items in the zone.</returns>
        public virtual IEnumerable<ContentItem> GetParts(ContentItem belowParentItem, string inZoneNamed, string filteredForInterface, ControlPanelState state)
        {
            if(belowParentItem == null)
                return new ItemList();

            var items = belowParentItem.Children.FindParts(inZoneNamed)
                .Where(new AccessFilter(WebContext.User, Security));

            if(filteredForInterface == Interfaces.Viewing 
                && !state.IsFlagSet(ControlPanelState.Previewing) 
                && !belowParentItem.VersionOf.HasValue)
                items = items.Where(new PublishedFilter());

            return items;
        }

        /// <summary>Retrieves allowed item definitions.</summary>
        /// <param name="parentItem">The parent item.</param>
        /// <param name="zoneName">The zone where children would be placed.</param>
        /// <param name="user">The user to restrict access for.</param>
        /// <returns>Item definitions allowed by zone, parent restrictions and security.</returns>
        public virtual IEnumerable<ItemDefinition> GetAllowedDefinitions(ContentItem parentItem, string zoneName, IPrincipal user)
        {
            return Definitions.GetAllowedChildren(parentItem, zoneName)
                .WhereAuthorized(Security, user, parentItem);
        }

        /// <summary>Retrieves allowed item definitions.</summary>
        /// <param name="parentItem">The parent item.</param>
        /// <param name="user">The user to restrict access for.</param>
        /// <returns>Item definitions allowed by zone, parent restrictions and security.</returns>
        public virtual IEnumerable<ItemDefinition> GetAllowedDefinitions(ContentItem parentItem, IPrincipal user)
        {
            return new[] { parentItem }
                .Concat(parentItem.Children.FindParts().SelectMany(Find.EnumerateTree))
				.SelectMany(ci => Definitions.GetAllowedChildren(ci))
				.Where(d => d.Enabled)
				.Where(d => d.AllowedIn != Integrity.AllowedZones.None)
                .Distinct();
        }

        /// <summary>Adds a content item part to a containing control hierarchy (typically a zone control). Override this method to adapt how a parent gets it's children added.</summary>
        /// <param name="item">The item to add a part.</param>
        /// <param name="container">The container control to host the part user interface.</param>
        public virtual Control AddChildPart(ContentItem item, Control container)
        {
            var adapter = Adapters.ResolveAdapter<PartsAdapter>(item);
            return adapter.AddTo(item, container);
        }

        class ContentRendererControl : Control
        {
            private Rendering.IContentRenderer renderer;
            private Rendering.ContentRenderingContext context;
            private string renderedContent;

            public ContentRendererControl(Rendering.IContentRenderer renderer, Rendering.ContentRenderingContext context)
            {
                this.renderer = renderer;
                this.context = context;
                this.context.Container = this;
            }

            protected override void OnInit(EventArgs e)
            {
                base.OnInit(e);

                using (var sw = new StringWriter())
                {
                    renderer.Render(context, sw);
                    renderedContent = sw.ToString();
                }
            }

            protected override void Render(HtmlTextWriter writer)
            {
                base.Render(writer);

                writer.Write(renderedContent);
            }
        }

        /// <summary>Adds a content part to a containing control. Override this method to adapt how a part is added to a parent.</summary>
        /// <param name="item"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public virtual Control AddTo(ContentItem item, Control container)
        {
            IAddablePart addablePart = item as IAddablePart;
            if (addablePart != null)
            {
                return addablePart.AddTo(container);
            }

            var renderer = item as Rendering.IContentRenderer
                ?? RendererSelector.ResolveRenderer(item.GetContentType());
            if (renderer != null)
            {
                var rendererControl = new ContentRendererControl(renderer, new Rendering.ContentRenderingContext { Content = item, Container = container });
                container.Controls.Add(rendererControl);
                return rendererControl;
            }

            string templateUrl = GetTemplateUrl(item);
            if (string.IsNullOrEmpty(templateUrl))
                return null;

            return AddUserControl(Url.ResolveTokens(templateUrl), container, item);
        }

        //private Control AddUserControl(Control container, ContentItem item)
        //{
        //  PathData path = item.FindPath(PathData.DefaultAction);
        //  if (!path.IsEmpty())
        //  {
        //      return AddUserControl(path.TemplateUrl, container, item);
        //  }
        //  return null;
        //}

        internal Control AddUserControl(string templateUrl, Control container, ContentItem item)
        {
            var userControlPath = Engine.ResolveAdapter<RequestAdapter>(item).ResolveTargetingUrl(templateUrl.ResolveUrlTokens());
            using (new ItemUtility.ItemStacker(item))
            {
                try
                {
                    Control templateItem = container.Page.LoadControl(userControlPath);
                    if (templateItem is IContentTemplate)
                        (templateItem as IContentTemplate).CurrentItem = item;
                    container.Controls.Add(templateItem);
                    return templateItem;
                }
                catch (HttpException ex)
                {
                    throw new HttpException("Error adding control for " + item, ex);
                }
            }
        }

        /// <summary>Gets the path to the given item's template. This is a way to override the default template provided by the content item.</summary>
        /// <param name="item">The item whose path is requested.</param>
        /// <returns>The virtual path of the template or null if the item is not supposed to be added.</returns>
        protected virtual string GetTemplateUrl(ContentItem item)
        {
            return item.FindPath(PathData.DefaultAction).TemplateUrl;
        }

        public virtual IEnumerable<TemplateDefinition> GetTemplates(ContentItem item, ItemDefinition definition)
        {
            return TemplateAggregator.GetTemplates(definition.ItemType);
        }

        public virtual void RenderPart(HtmlHelper html, ContentItem part, TextWriter writer = null)
        {
            var renderer = part as Rendering.IContentRenderer
                ?? RendererSelector.ResolveRenderer(part.GetContentType());
            if (renderer != null)
            {
                logger.DebugFormat("Using renderer {0} for part {1}", renderer, part);
                renderer.Render(new Rendering.ContentRenderingContext { Content = part, Html = html }, writer ?? html.ViewContext.Writer);
                return;
            }

            logger.DebugFormat("Using fallback template rendering for part {0}", part);
            new LegacyTemplateRenderer(Engine.Resolve<IControllerMapper>()).RenderTemplate(part, html, writer);
        }

        public class LegacyTemplateRenderer
        {
            Logger<LegacyTemplateRenderer> logger;

            private readonly IControllerMapper controllerMapper;

            public LegacyTemplateRenderer(IControllerMapper controllerMapper)
            {
                this.controllerMapper = controllerMapper;
            }

            public void RenderTemplate(ContentItem item, HtmlHelper helper, TextWriter writer = null)
            {
                RouteValueDictionary values = GetRouteValues(helper, item);

                if (values == null)
                    return;

                var currentPath = helper.ViewContext.RouteData.CurrentPath();
                try
                {
                    var newPath = currentPath.Clone(currentPath.CurrentPage, item);
                    helper.ViewContext.RouteData.ApplyCurrentPath(newPath);
					if (writer == null)
						helper.RenderAction("Index", values);
					else
						writer.Write(helper.Action("Index", values));
                }
                finally
                {
                    helper.ViewContext.RouteData.ApplyCurrentPath(currentPath);
                }
            }

            private RouteValueDictionary GetRouteValues(HtmlHelper helper, ContentItem item)
            {
                Type itemType = item.GetContentType();
                string controllerName = controllerMapper.GetControllerName(itemType);
                if (string.IsNullOrEmpty(controllerName))
                {
                    logger.WarnFormat("Found no controller for type {0}", itemType);
                    return null;
                }

                var values = new RouteValueDictionary();
                values[ContentRoute.ActionKey] = "Index";
                values[ContentRoute.ControllerKey] = controllerName;
                if (item.ID != 0)
                    values[ContentRoute.ContentItemKey] = item.ID;
                else
                    values[ContentRoute.ContentItemKey] = item;

                // retrieve the virtual path so we can figure out if this item is routed through an area
                var vpd = helper.RouteCollection.GetVirtualPath(helper.ViewContext.RequestContext, values);
                if (vpd == null)
                    throw new InvalidOperationException("Unable to render " + item + " (" + controllerName + " did not match any route)");

                values["area"] = vpd.DataTokens["area"];
                return values;
            }
        }
    }
}
