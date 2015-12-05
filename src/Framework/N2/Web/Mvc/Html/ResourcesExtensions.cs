using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using N2.Resources;
using System;
using System.Collections;
using System.Web;
using System.Text;

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

		public static ICollection<string> GetResourceStateCollection(this ViewContext context)
		{
			var collection = context.RouteData.DataTokens["ResourceStateCollection"] as ICollection<string>;
			if (collection == null)
				context.RouteData.DataTokens["ResourceStateCollection"] = collection = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			return collection;
		}

		public static ResourcesHelper Resources(this HtmlHelper html)
		{
			return new ResourcesHelper { Writer = html.ViewContext.Writer, StateCollection = html.ViewContext.GetResourceStateCollection() };
		}

		public static ResourcesHelper Resources(this HtmlHelper html, TextWriter writer)
		{
			return new ResourcesHelper { Writer = writer, StateCollection = html.ViewContext.GetResourceStateCollection() };
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

		public static ResourcesHelper IconsCss(this ResourcesHelper registrator)
		{
			var p = Register.IconsCssPath.Split(';');
			for (int i = 0; i < p.Length; ++i)
				registrator = registrator.StyleSheet(p[i].ResolveUrlTokens());
			return registrator;
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
			return registrator.HtmlLiteral(BootstrapRowClass(fluid));
		}

		public static string BootstrapRowClass( bool fluid = false)
		{
			if (N2.Resources.Register.BootstrapVersion.Major > 2 || !fluid)
				return ("row");
			else
				return ("row-fluid");
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
			return registrator.HtmlLiteral(BootstrapColumnClass(colSpan, size));
		}

		public static string BootstrapColumnClass(int colSpan, BootstrapScreenSize size)
		{
			if (colSpan < 0 || colSpan > 12)
				throw new ArgumentException("colSpan should be between 1 and 12, inclusive");

			if (N2.Resources.Register.BootstrapVersion.Major < 3)
			{
				return ("span" + colSpan);
			}
			else
			{
				switch (size)
				{
					case BootstrapScreenSize.lg:
						return ("col-lg-" + colSpan);
					case BootstrapScreenSize.md:
						return ("col-md-" + colSpan);
					case BootstrapScreenSize.sm:
						return ("col-sm-" + colSpan);
					case BootstrapScreenSize.xs:
						return ("col-xs-" + colSpan);
					default:
						throw new ArgumentException("size");
				}
			}
		}

		public static ResourcesHelper PartsJs(this ResourcesHelper registrator)
		{
			registrator = registrator.IconsCss();
			registrator = registrator.JavaScript(Register.PartsJsPath.ResolveUrlTokens());
			return registrator;
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

		public class ResourcesHelper : IHtmlString
		{
			private StringBuilder content = new StringBuilder();
			internal TextWriter Writer { get; set; }
			internal ICollection<string> StateCollection { get; set; }

			public ResourcesHelper JavaScript(string resourceUrl)
			{
				content.Append(N2.Resources.Register.JavaScript(StateCollection, resourceUrl));
				return this;
			}

			public ResourcesHelper JavaScript(string script, ScriptOptions options)
			{
				content.Append(N2.Resources.Register.JavaScript(StateCollection, script, options));
				return this;
			}

			public ResourcesHelper StyleSheet(string resourceUrl)
			{
				content.Append(N2.Resources.Register.StyleSheet(StateCollection, resourceUrl));
				return this;
			}

			public ResourcesHelper HtmlLiteral(string html)
			{
				content.Append(html);
				return this;
			}

			public void Render(TextWriter writer = null)
			{
				(writer ?? Writer).Write(content.ToString());
			}

			public override string ToString()
			{
				var output = content.ToString();
				return output;
			}

			string IHtmlString.ToHtmlString()
			{
				return ToString();
			}

		}
	}
}
