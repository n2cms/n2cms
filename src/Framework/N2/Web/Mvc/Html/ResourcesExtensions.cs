using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using N2.Resources;
using System;

namespace N2.Web.Mvc.Html
{
	/// <summary>
	/// Renders resources on a view.
	/// </summary>
	public static class ResourcesExtensions
	{
		public static ResourcesHelper Resources(this HtmlHelper html)
		{
			return new ResourcesHelper { Writer = html.ViewContext.Writer, ViewData = html.ViewData };
		}

		public static ResourcesHelper Resources(this HtmlHelper html, TextWriter writer)
		{
			return new ResourcesHelper { Writer = writer, ViewData = html.ViewData };
		}

		/// <summary>
		/// Includes the Fancybox Jquery plugin. Requires JQuery to be included on your page already. 
		/// See https://github.com/fancyapps/fancyBox for instructions.
		/// </summary>
		/// <param name="registrator"></param>
		/// <returns></returns>
		public static ResourcesHelper Fancybox(this ResourcesHelper registrator)
		{
			return registrator
				.JavaScript(Register.FancyboxPath + "jquery.fancybox.pack.js")
				.StyleSheet("jquery.fancybox.css");
		}

		public static ResourcesHelper JQuery(this ResourcesHelper registrator)
		{
			return registrator.JavaScript(N2.Resources.Register.JQueryPath.ResolveUrlTokens());
		}

		public static ResourcesHelper JQueryPlugins(this ResourcesHelper registrator, bool includeJQuery = true)
		{
			if(includeJQuery)
				registrator = registrator.JQuery();
			return registrator.JavaScript(Register.JQueryPluginsPath.ResolveUrlTokens());
		}

		public static ResourcesHelper JQueryUi(this ResourcesHelper registrator, bool includeJQuery = true)
		{
			if (includeJQuery)
				registrator = registrator.JQuery();
			return registrator.JavaScript(Register.JQueryUiPath.ResolveUrlTokens());
		}

		public static ResourcesHelper PartsJs(this ResourcesHelper registrator)
		{
			return registrator.JavaScript(Register.PartsJsPath.ResolveUrlTokens());
		}

		public static ResourcesHelper PartsCss(this ResourcesHelper registrator)
		{
			return registrator.StyleSheet(Register.PartsCssPath.ResolveUrlTokens());
		}

		public static ResourcesHelper CKEditor(this ResourcesHelper registrator)
		{
			return registrator.JavaScript(Register.CKEditorPath.ResolveUrlTokens());
		}

		[Obsolete("Renamed to Constants")]
		public static ResourcesHelper Constnats(this ResourcesHelper registrator)
		{
			return Constants(registrator);
		}

		public static ResourcesHelper Constants(this ResourcesHelper registrator)
		{
			return registrator.JavaScript(Register.SelectedQueryKeyRegistrationScript(), ScriptOptions.ScriptTags);
		}

		public class ResourcesHelper
		{
			internal TextWriter Writer { get; set; }
			internal IDictionary<string, object> ViewData { get; set; }

			public ResourcesHelper JavaScript(string resourceUrl)
			{
				Writer.Write(N2.Resources.Register.JavaScript(ViewData, resourceUrl));
				return this;
			}

			public ResourcesHelper JavaScript(string script, ScriptOptions options)
			{
				Writer.Write(N2.Resources.Register.JavaScript(ViewData, script, options));
				return this;
			}

			public ResourcesHelper StyleSheet(string resourceUrl)
			{
				Writer.Write(N2.Resources.Register.StyleSheet(ViewData, resourceUrl));
				return this;
			}

			public override string ToString()
			{
				return "";
			}
		}
	}
}
