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

		static string format = @"
<script type='text/javascript'>//<![CDATA[
(function($){
	if (!window.n2ctx) return;

	n2ctx.select('preview');
	$(document).ready(function () {
		n2ctx.refresh({ navigationUrl: '{NavigationUrl}', path: '{Path}', permission: '{Permission}', force: false });
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

		/// <summary>Gets the curent state of the control panel.</summary>
		/// <param name="html"></param>
		/// <returns></returns>
		public static ControlPanelState GetControlPanelState(HtmlHelper html)
		{
			return ControlPanel.GetState(html.ViewContext.HttpContext.User, html.ViewContext.HttpContext.Request.QueryString);
		}

		/// <summary>Renders the openable control panel displayed in the upper left corner on N2 sites.</summary>
		/// <param name="html"></param>
		public static void RenderControlPanel(this HtmlHelper html)
		{
			var engine = html.ContentEngine();
			var item = html.CurrentItem() ?? html.StartPage();

			if (!engine.SecurityManager.IsEditor(html.ViewContext.HttpContext.User))
				return;
			if (RegistrationExtensions.GetRegistrationExpression(html) != null)
				return;

			var state = GetControlPanelState(html);
			var settings = new
			{
				NavigationUrl = engine.ManagementPaths.GetNavigationUrl(item),
				ManagementUrl = engine.ManagementPaths.GetManagementInterfaceUrl(),
				Path = item.Path,
				Plugins = Plugins(html, item, state),
				Definitions = Definitions(html, engine, item, state),
				Version = typeof(ContentItem).Assembly.GetName().Version.ToString(),
				Permission = engine.GetContentAdapter<NodeAdapter>(item).GetMaximumPermission(item)
			};

			html.Resources().JQuery()
				.JQueryPlugins()
				.JQueryUi()
				.JavaScript("{ManagementUrl}/Resources/Js/parts.js").StyleSheet("{ManagementUrl}/Resources/Css/parts.css");

			string controlPanelHtml = format.Replace(settings);
			html.ViewContext.Writer.Write(controlPanelHtml);

			if (state == ControlPanelState.DragDrop)
				html.Resources().JavaScript(ControlPanel.DragDropScriptInitialization(), ScriptOptions.DocumentReady);
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
				var pluginControl = plugin.AddTo(span, new PluginContext(item, null, start, root, state, html.ContentEngine(), html.ViewContext.HttpContext));

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
				foreach (var d in ControlPanel.GetPartDefinitions(a, item, null, html.ViewContext.HttpContext.User))
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
								/*{5}*/ ControlPanel.FormatImageAndText(t.Definition.IconUrl, t.Title));
					}
				}

				if (sb.Length > 0)
					return "<div class='definitions'>" + sb + "</div>";
			}
			return "";
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
	}
}
