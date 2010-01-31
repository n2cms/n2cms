using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using N2.Definitions;
using N2.Resources;
using N2.Engine;
using N2.Edit;
using N2.Plugin;
using N2.Web.Parts;
using System.Collections;
using System.Collections.Specialized;

namespace N2.Web.UI.WebControls
{
	/// <summary>
	/// A control panel for on page editing. The control displays buttons for 
	/// start editing and saving changes.
	/// </summary>
	[PersistChildren(false)]
	[ParseChildren(true)]
    [ControlPanelLink("cpOrganize", "~/N2/Resources/Img/ico/png/layout_edit.png", "{Selected.Url}", "Organize parts", -10, ControlPanelState.Visible, 
        UrlEncode = false, 
        NavigateQuery = "edit=drag")]
	[ControlPanelLink("cpUnorganize", "~/N2/Resources/Img/ico/png/page_refresh.png", "{Selected.Url}", "Done", -10, ControlPanelState.DragDrop, 
        UrlEncode = false, 
        Title = "Done")]
	public class ControlPanel : Control, IItemContainer
	{
		const string ArrayKey = "ControlPanel.arrays";

		#region Properties

        public bool EnableEditInterfaceIntegration
        {
            get { return (bool)(ViewState["EnableEditInterfaceIntegration"] ?? true); }
            set { ViewState["EnableEditInterfaceIntegration"] = value; }
        }

		/// <summary>Gets or sets the url to a style sheet added to the page when editing.</summary>
		public string StyleSheetUrl
		{
			get { return (string)(ViewState["StyleSheetUrl"] ?? "~/N2/Resources/Css/edit.css"); }
			set { ViewState["StyleSheetUrl"] = value; }
		}

		public string DragDropScriptUrl
		{
			get { return (string)(ViewState["DragDropScriptUrl"] ?? "~/N2/Resources/Js/parts.js"); }
			set { ViewState["DragDropScriptUrl"] = value; }
		}

		public string DragDropStyleSheetUrl
		{
			get { return (string)(ViewState["DragDropStyleSheetUrl"] ?? "~/N2/Resources/Css/Parts.css"); }
			set { ViewState["DragDropStyleSheetUrl"] = value; }
		}

		protected virtual IEngine Engine
		{
			get { return N2.Context.Current; }
		}

		protected virtual PartsAdapter ZoneAdapter
		{
			get { return Engine.Resolve<IContentAdapterProvider>().ResolveAdapter<PartsAdapter>(CurrentItem.GetType()); }
		}

		public virtual ContentItem CurrentItem
		{
			get { return Find.CurrentPage; }
		}

		#endregion

		#region Page Methods

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.Page.InitComplete += Page_InitComplete;
		}

		void Page_InitComplete(object sender, EventArgs e)
		{
            EnsureChildControls();
		}

        protected override void CreateChildControls()
        {
			ControlPanelState state = GetState(Page.User, Page.Request.QueryString);

			if(state == ControlPanelState.Hidden)
			{
				AppendDefinedTemplate(HiddenTemplate, this);
			}
			else if (state == ControlPanelState.Visible)
			{
				AppendDefinedTemplate(VisibleHeaderTemplate, this);
				AddPlugins(state);
				AppendDefinedTemplate(VisibleFooterTemplate, this);
            }
			else if (state == ControlPanelState.DragDrop)
			{
				AppendDefinedTemplate(DragDropHeaderTemplate, this);
				AddPlugins(state);
				AddDefinitions(this);
				AppendDefinedTemplate(DragDropFooterTemplate, this);
				RegisterDragDropStyles();
				RegisterDragDropScripts();
                
                //Page.Response.CacheControl = "no-cache";
			}
			else if (state == ControlPanelState.Editing)
			{
				AppendDefinedTemplate(EditingHeaderTemplate, this);
				AddPlugins(state);
				Register.JQuery(Page);
				Register.StyleSheet(Page, Url.ToAbsolute(StyleSheetUrl), Media.All);
				AppendDefinedTemplate(EditingFooterTemplate, this);
			}
			else if (state == ControlPanelState.Previewing)
			{
				AppendDefinedTemplate(PreviewingHeaderTemplate, this);
				AddPlugins(state);
				AppendDefinedTemplate(PreviewingFooterTemplate, this);
			}
			else
				throw new N2Exception("Unknown control panel state: " + state);

            base.CreateChildControls();
		}

		protected virtual void AddDefinitions(Control container)
		{
			HtmlGenericControl definitions = new HtmlGenericControl("div");
			definitions.Attributes["class"] = "definitions";
			container.Controls.Add(definitions);

			IEnumerable<ItemDefinition> availableDefinitions;

			IList<Zone> pageZones = Page.Items[Zone.PageKey] as IList<Zone>;
			if (pageZones == null)
				availableDefinitions = ZoneAdapter.GetAllowedDefinitions(CurrentItem, Page.User);
			else
				availableDefinitions = GetPossibleDefinitions(pageZones);

			foreach (ItemDefinition definition in availableDefinitions)
			{
				HtmlGenericControl div = new HtmlGenericControl("div");
				div.Attributes["title"] = definition.ToolTip;
                div.Attributes["id"] = definition.Discriminator;
                div.Attributes[PartUtilities.TypeAttribute] = definition.Discriminator;
                div.Attributes["class"] = "definition " + definition.Discriminator;
				div.InnerHtml = FormatImageAndText(Url.ToAbsolute(definition.IconUrl), definition.Title);
				definitions.Controls.Add(div);
			}
		}

		private List<ItemDefinition> GetPossibleDefinitions(IList<Zone> pageZones)
		{
			List<ItemDefinition> availableDefinitions = new List<ItemDefinition>();
			foreach (Zone z in pageZones)
			{
				ContentItem item = z.CurrentItem;
				string zoneName = z.ZoneName;
				if (item == null || string.IsNullOrEmpty(zoneName)) continue;

				foreach (ItemDefinition definition in ZoneAdapter.GetAllowedDefinitions(item, zoneName, Page.User))
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
			Register.JQuery(Page);
			Register.JavaScript(Page, "~/N2/Resources/Js/jquery.ui.ashx");
			Register.JavaScript(Page, DragDropScriptUrl);

			Register.JavaScript(Page, @"
window.n2ddcp = new n2DragDrop();
", ScriptOptions.DocumentReady);
		}

		protected virtual void AddPlugins(ControlPanelState state)
		{
			Panel pluginPanel = new Panel();
			pluginPanel.CssClass = "plugins";
			Controls.Add(pluginPanel);

			foreach (IControlPanelPlugin plugin in Engine.Resolve<IPluginFinder>().GetPlugins<IControlPanelPlugin>())
			{
				plugin.AddTo(pluginPanel, new PluginContext(CurrentItem, null, state, Engine.EditManager.GetManagementInterfaceUrl()));
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
			var arrays = GetArrays(Page);
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
			if(EnableEditInterfaceIntegration)
			{
				writer.WriteLineNoTabs("if(window.n2ctx){");
				writer.WriteLineNoTabs("window.n2ctx.select('preview');");
				if (CurrentItem != null)
				{
					string navigationUrl = Engine.EditManager.GetNavigationUrl(CurrentItem);
					string previewUrl = Engine.EditManager.GetPreviewUrl(CurrentItem);
					string script = string.Format(switchScriptFormat, CurrentItem.Path, previewUrl, navigationUrl);
					writer.WriteLineNoTabs(script);
				}
				writer.WriteLineNoTabs("}");
			}

			writer.Write(@"//--></script>");

			writer.Write("<div class='controlPanel'>");
			base.Render(writer);
			writer.Write("</div>");
		}

		const string switchScriptFormat = @"
jQuery(document).ready(function(){{
    if(window.n2ctx){{
		n2ctx.refresh({{ navigationUrl: '{2}', path: '{0}', force:false }});
		if(n2ctx.hasTop()) jQuery('.cpAdminister').hide();
		else jQuery('.cpView').hide();
	}}
	if(window.n2SlidingCurtain) n2SlidingCurtain.recalculate();
}});";

		#endregion

		#region Methods

        protected bool OriginatesFromEdit()
        {
            if(Page.Request.UrlReferrer == null)
                return false;
            
            string editUrl = N2.Context.Current.EditManager.GetEditInterfaceUrl();
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
                return GetState(control.Page.User, control.Page.Request.QueryString);

            return ControlPanelState.Unknown;
        }

		public static ControlPanelState GetState(IPrincipal user, NameValueCollection queryString)
		{
			if (N2.Context.SecurityManager.IsEditor(user))
			{
                if (queryString["edit"] == "true")
					return ControlPanelState.Editing;
                if (queryString["edit"] == "drag")
					return ControlPanelState.DragDrop;
                if (!string.IsNullOrEmpty(queryString["preview"]))
					return ControlPanelState.Previewing;

				return ControlPanelState.Visible;
			}
			return ControlPanelState.Hidden;
		}

		protected string FormatImageAndText(string iconUrl, string text)
		{
			return string.Format("<img src='{0}' alt=''/>{1}", iconUrl, text);
		}

		public static void RegisterArrayValue(Page page, string key, string value)
		{
			IList<string> array = GetArray(key, page);
			array.Add(value);
		}

		static IList<string> GetArray(string key, Page page)
		{
			IDictionary<string, IList<string>> arrays = GetArrays(page);
			
			IList<string> array;
			if (arrays.ContainsKey(key))
				array = arrays[key];
			else
				arrays[key] = array = new List<string>();
			return array;
		}

		static IDictionary<string, IList<string>> GetArrays(Page page)
		{
			IDictionary<string, IList<string>> arrays = page.Items[ArrayKey] as IDictionary<string, IList<string>>;
			if(arrays == null)
				page.Items[ArrayKey] = arrays = new Dictionary<string, IList<string>>();
			return arrays;
		}

		#endregion

		[DefaultValue((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(SimpleTemplateContainer))]
		public virtual ITemplate HiddenTemplate { get; set; }

		[DefaultValue((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(SimpleTemplateContainer))]
		public virtual ITemplate VisibleHeaderTemplate { get; set; }
		[DefaultValue((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(SimpleTemplateContainer))]
		public virtual ITemplate VisibleFooterTemplate { get; set; }

		[DefaultValue((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(SimpleTemplateContainer))]
		public virtual ITemplate EditingHeaderTemplate { get; set; }
		[DefaultValue((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(SimpleTemplateContainer))]
		public virtual ITemplate EditingFooterTemplate { get; set; }

		[DefaultValue((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(SimpleTemplateContainer))]
		public virtual ITemplate PreviewingHeaderTemplate { get; set; }
		
		[DefaultValue((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(SimpleTemplateContainer))]
		public virtual ITemplate PreviewingFooterTemplate { get; set; }
		
		[DefaultValue((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(SimpleTemplateContainer))]
		public virtual ITemplate DragDropHeaderTemplate { get; set; }
		[DefaultValue((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(SimpleTemplateContainer))]
		public virtual ITemplate DragDropFooterTemplate { get; set; }

	}
}