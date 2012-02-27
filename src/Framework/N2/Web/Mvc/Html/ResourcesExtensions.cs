using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using N2.Resources;

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

		public static ResourcesHelper JQuery(this ResourcesHelper registrator)
		{
			return registrator.JavaScript(N2.Resources.Register.JQueryPath());
		}

		public static ResourcesHelper JQueryPlugins(this ResourcesHelper registrator)
		{
			return registrator.JQuery()
				.JavaScript("{ManagementUrl}/Resources/Js/plugins.ashx?v=" + Register.JQueryVersion);
		}

		public static ResourcesHelper JQueryUi(this ResourcesHelper registrator)
		{
			return registrator.JQuery()
				.JavaScript("{ManagementUrl}/Resources/Js/jquery.ui.ashx?v=" + Register.JQueryVersion);
		}

		public static ResourcesHelper TinyMCE(this ResourcesHelper registrator)
		{
			return registrator.JavaScript("{ManagementUrl}/Resources/tiny_mce/tiny_mce.js");
		}

		public static ResourcesHelper Constnats(this ResourcesHelper registrator)
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
