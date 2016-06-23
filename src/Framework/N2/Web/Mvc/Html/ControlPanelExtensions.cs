using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;

using N2;
using N2.Edit;
using N2.Engine;
using N2.Plugin;
using N2.Resources;
using N2.Web.Parts;
using N2.Web.UI.WebControls;
using N2.Edit.Versioning;
using N2.Management.Api;

namespace N2.Web.Mvc.Html
{
    public static class ControlPanelExtensions
    {
        public static Func<HtmlHelper, ControlPanelHelper> ControlPanelFactory { get; set; }

		static bool? legacyEnabled = null;
        static ControlPanelExtensions()
        {
            ControlPanelFactory = (html) =>
				(legacyEnabled ?? (legacyEnabled = html.ContentEngine().Config.Sections.Management.Organize.UseLegacyControlPanel) ?? false)
					? new LegacyControlPanelHelper(html)
					: new ControlPanelHelper(html.ContentEngine(), html.CurrentItem(), html.ViewContext.Writer, html.ViewContext.GetResourceStateCollection());
        }

        /// <summary>Gets the curent state of the control panel.</summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static ControlPanelState GetControlPanelState(this HtmlHelper html)
        {
            return UI.WebControls.ControlPanel.GetState(html.ContentEngine());
        }

        /// <summary>Renders the openable control panel displayed in the upper left corner on N2 sites.</summary>
        /// <param name="html"></param>
        public static ControlPanelHelper ControlPanel(this HtmlHelper html)
        {
            var cp = ControlPanelFactory(html);

			if (RegistrationExtensions.GetRegistrationExpression(html) != null)
				return new EmptyControlPanelHelper();
			if (html.ViewContext.HttpContext.Request["refresh"] == "true")
                cp = cp.ForceRefreshNavigation();
            return cp;
        }

        /// <summary>Renders the openable control panel displayed in the upper left corner on N2 sites.</summary>
        /// <param name="html"></param>
        public static void RenderControlPanel(this HtmlHelper html)
        {
            html.ControlPanel().WriteTo(html.ViewContext.Writer);
        }

        public static string Replace(this string format, IDictionary<string, object> replacements)
        {
            foreach (var kvp in replacements)
            {
                format = format.Replace("{" + kvp.Key + "}", kvp.Value != null ? kvp.Value.ToString() : "");
            }
            return format;
        }

        public static string Replace(this string format, object replacements)
        {
            return format.Replace(new RouteValueDictionary(replacements));
        }

		public class LegacyControlPanelHelper : ControlPanelHelper
		{
			private bool includeJQueryPlugins = true;
			private bool includeJQueryUi = true;
			
			public LegacyControlPanelHelper(HtmlHelper html)
				: base(html.ContentEngine(), html.CurrentItem(), html.ViewContext.Writer, html.ViewContext.GetResourceStateCollection())
			{
				this.Html = html;
			}

			public bool IncludeJQueryUi
			{
				get { return includeJQueryUi; }
				set { includeJQueryUi = value; }
			}

			public bool IncludeJQueryPlugins
			{
				get { return includeJQueryPlugins; }
				set { includeJQueryPlugins = value; }
			}

			/// <summary>Is used to instruct the control panel helper to render less javascript and css resources.</summary>
			/// <param name="jQuery"></param>
			/// <param name="jQueryPlugins"></param>
			/// <param name="partScripts"></param>
			/// <param name="partStyles"></param>
			/// <returns></returns>
			[Obsolete("Use Configure(c => c.IncludeJQuery = true)")]
			public ControlPanelHelper Includes(bool jQuery = true, bool jQueryPlugins = true, bool partScripts = true, bool partStyles = true)
			{
				IncludeJQuery = jQuery;
				IncludeJQueryPlugins = jQueryPlugins;
				IncludeJQueryUi = jQueryPlugins;
				IncludePartScripts = partScripts;
				IncludePartStyles = partStyles;

				return this;
			}

			protected override  void AppendControlPanel(TextWriter writer, IEngine engine, ContentItem item)
			{
				var state = ControlPanelExtensions.GetControlPanelState(Html);
				var settings = new
				{
					NavigationUrl = engine.ManagementPaths.GetNavigationUrl(item),
					ManagementUrl = engine.ManagementPaths.GetManagementInterfaceUrl(),
					Path = item.Path,
					Plugins = Plugins(Html, item, state),
					Definitions = Definitions(Html, engine, item, state),
					Version = typeof(ContentItem).Assembly.GetName().Version.ToString(),
					Permission = engine.GetContentAdapter<NodeAdapter>(item).GetMaximumPermission(item),
					VersionIndex = item.VersionIndex,
					VersionKey = item.GetVersionKey(),
					Force = ForceRefreshNavigationOnLoad ? "true" : "false",
					State = item != null ? item.State.ToString() : "NonContent",
					Mode = GetControlPanelState(Html).ToString()
				};

				var resources = Html.Resources(writer).Constants();
				if (IncludeJQuery) resources.JQuery();
				if (IncludeJQueryPlugins) resources.JQueryPlugins(IncludeJQuery);
				if (IncludeJQueryUi) resources.JQueryUi(IncludeJQuery);
				if (IncludePartScripts) resources.PartsJs();
				if (IncludePartStyles) { resources.PartsCss(); resources.IconsCss(); }

				if (RefreshNavigationOnLoad)
					resources.HtmlLiteral(formatWithRefresh.Replace(settings));
				else
					resources.HtmlLiteral(formatWithoutRefresh.Replace(settings));

				if (state.IsFlagSet(ControlPanelState.DragDrop))
					resources.JavaScript(UI.WebControls.ControlPanel.DragDropScriptInitialization(item), ScriptOptions.DocumentReady);
				resources.Render(writer);
			}

			private static string Plugins(HtmlHelper html, ContentItem item, ControlPanelState state)
			{
				ContentItem start = html.StartPage();
				ContentItem root = html.RootPage();

				Page p = new Page();
				foreach (IControlPanelPlugin plugin in html.ContentEngine().Resolve<IPluginFinder>().GetPlugins<IControlPanelPlugin>())
				{
					plugin.AddTo(p, new PluginContext(new SelectionUtility(item, null), start, root, state, html.ContentEngine(), html.ViewContext.HttpContext));
				}

				using (var sw = new StringWriter())
				using (var htw = new HtmlTextWriter(sw))
				{
					p.RenderControl(htw);
					return sw.ToString();
				}
			}

			private static string Definitions(HtmlHelper html, IEngine engine, ContentItem item, ControlPanelState state)
			{
				if (state.IsFlagSet(ControlPanelState.DragDrop))
				{
					StringBuilder sb = new StringBuilder();

					var a = engine.ResolveAdapter<PartsAdapter>(item);
					foreach (var d in UI.WebControls.ControlPanel.GetPartDefinitions(a, item, null, html.ViewContext.HttpContext.User))
					{
						foreach (var t in a.GetTemplates(item, d))
						{
							sb.AppendFormat(
								@"<div id=""{0}"" title=""{1}"" data-type=""{2}"" data-template=""{3}"" class=""{4}"">{5}</div>",
								/*{0}*/ t.Definition.ToString().Replace('/', '-'),
								/*{1}*/ t.Description,
								/*{2}*/ t.Definition.Discriminator,
								/*{3}*/ t.Name,
								/*{4}*/ "definition " + t.Definition.Discriminator,
								/*{5}*/ UI.WebControls.ControlPanel.FormatImageAndText(t.Definition.IconUrl, t.Definition.IconClass, t.Title));
						}
					}

					if (sb.Length > 0)
						return @"<div class=""definitions"">" + sb + "</div>";
				}
				return "";
			}

			#region Format
			static string format1 = @"
<script type='text/javascript'>//<![CDATA[
(function($){
    if (!window.n2ctx) return;

    n2ctx.select('preview');
    $(document).ready(function () {";
			static string format2 = @"
        n2ctx.refresh({ navigationUrl: '{NavigationUrl}', path: '{Path}', permission: '{Permission}', force: {Force}, versionIndex:{VersionIndex}, versionKey:'{VersionKey}', mode: '{Mode}' });";
			static string format3 = @"
        if (n2ctx.hasTop()) $('.complementary').hide();
        else $('.cpView').hide();
                
        if (window.n2SlidingCurtain) {
            n2SlidingCurtain.init('#cpCurtain', false);
            n2SlidingCurtain.recalculate();
            if($.browser.webkit) setTimeout(function(){ n2SlidingCurtain.recalculate(); }, 50);
        }
    });
})(jQuery);
//]]></script>

<div id=""cpCurtain"" class=""sc state{State}""><div class=""scContent"">
    <div class=""controlPanel"">
        <div class=""plugins"">
            {Plugins}
        </div>
        {Definitions}
    </div>
    <a href=""javascript:void(0);"" class=""close sc-toggler"" title=""Close"">&laquo;</a>
    <a href=""javascript:void(0);"" class=""open sc-toggler"" title=""Open"">&raquo;</a>
</div></div>
";
			static string formatWithRefresh = format1 + format2 + format3;
			static string formatWithoutRefresh = format1 + format3;
			private HtmlHelper Html;

			#endregion
		}

		public abstract class ControlPanelHelperBase : IHtmlString
		{
			public IEngine Engine { get; set; }
			public ContentItem CurrentItem { get; set; }
			public TextWriter Writer { get; set; }

			public virtual string ToHtmlString()
			{
				return ToString();
			}

			public override string ToString()
			{
				using (var tw = new StringWriter())
				{
					WriteTo(tw);
					return tw.ToString();
				}
			}

			public string Render()
			{
				WriteTo(Writer);
				return "";
			}

			public abstract void WriteTo(TextWriter writer);
		}

		internal class EmptyControlPanelHelper : ControlPanelHelper
		{
			public override void WriteTo(TextWriter writer)
			{
			}
		}

		public class ControlPanelHelper : ControlPanelHelperBase
		{
			internal ControlPanelHelper()
			{
			}

			public ControlPanelHelper(IEngine engine, ContentItem contentItem, TextWriter writer, ICollection<string> stateCollection)
			{
				Engine = engine;
				CurrentItem = contentItem;
				Writer = writer;
				StateCollection = stateCollection;
				RefreshNavigationOnLoad = true;
				IncludeJQuery = true;
				IncludePartScripts = true;
				IncludePartStyles = true;
            }

			public bool RefreshNavigationOnLoad { get; set; }

			public bool ForceRefreshNavigationOnLoad { get; set; }

            public bool IncludeJQuery { get; set; }

			public bool IncludePartScripts { get; set; }

			public bool IncludePartStyles { get; set; }

			public ICollection<string> StateCollection { get; private set; }

			/// <summary>Configures the control panel calling the given lambda expression.</summary>
			/// <param name="config">The configuration expression.</param>
			/// <returns>The same instance.</returns>
			public ControlPanelHelper Configure(Action<ControlPanelHelper> config)
            {
                config(this);
                return this;
            }

            /// <summary>Is used to instruct the control panel helper not to refresh navigation to the current page.</summary>
            /// <param name="refreshNavigation"></param>
            /// <returns></returns>
            public ControlPanelHelper RefreshNavigation(bool refreshNavigation = true)
            {
                this.RefreshNavigationOnLoad = refreshNavigation;
                return this;
            }

            /// <summary>Is used to instruct the control panel helper not to refresh navigation to the current page.</summary>
            /// <param name="refreshNavigation"></param>
            /// <returns></returns>
            public ControlPanelHelper ForceRefreshNavigation(bool forceRefreshNavigation = true)
            {
                this.ForceRefreshNavigationOnLoad = forceRefreshNavigation;
                return this;
            }

            /// <summary>Sets the selected item control panel plugins are bound to.</summary>
            /// <param name="currentItem"></param>
            /// <returns></returns>
            public ControlPanelHelper Selected(ContentItem currentItem)
            {
                CurrentItem = currentItem;
                return this;
            }
            
            public override void WriteTo(TextWriter writer)
			{
				if (!Engine.RequestContext.User.Identity.IsAuthenticated || !Engine.SecurityManager.IsEditor(Engine.RequestContext.User))
					return;

				var item = CurrentItem ?? Engine.UrlParser.CurrentPage ?? Engine.UrlParser.StartPage;

				AppendControlPanel(writer, Engine, item);
			}

			protected virtual void AppendControlPanel(TextWriter writer, IEngine engine, ContentItem item)
			{
                writer.Write("<script>n2 = window.n2 || {};");
				writer.Write("n2.settings = ");
				var settings = engine.Resolve<InterfaceBuilder>().GetControlPanelDefinition(engine.RequestContext.HttpContext, item);
				settings.ToJson(writer);
				writer.Write(";</script>");

				var resources = new ResourcesExtensions.ResourcesHelper() { StateCollection = StateCollection, Writer = writer };
				resources = resources.Constants();
				if (IncludeJQuery) resources = resources.JQuery();
				if (IncludePartScripts) resources = resources.JavaScript("{ManagementUrl}/App/Preview/PreviewBoostrapper.js");
                if (IncludePartStyles) resources = resources.IconsCss().StyleSheet("{ManagementUrl}/App/Preview/Preview.css");
				resources.Render(writer);
			}
		}
    }
}
