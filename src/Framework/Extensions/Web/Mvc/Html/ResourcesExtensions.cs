using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			return new ResourcesHelper { Html = html };
		}

		public static ResourcesHelper JQuery(this ResourcesHelper registrator)
		{
			return registrator.JavaScript(N2.Resources.Register.JQueryPath());
		}

		public static ResourcesHelper JQueryPlugins(this ResourcesHelper registrator)
		{
			return registrator.JQuery()
				.JavaScript("{ManagementUrl}/Resources/Js/plugins.ashx?v=" + typeof(Register).Assembly.GetName().Version);
		}

		public static ResourcesHelper JQueryUi(this ResourcesHelper registrator)
		{
			return registrator.JQuery()
				.JavaScript("{ManagementUrl}/Resources/Js/jquery.ui.ashx?v=" + typeof(Register).Assembly.GetName().Version);
		}

		public static ResourcesHelper TinyMCE(this ResourcesHelper registrator)
		{
			return registrator.JavaScript("{ManagementUrl}/Resources/tiny_mce/tiny_mce.js");
		}

		public class ResourcesHelper
		{
			internal HtmlHelper Html { get; set; }

			public ResourcesHelper JavaScript(string resourceUrl)
			{
				Html.ViewContext.Writer.Write(N2.Resources.Register.JavaScript(Html.ViewContext.HttpContext.Items, resourceUrl));
				return this;
			}

			public ResourcesHelper JavaScript(string script, ScriptOptions options)
			{
				Html.ViewContext.Writer.Write(N2.Resources.Register.JavaScript(Html.ViewContext.HttpContext.Items, script, options));
				return this;
			}

			public ResourcesHelper StyleSheet(string resourceUrl)
			{
				Html.ViewContext.Writer.Write(N2.Resources.Register.StyleSheet(Html.ViewContext.HttpContext.Items, resourceUrl));
				return this;
			}
		}
	}
}
