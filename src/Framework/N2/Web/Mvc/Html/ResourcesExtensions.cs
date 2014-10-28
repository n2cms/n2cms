using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using N2.Resources;
using System;
using System.Collections;
using System.Web;

namespace N2.Web.Mvc.Html
{
	/// <summary>
	/// Renders resources on a view.
	/// </summary>
	public static class ResourcesExtensions
	{
		public static ICollection<string> GetResourceStateCollection(this HttpContextBase context)
		{
			var collection = context.Items["ResourceStateCollection"] as ICollection<string>;
			if (collection == null)
				context.Items["ResourceStateCollection"] = collection = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			return collection;
		}

		public static ResourcesHelper Resources(this HtmlHelper html)
		{
			return new ResourcesHelper { Writer = html.ViewContext.Writer, StateCollection = html.ViewContext.HttpContext.GetResourceStateCollection() };
		}

		public static ResourcesHelper Resources(this HtmlHelper html, TextWriter writer)
		{
			return new ResourcesHelper { Writer = writer, StateCollection = html.ViewContext.HttpContext.GetResourceStateCollection() };
		}

		/// <summary>
		/// Includes the Fancybox Jquery plugin. Requires JQuery to be included on your page already. 
		/// See https://github.com/fancyapps/fancyBox for instructions. You need to add a script like
		/// this somewhere in your page: $(document).ready(function() { $(".fancybox").fancybox(); });
		/// </summary>
		/// <param name="registrator"></param>
		/// <returns></returns>
		public static ResourcesHelper Fancybox(this ResourcesHelper registrator)
		{
			return registrator
				.JavaScript(Register.FancyboxJsPath.ResolveUrlTokens())
				.StyleSheet(Register.FancyboxCssPath.ResolveUrlTokens());
		}

		public static IEnumerable<ResourcesHelper> IconsCss(this ResourcesHelper registrator)
		{
			var p = Register.IconsCssPath.Split(';');
			var r = new ResourcesHelper[p.Length];
			for (int i = 0; i < p.Length; ++i)
				r[i] = registrator.StyleSheet(p[i].ResolveUrlTokens());
			return r;
		}

		public static ResourcesHelper JQuery(this ResourcesHelper registrator)
		{
			return registrator.JavaScript(N2.Resources.Register.JQueryJsPath.ResolveUrlTokens());
		}

		public static ResourcesHelper JQueryPlugins(this ResourcesHelper registrator, bool includeJQuery = true)
		{
			if (includeJQuery)
				registrator = registrator.JQuery();
			return registrator.JavaScript(Register.JQueryPluginsPath.ResolveUrlTokens());
		}

		public static ResourcesHelper JQueryUi(this ResourcesHelper registrator, bool includeJQuery = true)
		{
			if (includeJQuery)
				registrator = registrator.JQuery();
			return registrator.JavaScript(Register.JQueryUiPath.ResolveUrlTokens());
		}

		public static ResourcesHelper BootstrapJs(this ResourcesHelper registrator)
		{
			return registrator.JavaScript(N2.Resources.Register.BootstrapJsPath.ResolveUrlTokens());
		}

		public static ResourcesHelper BootstrapCss(this ResourcesHelper registrator)
		{
			return registrator.StyleSheet(N2.Resources.Register.BootstrapCssPath.ResolveUrlTokens());
		}

		public static ResourcesHelper Bootstrap(this ResourcesHelper registrator)
		{
			return registrator.BootstrapCss().BootstrapJs();
		}

		public static ResourcesHelper BootstrapRowClass(this ResourcesHelper registrator, bool fluid = false)
		{
			if (N2.Resources.Register.BootstrapVersion.Major > 2 || !fluid)
				return registrator.HtmlLiteral("row");
			else
				return registrator.HtmlLiteral("row-fluid");
		}

		public enum BootstrapScreenSize
		{
			xs,
			sm,
			md,
			lg
		}

		public static ResourcesHelper BootstrapColumnClass(this ResourcesHelper registrator, int colSpan, BootstrapScreenSize size)
		{
			if (colSpan < 0 || colSpan > 12)
				throw new ArgumentException("colSpan should be between 1 and 12, inclusive");

			if (N2.Resources.Register.BootstrapVersion.Major < 3)
			{
				return registrator.HtmlLiteral("span" + colSpan);
			}
			else
			{
				switch (size)
				{
					case BootstrapScreenSize.lg:
						return registrator.HtmlLiteral("col-lg-" + colSpan);
					case BootstrapScreenSize.md:
						return registrator.HtmlLiteral("col-md-" + colSpan);
					case BootstrapScreenSize.sm:	
						return registrator.HtmlLiteral("col-sm-" + colSpan);
					case BootstrapScreenSize.xs:
						return registrator.HtmlLiteral("col-xs-" + colSpan);
					default:
						throw new ArgumentException("size");
				}
			}
		}

		public static IEnumerable<ResourcesHelper> PartsJs(this ResourcesHelper registrator)
		{
			List<ResourcesHelper> result = new List<ResourcesHelper>();
			result.AddRange(registrator.IconsCss());
			result.Add(registrator.JavaScript(Register.PartsJsPath.ResolveUrlTokens()));
			return result;
		}

		public static ResourcesHelper PartsCss(this ResourcesHelper registrator)
		{
			return registrator.StyleSheet(Register.PartsCssPath.ResolveUrlTokens());
		}

		public static ResourcesHelper CkEditor(this ResourcesHelper registrator)
		{
			return registrator.JavaScript(Register.CkEditorJsPath.ResolveUrlTokens());
		}

		public static ResourcesHelper Constants(this ResourcesHelper registrator)
		{
			return registrator.JavaScript(Register.SelectedQueryKeyRegistrationScript(), ScriptOptions.ScriptTags);
		}

		public class ResourcesHelper
		{
			internal TextWriter Writer { get; set; }
			internal ICollection<string> StateCollection { get; set; }

			public ResourcesHelper JavaScript(string resourceUrl)
			{
				Writer.Write(N2.Resources.Register.JavaScript(StateCollection, resourceUrl));
				return this;
			}

			public ResourcesHelper JavaScript(string script, ScriptOptions options)
			{
				Writer.Write(N2.Resources.Register.JavaScript(StateCollection, script, options));
				return this;
			}

			public ResourcesHelper StyleSheet(string resourceUrl)
			{
				Writer.Write(N2.Resources.Register.StyleSheet(StateCollection, resourceUrl));
				return this;
			}

			public ResourcesHelper HtmlLiteral(string html)
			{
				Writer.Write(html);
				return this;
			}

			public override string ToString()
			{
				return "";
			}
		}
	}
}
