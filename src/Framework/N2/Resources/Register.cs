using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using N2.Web;
using N2.Edit;
using System.Web;

namespace N2.Resources
{
	/// <summary>
	/// Methods to register styles and javascripts.
	/// </summary>
	public static class Register
	{
		/// <summary>
		/// Set the resource paths to the defaults. This happens BEFORE the config file is loaded, right at the app start-up. 
		/// </summary>
		static Register()
		{
			AngularJsRoot = DefaultAngularJsRoot;
			AngularStrapJsPath = DefaultAngularStrapJsRoot;
			BootstrapCssPath = DefaultBootstrapCssPath;
			BootstrapJsPath = DefaultBootstrapJsPath;
			BootstrapDatePickerJsPath = DefaultBootstrapDatePickerJsPath;
			BootstrapTimePickerJsPath = DefaultBootstrapTimePickerJsPath;
			BootstrapDatePickerCssPath = DefaultBootstrapDatePickerCssPath;
			BootstrapTimePickerCssPath = DefaultBootstrapTimePickerCssPath;
			BootstrapVersion = new Version(DefaultBootstrapVersion);
			CkEditorJsPath = DefaultCkEditorPath;
			FancyboxCssPath = DefaultFancyboxCssPath;
			FancyboxJsPath = DefaultFancyboxJsPath;
			IconsCssPath = DefaultIconsCssPath;
			JQueryJsPath = DefaultJQueryJsPath;
			JQueryUiPath = DefaultJQueryUiJsPath;
			JQueryPluginsPath = DefaultJQueryPluginsPath;
			PartsJsPath = DefaultPartsJsPath;
			PartsCssPath = DefaultPartsCssPath;
			AngularUiJsPath = DefaultAngularUiJsPath;
		}

		private static bool? _debug;
		/// <summary>Whether javascript resources should be uncompressed.</summary>
		public static bool Debug
		{
			get { return _debug ?? (HttpContext.Current != null && HttpContext.Current.IsDebuggingEnabled); }
			set { _debug = value; }
		}

		/// <summary>The jQuery version used by N2.</summary>
		public const string JQueryVersion = "1.11.3";
		public const string JQueryUiVersion = "1.11.4";
		public const string AngularJsVersion = "1.2.20";
		public const string CkEditorVersion = "4.5.4";
		public const string DefaultBootstrapVersion = "2.3.2";

		public const string DefaultFlagsCssPath = "{ManagementUrl}/Resources/icons/flags.css";
		public const string DefaultJQueryJsPath = "//code.jquery.com/jquery-" + JQueryVersion + ".min.js";
		public const string DefaultJQueryUiJsPath = "//code.jquery.com/ui/" + JQueryUiVersion + "/jquery-ui.min.js";
		public const string DefaultFancyboxJsPath = "//cdnjs.cloudflare.com/ajax/libs/fancybox/2.1.5/jquery.fancybox.min.js";
		public const string DefaultFancyboxCssPath = "//cdnjs.cloudflare.com/ajax/libs/fancybox/2.1.5/jquery.fancybox.min.css";
		public const string DefaultIconsCssPath = "//maxcdn.bootstrapcdn.com/font-awesome/4.5.0/css/font-awesome.min.css";
		public const string DefaultAngularJsRoot = "//cdnjs.cloudflare.com/ajax/libs/angular.js/" + AngularJsVersion + "/";
		public const string DefaultAngularStrapJsRoot = "//cdnjs.cloudflare.com/ajax/libs/angular-strap/0.7.4/angular-strap.min.js";
		public const string DefaultAngularUiJsPath = "//cdnjs.cloudflare.com/ajax/libs/angular-ui/0.4.0/angular-ui.min.js";
		public const string DefaultBootstrapJsPath =  "//cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/" + DefaultBootstrapVersion + "/js/bootstrap.min.js";
		public const string DefaultBootstrapCssPath = "//cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/" + DefaultBootstrapVersion + "/css/bootstrap.min.css";

		public const string DefaultBootstrapDatePickerJsPath = "{ManagementUrl}/Resources/bootstrap-components/bootstrap-datepicker.js";
		public const string DefaultBootstrapDatePickerCssPath = "{ManagementUrl}/Resources/bootstrap-components/bootstrap-datepicker.css";
		public const string DefaultBootstrapTimePickerJsPath = "{ManagementUrl}/Resources/bootstrap-components/bootstrap-timepicker.js";
		public const string DefaultBootstrapTimePickerCssPath = "{ManagementUrl}/Resources/bootstrap-components/bootstrap-timepicker.css";

		//public const string DefaultCkEditorPath = "{ManagementUrl}/Resources/ckeditor/ckeditor.js?v=" + JQueryVersion;
		public const string DefaultCkEditorPath = "//cdn.ckeditor.com/4.5.4/full/ckeditor.js";
        public const string DefaultJQueryPluginsPath = "{ManagementUrl}/Resources/Js/plugins.ashx?v=" + JQueryVersion;
		public const string DefaultPartsJsPath = "{ManagementUrl}/Resources/Js/parts.js?v=" + JQueryVersion;
		public const string DefaultPartsCssPath = "{ManagementUrl}/Resources/Css/parts.css?v=" + JQueryVersion;

		/// <summary>Path to jQuery.</summary>
		public static string JQueryJsPath { get; set; }

		/// <summary>The path to jQuery UI javascript bundle.</summary>
		public static string JQueryUiPath { get; set; }

		/// <summary>The path to the jquery plugins used by N2.</summary>
		public static string JQueryPluginsPath { get; set; }

		/// <summary> The path to angularjs folder used by N2. </summary>
		public static string AngularJsRoot { get; set; }

		/// <summary> The path to angularjs used by N2. </summary>
		public static string AngularJsPath { get { return AngularJsRoot + "angular.js"; } }

		/// <summary> The path to angular-resources used by N2. </summary>
		public static string AngularJsResourcePath { get { return AngularJsRoot + "angular-resource.js"; } }

		/// <summary> The path to angular-resources used by N2. </summary>
		public static string AngularJsSanitizePath { get { return AngularJsRoot + "angular-sanitize.js"; } }

		/// <summary>The path to the CKeditor script</summary>
		public static string CkEditorJsPath { get; set; }

		/// <summary>The path to the parts script.</summary>
		public static string PartsJsPath { get; set; }

		/// <summary>The path to the parts css.</summary>
		public static string PartsCssPath { get; set; }

		/// <summary>The path to Twitter Bootstrap CSS library.</summary>
		public static string BootstrapCssPath { get; set; }

		/// <summary>The path to Twitter Bootstrap JS library.</summary>
		public static string BootstrapJsPath { get; set; }
		public static Version BootstrapVersion { get; set; }

		/// <summary>The path to the icon css classes.</summary>
		public static string IconsCssPath { get; set; }

		/// <summary>The path to Fancybox JS library.</summary>
		public static string FancyboxJsPath { get; set; }

		/// <summary>The path to Fancybox CSS resources.</summary>
		public static string FancyboxCssPath { get; set; }

		public static string AngularStrapJsPath { get; set; }
		public static string AngularUiJsPath { get; set; }
		public static string BootstrapDatePickerCssPath { get; set; }
		public static string BootstrapDatePickerJsPath { get; set; }
		public static string BootstrapTimePickerCssPath { get; set; }
		public static string BootstrapTimePickerJsPath { get; set; }

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

			resourceUrl = Url.ToAbsolute(resourceUrl);

			if (page.Items[resourceUrl] == null)
			{
				var holder = GetPlaceHolder(page);
				if (holder == null)
					return;

				var link = new HtmlLink {Href = Url.ResolveTokens(resourceUrl)};
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
				string key = script.GetHashCode().ToString(CultureInfo.InvariantCulture);
				if (options.Is(ScriptOptions.None))
					page.ClientScript.RegisterStartupScript(typeof(Register), key, script);
				else if (options.Is(ScriptOptions.ScriptTags))
					page.ClientScript.RegisterStartupScript(typeof(Register), key, script, true);
				else if (options.Is(ScriptOptions.DocumentReady))
				{
					page.JQuery();
					page.ClientScript.RegisterStartupScript(typeof(Register), key, EmbedDocumentReady(script), true);
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
			if (page == null) return;

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
				writer.Write("<script type=\"text/javascript\">//<![CDATA[\n");
				base.Render(writer);
				writer.Write("\n//]]>\n</script>");
			}
		}

		private static Literal AddString(Control holder, string script, bool priority)
		{
			var l = new Literal { Text = script };
			if (priority)
				holder.Controls.AddAt(0, l);
			else
				holder.Controls.Add(l);
			return l;
		}

		private static Control AddScriptInclude(Page page, string resourceUrl, Control holder, bool priority)
		{
			if (page == null) throw new ArgumentNullException("page");

			var script = new HtmlGenericControl("script");
			page.Items[resourceUrl] = script;

			resourceUrl = Url.ResolveTokens(resourceUrl);

			script.Attributes["src"] = resourceUrl;
			script.Attributes["type"] = "text/javascript";
			if (priority)
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
			JavaScript(page, Url.ResolveTokens(JQueryJsPath), ScriptPosition.Header, ScriptOptions.Prioritize | ScriptOptions.Include);
		}

		private static Script GetScriptHolder(Page page)
		{
			var holder = GetPlaceHolder(page);
			var scripts = page.Items["N2.Resources.scripts"] as Script;
			if (scripts == null)
			{
				page.Items["N2.Resources.scripts"] = scripts = new Script();
				holder.Controls.Add(scripts);
			}
			return scripts;
		}

		private static PlaceHolder GetPlaceHolder(Page page)
		{
			if (page == null) return null;

			var holder = page.Items["N2.Resources.holder"] as PlaceHolder;
			if (holder != null)
				return holder;

			page.Items["N2.Resources.holder"] = holder = new PlaceHolder();

			if (page.Header == null)
				page.Controls.Add(holder);
			else if (page.Header.Controls.Count > 0)
				page.Header.Controls.AddAt(1, holder);
			else
				page.Header.Controls.Add(holder);

			return holder;
		}

		#region TabPanel

		public static void TabPanel(Page page, string selector, bool registerTabCss)
		{
			var key = "N2.Resources.TabPanel" + selector;
			if (page.Items[key] != null)
				return;

			JQuery(page);
			JQueryPlugins(page);

			var script = String.Format("jQuery(\"{0}\").n2tabs(\"{1}\",location.hash);", selector, selector.Replace('.', '_'));
			JavaScript(page, script, ScriptOptions.DocumentReady);
			page.Items[key] = new object();
			if (registerTabCss)
			{
				StyleSheet(page, Url.ResolveTokens("{ManagementUrl}/Resources/Css/TabPanel.css"));
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

		public static void CkEditor(this Page page)
		{
			JavaScript(page, CkEditorJsPath.ResolveUrlTokens());
		}

		#endregion

		#region MVC
		public static bool RegisterResource(ICollection<string> stateCollection, string resourceUrl)
		{
			if (IsRegistered(stateCollection, resourceUrl))
				return true;

			stateCollection.Add(resourceUrl);
			return false;
		}

		public static bool IsRegistered(ICollection<string> stateCollection, string resourceUrl)
		{
			return stateCollection.Contains(resourceUrl);
		}

		public static string JavaScript(ICollection<string> stateCollection, string resourceUrl)
		{
			if (IsRegistered(stateCollection, resourceUrl))
				return null;

			RegisterResource(stateCollection, resourceUrl);

			return String.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", Url.ResolveTokens(resourceUrl));
		}

		public static string JavaScript(ICollection<string> stateCollection, string script, ScriptOptions options)
		{
			const string scriptFormat = "<script type=\"text/javascript\">//<![CDATA[\n{0}//]]></script>";

			if (IsRegistered(stateCollection, script))
				return null;

			RegisterResource(stateCollection, script);

			if (options == ScriptOptions.Include)
				return JavaScript(stateCollection, script);
			if (options == ScriptOptions.None)
				return script;
			if (options == ScriptOptions.ScriptTags)
				return String.Format(scriptFormat, script);
			if (options == ScriptOptions.DocumentReady)
				return String.Format(scriptFormat, EmbedDocumentReady(script));

			throw new NotSupportedException(options + " not supported");
		}

		public static string JQuery(ICollection<string> stateCollection)
		{
			return JavaScript(stateCollection, JQueryJsPath.ResolveUrlTokens());
		}

		public static string JQueryPlugins(ICollection<string> stateCollection)
		{
			return JQuery(stateCollection) + JavaScript(stateCollection, JQueryPluginsPath.ResolveUrlTokens());
		}

		public static string JQueryUi(ICollection<string> stateCollection)
		{
			return JQuery(stateCollection) + JavaScript(stateCollection, JQueryUiPath.ResolveUrlTokens());
		}

		[Obsolete("TinyMCE no longer supported; use CkEditor() instead.")]
		public static string TinyMCE(ICollection<string> stateCollection)
		{
			return JavaScript(stateCollection, Url.ResolveTokens(CkEditorJsPath));
		}

		public static string StyleSheet(ICollection<string> stateCollection, string resourceUrl)
		{
			if (IsRegistered(stateCollection, resourceUrl))
				return null;

			RegisterResource(stateCollection, resourceUrl);

			return String.Format("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />", Url.ResolveTokens(resourceUrl));
		}
		#endregion

		internal static string SelectedQueryKeyRegistrationScript()
		{
			return "n2SelectedQueryKey = '" + SelectionUtility.SelectedQueryKey + "';";
		}

		internal static void FrameInteraction(this Page page)
		{
			JavaScript(page, "{ManagementUrl}/Resources/Js/frameInteraction.js");
		}
	}
}
