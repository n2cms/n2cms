using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using N2.Edit;
using N2.Engine;
using N2.Plugin;
using N2.Resources;
using N2.Web.Parts;
using N2.Web.UI.WebControls;

namespace N2.Web.Mvc.Html
{
	public static class ControlPanelExtensions
	{
		public static Func<HtmlHelper, ControlPanelHelper> ControlPanelFactory { get; set; }

		static ControlPanelExtensions()
		{
			ControlPanelFactory = (html) => new ControlPanelHelper { Html = html };
		}

		/// <summary>Gets the curent state of the control panel.</summary>
		/// <param name="html"></param>
		/// <returns></returns>
		public static ControlPanelState GetControlPanelState(HtmlHelper html)
		{
			return UI.WebControls.ControlPanel.GetState(html.ContentEngine().SecurityManager, html.ViewContext.HttpContext.User, html.ViewContext.HttpContext.Request.QueryString);
		}

		/// <summary>Renders the openable control panel displayed in the upper left corner on N2 sites.</summary>
		/// <param name="html"></param>
		public static ControlPanelHelper ControlPanel(this HtmlHelper html)
		{
			return ControlPanelFactory(html);
		}

		/// <summary>Renders the openable control panel displayed in the upper left corner on N2 sites.</summary>
		/// <param name="html"></param>
		public static void RenderControlPanel(this HtmlHelper html)
		{
			ControlPanelFactory(html).WriteTo(html.ViewContext.Writer);
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

		public class ControlPanelHelper
		{
			private bool refreshNavigation = true;
			private bool includeJQuery = true;
			private bool includeJQueryPlugins = true;
			private bool includeJQueryUi = true;
			private bool includePartScripts = true;
			private bool includePartStyles = true;
			private ContentItem currentItem;

			public HtmlHelper Html { get; set; }

			public ContentItem CurrentItem
			{
				get { return currentItem; }
				set { currentItem = value; }
			}

			public bool RefreshNavigationOnLoad
			{
				get { return refreshNavigation; }
				set { refreshNavigation = value; }
			}

			public bool IncludeJQuery
			{
				get { return includeJQuery; }
				set { includeJQuery = value; }
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

			public bool IncludePartScripts
			{
				get { return includePartScripts; }
				set { includePartScripts = value; }
			}

			public bool IncludePartStyles
			{
				get { return includePartStyles; }
				set { includePartStyles = value; }
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
                includeJQuery = jQuery;
                includeJQueryPlugins = jQueryPlugins;
				includeJQueryUi = jQueryPlugins;
                includePartScripts = partScripts;
                includePartStyles = partStyles;

                return this;
            }

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
				this.refreshNavigation = refreshNavigation;
				return this;
			}

            /// <summary>Sets the selected item control panel plugins are bound to.</summary>
            /// <param name="currentItem"></param>
            /// <returns></returns>
			public ControlPanelHelper Selected(ContentItem currentItem)
			{
				this.currentItem = currentItem;
				return this;
			}
            
			public override string ToString()
			{
				using (var tw = new StringWriter())
				{
					WriteTo(tw);
					return tw.ToString();
				}
			}

			#region IHtmlString Members

			public string ToHtmlString()
			{
				return ToString();
			}

			#endregion

			public void Render()
			{
				WriteTo(Html.ViewContext.Writer);
			}

			public virtual void WriteTo(TextWriter writer)
			{
				var engine = Html.ContentEngine();

                if (!Html.ViewContext.HttpContext.User.Identity.IsAuthenticated || !engine.SecurityManager.IsEditor(Html.ViewContext.HttpContext.User))
                    return;
				if (RegistrationExtensions.GetRegistrationExpression(Html) != null)
					return;

                var item = currentItem ?? Html.CurrentItem() ?? Html.StartPage();

				var state = ControlPanelExtensions.GetControlPanelState(Html);
				var settings = new
				{
					NavigationUrl = engine.ManagementPaths.GetNavigationUrl(item),
					ManagementUrl = engine.ManagementPaths.GetManagementInterfaceUrl(),
					Path = item.Path,
					Plugins = Plugins(Html, item, state),
					Definitions = Definitions(Html, engine, item, state),
					Version = typeof(ContentItem).Assembly.GetName().Version.ToString(),
					Permission = engine.GetContentAdapter<NodeAdapter>(item).GetMaximumPermission(item)
				};

                var resources = Html.Resources(writer);
                if (includeJQuery) resources.JQuery();
				if (includeJQueryPlugins) resources.JQueryPlugins(includeJQuery);
				if (includeJQueryUi) resources.JQueryUi(includeJQuery);
                if (includePartScripts) resources.PartsJs();
                if (includePartStyles) resources.PartsCss();

				if (refreshNavigation)
					writer.Write(formatWithRefresh.Replace(settings));
				else
					writer.Write(formatWithoutRefresh.Replace(settings));

				if (state == ControlPanelState.DragDrop)
					Html.Resources().JavaScript(UI.WebControls.ControlPanel.DragDropScriptInitialization(), ScriptOptions.DocumentReady);
			}

			private static string Plugins(HtmlHelper html, ContentItem item, ControlPanelState state)
			{
				ContentItem start = html.StartPage();
				ContentItem root = html.RootPage();

				Page p = new Page();
				foreach (IControlPanelPlugin plugin in html.ContentEngine().Resolve<IPluginFinder>().GetPlugins<IControlPanelPlugin>())
				{
					var span = new System.Web.UI.HtmlControls.HtmlGenericControl("span");
					span.Attributes["class"] = "control";
					var pluginControl = plugin.AddTo(span, new PluginContext(new SelectionUtility(item, null), start, root, state, html.ContentEngine(), html.ViewContext.HttpContext));

					if (pluginControl != null)
						p.Controls.Add(span);
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
				if (state == ControlPanelState.DragDrop)
				{
					StringBuilder sb = new StringBuilder();

					var a = engine.ResolveAdapter<PartsAdapter>(item);
					foreach (var d in UI.WebControls.ControlPanel.GetPartDefinitions(a, item, null, html.ViewContext.HttpContext.User))
					{
						foreach (var t in a.GetTemplates(item, d))
						{
							sb.AppendFormat(
								"<div id='{0}' title='{1}' data-type='{2}' data-template='{3}' class='{4}'>{5}</div>",
								/*{0}*/ t.Definition.ToString().Replace('/', '-'),
								/*{1}*/ t.Description,
								/*{2}*/ t.Definition.Discriminator,
								/*{3}*/ t.Name,
								/*{4}*/ "definition " + t.Definition.Discriminator,
								/*{5}*/ UI.WebControls.ControlPanel.FormatImageAndText(t.Definition.IconUrl, t.Title));
						}
					}

					if (sb.Length > 0)
						return "<div class='definitions'>" + sb + "</div>";
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
		n2ctx.refresh({ navigationUrl: '{NavigationUrl}', path: '{Path}', permission: '{Permission}', force: false });";
			static string format3 = @"
		if (n2ctx.hasTop()) $('.cpAdminister').hide();
		else $('.cpView').hide();
				
		if (window.n2SlidingCurtain) {
			n2SlidingCurtain.init('#cpCurtain', false);
			n2SlidingCurtain.recalculate();
			if($.browser.webkit) setTimeout(function(){ n2SlidingCurtain.recalculate(); }, 50);
		}
	});
})(jQuery);
//]]></script>

<div id='cpCurtain' class='sc'><div class='scContent'>
	<div class='controlPanel'>
		<div class='plugins'>
			{Plugins}
		</div>
		{Definitions}
	</div>
	<a href='javascript:void(0);' class='close' title='Close'>&laquo;</a>
	<a href='javascript:void(0);' class='open' title='Open'>&raquo;</a>
</div></div>
";
			static string formatWithRefresh = format1 + format2 + format3;
			static string formatWithoutRefresh = format1 + format3;
			#endregion
		}
	}
}
