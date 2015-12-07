using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Edit;
using N2.Engine;
using N2.Plugin;
using N2.Resources;
using N2.Web.Parts;
using N2.Security;
using System.Linq;

namespace N2.Web.UI.WebControls
{
    /// <summary>
    /// A control panel for on page editing. The control displays buttons for 
    /// start editing and saving changes.
    /// </summary>
    [PersistChildren(false)]
    [ParseChildren(true)]
    [OrganizeSwitch]
    public class ControlPanel : Control, IItemContainer
    {
        private const string ArrayKey = "ControlPanel.arrays";

        #region Properties

        public bool EnableEditInterfaceIntegration
        {
            get { return (bool) (ViewState["EnableEditInterfaceIntegration"] ?? true); }
            set { ViewState["EnableEditInterfaceIntegration"] = value; }
        }

        /// <summary>Gets or sets the url to a style sheet added to the page when editing.</summary>
        public string StyleSheetUrl
        {
            get { return (string) (ViewState["StyleSheetUrl"] ?? "{ManagementUrl}/Resources/Css/edit.css"); }
            set { ViewState["StyleSheetUrl"] = value; }
        }

        public string DragDropScriptUrl
        {
            get { return (string) (ViewState["DragDropScriptUrl"] ?? "{ManagementUrl}/Resources/Js/parts.js"); }
            set { ViewState["DragDropScriptUrl"] = value; }
        }

        public string DragDropStyleSheetUrl
        {
            get { return (string) (ViewState["DragDropStyleSheetUrl"] ?? "{ManagementUrl}/Resources/Css/Parts.css"); }
            set { ViewState["DragDropStyleSheetUrl"] = value; }
        }

        protected virtual IEngine Engine
        {
            get { return N2.Context.Current; }
        }

        protected virtual PartsAdapter ZoneAdapter
        {
            get { return Engine.Resolve<IContentAdapterProvider>().ResolveAdapter<PartsAdapter>(CurrentItem); }
        }

        private ContentItem currentItem;
        public virtual ContentItem CurrentItem
        {
            get
            {
                if (currentItem != null)
                    return currentItem;

                int selectedItemID;
                if (int.TryParse(Page.Request[PathData.ItemQueryKey], out selectedItemID))
                {
                    return currentItem = Engine.Persister.Get(selectedItemID);
                }

                return currentItem = Find.ClosestItem(Parent);
            }
            set
            {
                currentItem = value;
            }
        }

        #endregion

        #region Page Methods

        private const string switchScriptFormat =
            @"
jQuery(document).ready(function(){{
    if(window.n2ctx){{
        n2ctx.refresh({{ path: '{0}', navigationUrl: '{2}', permission: '{3}', force:{4}, mode:'{5}' }});
        if (n2ctx.hasTop()) $('.complementary').hide();
        else $('.cpView').hide();
    }}
    if(window.n2SlidingCurtain) n2SlidingCurtain.recalculate();
}});";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.InitComplete += Page_InitComplete;
        }

        private void Page_InitComplete(object sender, EventArgs e)
        {
            EnsureChildControls();
        }

        protected override void CreateChildControls()
        {
			if (!Engine.Config.Sections.Management.Organize.UseLegacyControlPanel)
				return;

            ControlPanelState state = GetState(Page.GetEngine());

            if (state.IsFlagSet(ControlPanelState.Hidden))
            {
                AppendDefinedTemplate(HiddenTemplate, this);
                return;
            }

            Register.StyleSheet(Page, Register.DefaultIconsCssPath);


            if (state.IsFlagSet(ControlPanelState.Visible))
                AppendDefinedTemplate(VisibleHeaderTemplate, this);
            if (state.IsFlagSet(ControlPanelState.DragDrop))
                AppendDefinedTemplate(DragDropHeaderTemplate, this);
            if (state.IsFlagSet(ControlPanelState.Editing))
                AppendDefinedTemplate(EditingHeaderTemplate, this);
            if (state.IsFlagSet(ControlPanelState.Previewing))
                AppendDefinedTemplate(PreviewingHeaderTemplate, this);

            AddPlugins(state);

            if (state.IsFlagSet(ControlPanelState.DragDrop))
            {
                AddDefinitions(this);
                RegisterDragDropStyles();
                RegisterDragDropScripts();
            }
            if (state.IsFlagSet(ControlPanelState.Editing))
            {
                Register.JQuery(Page);
                Register.StyleSheet(Page, Url.ToAbsolute(StyleSheetUrl), Media.All);
            }

            if (state.IsFlagSet(ControlPanelState.Previewing))
                AppendDefinedTemplate(PreviewingFooterTemplate, this);
            if (state.IsFlagSet(ControlPanelState.Editing))
                AppendDefinedTemplate(EditingFooterTemplate, this);
            if (state.IsFlagSet(ControlPanelState.DragDrop))
                AppendDefinedTemplate(DragDropFooterTemplate, this);
            if (state.IsFlagSet(ControlPanelState.Visible))
                AppendDefinedTemplate(VisibleFooterTemplate, this);

            base.CreateChildControls();
        }

        protected virtual void AddDefinitions(Control container)
        {
            var definitions = new HtmlGenericControl("div");
            definitions.Attributes["class"] = "definitions";
            container.Controls.Add(definitions);

            var sortedDefinitions = GetPartDefinitions(ZoneAdapter, CurrentItem, Page.Items[Zone.PageKey] as IList<Zone>, Page.User);

            foreach (ItemDefinition definition in sortedDefinitions)
            {
                var div = new HtmlGenericControl("div");
                div.Attributes["title"] = definition.ToolTip;
                div.Attributes["id"] = definition.Discriminator;
                div.Attributes[PartUtilities.TypeAttribute] = definition.Discriminator;
                div.Attributes["class"] = "definition " + definition.Discriminator;
                div.InnerHtml = FormatImageAndText(Url.ResolveTokens(definition.IconUrl), definition.IconClass, definition.Title);
                definitions.Controls.Add(div);
            }
        }

        /// <summary>Gets part definitions that can be added to the given page.</summary>
        /// <param name="adapter"></param>
        /// <param name="item"></param>
        /// <param name="pageZones"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static IEnumerable<ItemDefinition> GetPartDefinitions(PartsAdapter adapter, ContentItem item, IEnumerable<Zone> pageZones, IPrincipal user)
        {
            IEnumerable<ItemDefinition> availableDefinitions;

            if (pageZones == null)
                availableDefinitions = adapter.GetAllowedDefinitions(item, user);
            else
                availableDefinitions = GetPossibleDefinitions(adapter, pageZones, user);

			return availableDefinitions
				.OrderBy(d => d.SortOrder)
				.ThenBy(d => d.Title);
        }

        private static List<ItemDefinition> GetPossibleDefinitions(PartsAdapter adapter, IEnumerable<Zone> pageZones, IPrincipal user)
        {
            var availableDefinitions = new List<ItemDefinition>();
            foreach (Zone z in pageZones)
            {
                ContentItem item = z.CurrentItem;
                string zoneName = z.ZoneName;
                if (item == null || string.IsNullOrEmpty(zoneName)) continue;

                foreach (ItemDefinition definition in adapter.GetAllowedDefinitions(item, zoneName, user))
                {
                    if (!availableDefinitions.Contains(definition))
                        availableDefinitions.Add(definition);
                }
            }
            return availableDefinitions;
        }

        private void RegisterDragDropStyles()
        {
            Register.StyleSheet(Page, DragDropStyleSheetUrl, Media.All);
        }

        private void RegisterDragDropScripts()
        {
            Register.JavaScript(Page, Register.SelectedQueryKeyRegistrationScript(), ScriptPosition.Header, ScriptOptions.ScriptTags | ScriptOptions.Prioritize);
            Register.JQueryUi(Page);
            Register.JavaScript(Page, DragDropScriptUrl);

            Register.JavaScript(Page, DragDropScriptInitialization(CurrentItem), ScriptOptions.DocumentReady);
        }

        protected virtual void AddPlugins(ControlPanelState state)
        {
            var pluginPanel = new Panel();
            pluginPanel.CssClass = "plugins";
            Controls.Add(pluginPanel);

            ContentItem start = Engine.Resolve<IUrlParser>().StartPage;
            ContentItem root = Engine.Persister.Repository.Get(Engine.Resolve<IHost>().CurrentSite.RootItemID);
            foreach (IControlPanelPlugin plugin in Engine.Resolve<IPluginFinder>().GetPlugins<IControlPanelPlugin>())
            {
                plugin.AddTo(pluginPanel, new PluginContext(new SelectionUtility(CurrentItem, null), start, root, state, Engine, new HttpContextWrapper(Context)));
            }
        }

        protected void AppendDefinedTemplate(ITemplate template, Control container)
        {
            if (template != null)
            {
                Control templateContainer = new SimpleTemplateContainer();
                container.Controls.Add(templateContainer);

                template.InstantiateIn(templateContainer);
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
			if (!Engine.Config.Sections.Management.Organize.UseLegacyControlPanel)
			{
				var cph = new Mvc.Html.ControlPanelExtensions.ControlPanelHelper(Engine, CurrentItem, writer, Page.Items["StateCollection"] as HashSet<string> ?? (Page.Items["StateCollection"] = new HashSet<string>()) as HashSet<string>);
				cph.Render();
                return;
            }

            IDictionary<string, IList<string>> arrays = GetArrays(Page);
            writer.WriteLineNoTabs(@"<script type='text/javascript'>//<!--");
            if (arrays.Count > 0)
            {
                foreach (var pair in arrays)
                {
                    IList<string> array = pair.Value;
                    writer.Write("var " + pair.Key + " = [" + array[0]);
                    for (int i = 1; i < array.Count; i++)
                    {
                        writer.Write("," + array[i]);
                    }
                    writer.WriteLineNoTabs("];");
                }
            }
            if (EnableEditInterfaceIntegration)
            {
                writer.WriteLineNoTabs("if(window.n2ctx){");
                writer.WriteLineNoTabs("n2ctx.select('preview');");
                if (CurrentItem != null)
                {
                    var adapter = Engine.GetContentAdapter<NodeAdapter>(CurrentItem);
                    string navigationUrl = Engine.ManagementPaths.GetNavigationUrl(CurrentItem);
                    string previewUrl = adapter.GetPreviewUrl(CurrentItem);
                    string script = string.Format(switchScriptFormat, 
                        CurrentItem.Path, // 0
                        previewUrl, // 1
                        navigationUrl, // 2
                        adapter.GetMaximumPermission(CurrentItem), // 3
                        (Page.Request["refresh"] == "true").ToString().ToLower(), // 4
                        GetState(Page.GetEngine()).ToString());
                    writer.WriteLineNoTabs(script);
                }
                writer.WriteLineNoTabs("}");
            }

            writer.Write(@"//--></script>");

            writer.Write("<div class='controlPanel state" + CurrentItem.State.ToString() + "'>");
            base.Render(writer);
            writer.Write("</div>");
		}

        #endregion

        #region Methods

        protected bool OriginatesFromEdit()
        {
            if (Page.Request.UrlReferrer == null)
                return false;

            string editUrl = N2.Context.Current.ManagementPaths.GetEditInterfaceUrl();
            string currentUrl = Page.Request.UrlReferrer.PathAndQuery;
            return currentUrl.StartsWith(editUrl, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>Gets the url for editing the page directly.</summary>
        public virtual string GetQuickEditUrl(string editParameter)
        {
            return Url.Parse(Page.Request.RawUrl).SetQueryParameter("edit", editParameter);
        }

        #endregion

        #region Static Methods

        public static ControlPanelState GetState(Control control)
        {
            if (HttpContext.Current != null)
                return GetState(HttpContext.Current.GetEngine());

            return ControlPanelState.Unknown;
        }

        [Obsolete("Use overload with IWebContext parameter")]
        public static ControlPanelState GetState(ISecurityManager security, IPrincipal user, NameValueCollection queryString)
        {
            return GetState(security, HttpContext.Current.GetEngine().RequestContext);
        }

        [Obsolete("Use overload with ISecurityManager, IWebContext, ContentItem and IPrincipal parameters")]
        public static ControlPanelState GetState(IPrincipal user, NameValueCollection queryString)
        {
            return GetState(HttpContext.Current.GetEngine());
        }

        [Obsolete("Use overload with ISecurityManager, IWebContext, ContentItem and IPrincipal parameters")]
        public static ControlPanelState GetState(ISecurityManager security, IWebContext request)
        {
            return GetState(security, request.Url.GetQueries().ToNameValueCollection(), request.CurrentPath.CurrentItem, request.User);
        }

        public static ControlPanelState GetState(IEngine engine)
        {
            var request = engine.RequestContext;
            return GetState(engine.SecurityManager, request.Url.GetQueries().ToNameValueCollection(), request.CurrentPath.CurrentItem, request.User);
        }

        public static ControlPanelState GetState(ISecurityManager security, NameValueCollection queryString, ContentItem item, IPrincipal user)
        {
            if (!security.IsEditor(user))
                return ControlPanelState.Hidden;

            var state = ControlPanelState.Visible;

            if (queryString["edit"] == "true")
                state |= ControlPanelState.Editing;
            if (queryString["edit"] == "drag")
                state |= ControlPanelState.DragDrop;
            if (item != null && (item.State == ContentState.Draft || item.VersionOf.HasValue))
                state |= ControlPanelState.Previewing;

            return state;

        }

        public static string FormatImageAndText(string iconUrl, string iconClass, string text)
        {
            const string C_SPRITE = "sprite:";
            string icon = "";
            if (!string.IsNullOrEmpty(iconUrl))
            {
                if (iconUrl.StartsWith(C_SPRITE))
                    icon = string.Format(@"<span class=""{0} sprite""></span>", (iconUrl.Split('-').LastOrDefault() ?? string.Empty).ToLower());
                else
                    icon = string.Format(@"<img src=""{0}"" alt=""icon"" />", iconUrl);
            }
            else if (!string.IsNullOrEmpty(iconClass))
            {
                icon = string.Format(@"<b class=""{0}""></b>", iconClass);
            }
            
            if (string.IsNullOrEmpty(text))
                return icon;
            else
                return icon + " " + text;
        }

        public static void RegisterArrayValue(Page page, string key, string value)
        {
            IList<string> array = GetArray(key, page);
            array.Add(value);
        }

        private static IList<string> GetArray(string key, Page page)
        {
            IDictionary<string, IList<string>> arrays = GetArrays(page);

            IList<string> array;
            if (arrays.ContainsKey(key))
                array = arrays[key];
            else
                arrays[key] = array = new List<string>();
            return array;
        }

        private static IDictionary<string, IList<string>> GetArrays(Page page)
        {
            var arrays = page.Items[ArrayKey] as IDictionary<string, IList<string>>;
            if (arrays == null)
                page.Items[ArrayKey] = arrays = new Dictionary<string, IList<string>>();
            return arrays;
        }

        #endregion

        #region Templates
        [DefaultValue((string) null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty),
         TemplateContainer(typeof (SimpleTemplateContainer))]
        public virtual ITemplate HiddenTemplate { get; set; }

        [DefaultValue((string) null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty),
         TemplateContainer(typeof (SimpleTemplateContainer))]
        public virtual ITemplate VisibleHeaderTemplate { get; set; }

        [DefaultValue((string) null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty),
         TemplateContainer(typeof (SimpleTemplateContainer))]
        public virtual ITemplate VisibleFooterTemplate { get; set; }

        [DefaultValue((string) null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty),
         TemplateContainer(typeof (SimpleTemplateContainer))]
        public virtual ITemplate EditingHeaderTemplate { get; set; }

        [DefaultValue((string) null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty),
         TemplateContainer(typeof (SimpleTemplateContainer))]
        public virtual ITemplate EditingFooterTemplate { get; set; }

        [DefaultValue((string) null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty),
         TemplateContainer(typeof (SimpleTemplateContainer))]
        public virtual ITemplate PreviewingHeaderTemplate { get; set; }

        [DefaultValue((string) null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty),
         TemplateContainer(typeof (SimpleTemplateContainer))]
        public virtual ITemplate PreviewingFooterTemplate { get; set; }

        [DefaultValue((string) null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty),
         TemplateContainer(typeof (SimpleTemplateContainer))]
        public virtual ITemplate DragDropHeaderTemplate { get; set; }

        [DefaultValue((string) null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty),
         TemplateContainer(typeof (SimpleTemplateContainer))]
        public virtual ITemplate DragDropFooterTemplate { get; set; }
        #endregion

        public static string DragDropScriptInitialization(ContentItem item)
        {
            return string.Format(@"window.n2ddcp = new n2DragDrop({{ copy:'{0}/Resources/Js/copy.n2.ashx', move:'{0}/Resources/Js/move.n2.ashx', remove:'{0}/Resources/Js/remove.n2.ashx', create:'{0}/Resources/Js/create.n2.ashx', editsingle:'{0}/Content/EditSingle.aspx'}}, {{}},{{versionIndex:{1}, isMasterVersion:{2}, pagePath: '{3}'}});", Url.ResolveTokens("{ManagementUrl}"), item.VersionIndex, !item.VersionOf.HasValue ? "true" : "false", Find.ClosestPage(item).Path);
        }
    }
}
