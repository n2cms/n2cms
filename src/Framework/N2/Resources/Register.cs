using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using N2.Web;
using N2.Edit;

namespace N2.Resources
{
	/// <summary>
	/// Methods to register styles and javascripts.
	/// </summary>
	public static class Register
	{
		static Register()
		{
			JQueryPath = "{ManagementUrl}/Resources/Js/jquery-" + JQueryVersion + ".min.js";
			JQueryUiPath = "{ManagementUrl}/Resources/Js/jquery.ui.ashx?v=" + JQueryVersion;
			JQueryPluginsPath = "{ManagementUrl}/Resources/Js/plugins.ashx?v=" + JQueryVersion;
			TinyMCEPath = "{ManagementUrl}/Resources/tiny_mce/tiny_mce.js?v=" + JQueryVersion;
			PartsJsPath = "{ManagementUrl}/Resources/Js/parts.js?v=" + JQueryVersion;
			PartsCssPath = "{ManagementUrl}/Resources/Css/parts.css?v=" + JQueryVersion;
		}

		/// <summary>Whether javascript resources should be uncompressed.</summary>
		public static bool Debug { get; set; }
		
		/// <summary>The jQuery version used by N2.</summary>
		public const string JQueryVersion = N2.Configuration.ResourcesElement.JQueryVersion;
		
		/// <summary>Path to jQuery.</summary>
		public static string JQueryPath { get; set; }
		
		/// <summary>The path to jQuery UI javascript bundle.</summary>
		public static string JQueryUiPath { get; set; }
		
		/// <summary>The path to the jquery plugins used by N2.</summary>
		public static string JQueryPluginsPath { get; set; }
		
		/// <summary>The path to the tiny MCE editor script</summary>
		public static string TinyMCEPath { get; set; }

		/// <summary>The path to the parts script.</summary>
		public static string PartsJsPath { get; set; }

		/// <summary>The path to the parts css.</summary>
		public static string PartsCssPath { get; set; }

		#region page StyleSheet

		/// <summary>Register an embedded style sheet reference in the page's header.</summary>
		/// <param name="page">The page onto which to register the style sheet.</param>
		/// <param name="type">The type whose assembly contains the embedded style sheet.</param>
		/// <param name="resourceName">The name of the embedded resource.</param>
		public static void StyleSheet(this Page page, Type type, string resourceName)
		{
			StyleSheet(page, page.ClientScript.GetWebResourceUrl(type, resourceName), Media.All);
		}

		/// <summary>Register a style sheet reference in the page's header.</summary>
		/// <param name="page">The page onto which to register the style sheet.</param>
		/// <param name="resourceUrl">The url to the style sheet to register.</param>
		public static void StyleSheet(this Page page, string resourceUrl)
		{
			StyleSheet(page, resourceUrl, Media.All);
		}

		/// <summary>Register a style sheet reference in the page's header with media type.</summary>
		/// <param name="page">The page onto which to register the style sheet.</param>
		/// <param name="resourceUrl">The url to the style sheet to register.</param>
		/// <param name="media">The media type to assign, e.g. print.</param>
		public static void StyleSheet(this Page page, string resourceUrl, Media media)
		{
			if (page == null) throw new ArgumentNullException("page");
			if (resourceUrl == null) throw new ArgumentNullException("resourceUrl");
			
			resourceUrl = N2.Web.Url.ToAbsolute(resourceUrl);

			if (page.Items[resourceUrl] == null)
			{
				PlaceHolder holder = GetPlaceHolder(page);

				HtmlLink link = new HtmlLink();
				link.Href = Url.ResolveTokens(resourceUrl);
				link.Attributes["type"] = "text/css";
				link.Attributes["media"] = media.ToString().ToLower();
				link.Attributes["rel"] = "stylesheet";
				holder.Controls.Add(link);

				page.Items[resourceUrl] = true;
			}
		}

		#endregion

		#region page JavaScript

		/// <summary>Register an embedded javascript resource reference in the page header.</summary>
		/// <param name="page">The page in whose header to register the javascript.</param>
		/// <param name="type">The type in whose assembly the javascript is embedded.</param>
		/// <param name="resourceName">The name of the embedded resource.</param>
		public static void JavaScript(this Page page, Type type, string resourceName)
		{
			JavaScript(page, page.ClientScript.GetWebResourceUrl(type, resourceName));
		}

		/// <summary>Register an embedded javascript resource reference in the page header with options.</summary>
		/// <param name="page">The page in whose header to register the javascript.</param>
		/// <param name="type">The type in whose assembly the javascript is embedded.</param>
		/// <param name="resourceName">The name of the embedded resource.</param>
		/// <param name="options">Options flag.</param>
		public static void JavaScript(this Page page, Type type, string resourceName, ScriptOptions options)
		{
			JavaScript(page, page.ClientScript.GetWebResourceUrl(type, resourceName), options);
		}

		/// <summary>Registers a script block on a page.</summary>
		/// <param name="page">The page onto which to added the script.</param>
		/// <param name="script">The script to add.</param>
		/// <param name="position">Where to add the script.</param>
		/// <param name="options">Script registration options.</param>
		public static void JavaScript(this Page page, string script, ScriptPosition position, ScriptOptions options)
		{
			if (page == null) throw new ArgumentNullException("page");
			
			if (position == ScriptPosition.Header)
			{
				JavaScript(page, script, options);
			}
			else if (position == ScriptPosition.Bottom)
			{
				string key = script.GetHashCode().ToString();
				if (options.Is(ScriptOptions.None))
					page.ClientScript.RegisterStartupScript(typeof(Register), key, script);
				else if (options.Is(ScriptOptions.ScriptTags))
					page.ClientScript.RegisterStartupScript(typeof(Register), key, script, true);
				else if (options.Is(ScriptOptions.DocumentReady))
				{
					page.JQuery();
					page.ClientScript.RegisterStartupScript(typeof (Register), key, EmbedDocumentReady(script), true);
				}
				else if (options.Is(ScriptOptions.Include))
					page.ClientScript.RegisterClientScriptInclude(key, Url.ResolveTokens(script));
				else
					throw new ArgumentException("options");
			}
			else
				throw new ArgumentException("position");
		}

		private static string EmbedDocumentReady(string script)
		{
			return "jQuery(document).ready(function(){" + script + "});";
		}

		public static void JavaScript(this Page page, string script, ScriptOptions options)
		{
			if (page == null) throw new ArgumentNullException("page");
			
			if (page.Items[script] == null)
			{
				PlaceHolder holder = GetPlaceHolder(page);

				if (options.Is(ScriptOptions.Include))
				{
					AddScriptInclude(page, script, holder, options.Is(ScriptOptions.Prioritize));
				}
				else if (options.Is(ScriptOptions.None))
				{
					holder.Page.Items[script] = AddString(holder, script, options.Is(ScriptOptions.Prioritize));
				}
				else
				{
					Script scriptHolder = GetScriptHolder(page);
					if (options.Is(ScriptOptions.ScriptTags))
					{
						holder.Page.Items[script] = AddString(scriptHolder, script + Environment.NewLine, Is(options, ScriptOptions.Prioritize));
					}
					else if (options.Is(ScriptOptions.DocumentReady))
					{
						JQuery(page);
						holder.Page.Items[script] = AddString(scriptHolder, EmbedDocumentReady(script) + Environment.NewLine, options.Is(ScriptOptions.Prioritize));
					}
				}
			}
		}

		private class Script : Control
		{
			protected override void Render(HtmlTextWriter writer)
			{
				writer.Write("<script type='text/javascript'>");
				writer.Write("//<![CDATA[");
				writer.Write(Environment.NewLine);
				base.Render(writer);
				writer.Write(Environment.NewLine);
				writer.Write("//]]>");
				writer.Write("</script>");
			}
		}

		private static Literal AddString(Control holder, string script, bool priority)
		{
			Literal l = new Literal();
			l.Text = script;
			if(priority)
				holder.Controls.AddAt(0, l);
			else 
				holder.Controls.Add(l);
			return l;
		}

		private static Control AddScriptInclude(Page page, string resourceUrl, Control holder, bool priority)
		{
			if (page == null) throw new ArgumentNullException("page");
			
			HtmlGenericControl script = new HtmlGenericControl("script");
			page.Items[resourceUrl] = script;

			resourceUrl = Url.ResolveTokens(resourceUrl);

			script.Attributes["src"] = resourceUrl;
			script.Attributes["type"] = "text/javascript";
			if(priority)
				holder.Controls.AddAt(0, script);
			else
				holder.Controls.Add(script);

			return script;
		}

		/// <summary>Registers a script reference in the page's header.</summary>
		/// <param name="page">The page onto which to register the javascript.</param>
		/// <param name="resourceUrl">The url to the javascript to register.</param>
		public static void JavaScript(this Page page, string resourceUrl)
		{
			if (page == null) throw new ArgumentNullException("page");
			if (resourceUrl == null) throw new ArgumentNullException("resourceUrl");

			JavaScript(page, resourceUrl, ScriptOptions.Include);
		}

		public static void JQuery(this Page page)
		{
			JavaScript(page, Url.ResolveTokens(JQueryPath), ScriptPosition.Header, ScriptOptions.Prioritize | ScriptOptions.Include);
		}

		private static Script GetScriptHolder(Page page)
		{
			PlaceHolder holder = GetPlaceHolder(page);
			Script scripts = page.Items["N2.Resources.scripts"] as Script;
			if (scripts == null)
			{
				page.Items["N2.Resources.scripts"] = scripts = new Script();
				holder.Controls.Add(scripts);
			}
			return scripts;
		}

		private static PlaceHolder GetPlaceHolder(Page page)
		{
			PlaceHolder holder = page.Items["N2.Resources.holder"] as PlaceHolder;
			if (holder == null)
			{
				if (page.Header == null)
					throw new N2Exception("Couldn't find the page header. The register command needs the tag <header runat='server'> somewhere in the page template, master page or a user control.");

				page.Items["N2.Resources.holder"] = holder = new PlaceHolder();
				if (page.Header.Controls.Count > 0)
					page.Header.Controls.AddAt(1, holder);
				else
					page.Header.Controls.Add(holder);
			}
			return holder;
		}

		#region TabPanel
		private const string tabPanelFormat = @"jQuery('{0}').n2tabs('{1}',location.hash);";

		public static void TabPanel(Page page, string selector, bool registerTabCss)
		{
			var key = "N2.Resources.TabPanel" + selector;
			if (page.Items[key] == null)
			{
				JQuery(page);
				JQueryPlugins(page);

				string script = string.Format(tabPanelFormat, selector, selector.Replace('.', '_'));
				JavaScript(page, script, ScriptOptions.DocumentReady);
				page.Items[key] = new object();
				if (registerTabCss)
				{
					StyleSheet(page, Url.ResolveTokens("{ManagementUrl}/Resources/Css/TabPanel.css"));
				}
			}
		}

		public static void TabPanel(Page page, string selector)
		{
			TabPanel(page, selector, true);
		}

		private static bool Is(this ScriptOptions options, ScriptOptions expectedOption)
		{
			return (options & expectedOption) == expectedOption;
		}
		#endregion

		public static void JQueryPlugins(this Page page)
		{
			page.JQuery();
			page.JavaScript(JQueryPluginsPath.ResolveUrlTokens(), ScriptPosition.Header, ScriptOptions.Include);
		}

		public static void JQueryUi(this Page page)
		{
			page.JQuery();
			page.JavaScript(JQueryUiPath.ResolveUrlTokens(), ScriptPosition.Header, ScriptOptions.Include);
		}

		public static void TinyMCE(this Page page)
		{
			JavaScript(page, TinyMCEPath.ResolveUrlTokens());
		}

		#endregion

		#region MVC
		public static bool RegisterResource(IDictionary<string, object> stateCollection, string resourceUrl)
		{
			if (IsRegistered(stateCollection, resourceUrl))
				return true;
			
			stateCollection[resourceUrl] = "";
			return false;
		}

		public static bool IsRegistered(IDictionary<string, object> stateCollection, string resourceUrl)
		{
			return stateCollection.ContainsKey(resourceUrl);
		}

		public static string JavaScript(IDictionary<string, object> stateCollection, string resourceUrl)
		{
			if (IsRegistered(stateCollection, resourceUrl))
				return null;

			RegisterResource(stateCollection, resourceUrl);

			return string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", Url.ResolveTokens(resourceUrl));
		}

		const string scriptFormat = @"<script type=""text/javascript"">//<![CDATA[
{0}//]]></script>";
		public static string JavaScript(IDictionary<string, object> stateCollection, string script, ScriptOptions options)
		{
			if (IsRegistered(stateCollection, script))
				return null;

			RegisterResource(stateCollection, script);

			if (options == ScriptOptions.Include)
				return JavaScript(stateCollection, script);
			if (options == ScriptOptions.None)
				return script;
			if (options == ScriptOptions.ScriptTags)
				return string.Format(scriptFormat, script);
			if (options == ScriptOptions.DocumentReady)
				return string.Format(scriptFormat, EmbedDocumentReady(script));

			throw new NotSupportedException(options + " not supported");
		}

		public static string JQuery(IDictionary<string, object> stateCollection)
		{
			return JavaScript(stateCollection, JQueryPath.ResolveUrlTokens());
		}

		public static string JQueryPlugins(IDictionary<string, object> stateCollection)
		{
			return JQuery(stateCollection) + JavaScript(stateCollection, JQueryPluginsPath.ResolveUrlTokens());
		}

		public static string JQueryUi(IDictionary<string, object> stateCollection)
		{
			return JQuery(stateCollection) + JavaScript(stateCollection, JQueryUiPath.ResolveUrlTokens());
		}

		public static string TinyMCE(IDictionary<string, object> stateCollection)
		{
			return JavaScript(stateCollection, Url.ResolveTokens(TinyMCEPath));
		}

		public static string StyleSheet(IDictionary<string, object> stateCollection, string resourceUrl)
		{
			if (IsRegistered(stateCollection, resourceUrl))
				return null;

			RegisterResource(stateCollection, resourceUrl);

			return string.Format("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />", Url.ResolveTokens(resourceUrl));
		}
		#endregion

		internal static string SelectedQueryKeyRegistrationScript()
		{
			return "n2SelectedQueryKey = '" + SelectionUtility.SelectedQueryKey + "';";
		}
	}
}