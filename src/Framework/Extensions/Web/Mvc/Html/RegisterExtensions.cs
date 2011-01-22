using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using N2.Resources;

namespace N2.Web.Mvc.Html
{
	public static class RegisterExtensions
	{
		public static RegisterHelper Register(this HtmlHelper html)
		{
			return new RegisterHelper { Html = html };
		}

		public static RegisterHelper JQuery(this RegisterHelper registrator)
		{
			return registrator.JavaScript(N2.Resources.Register.JQueryPath());
		}

		public static RegisterHelper JQueryPlugins(this RegisterHelper registrator)
		{
			return registrator.JQuery()
				.JavaScript("{ManagementUrl}/Resources/Js/plugins.ashx?v=" + typeof(Register).Assembly.GetName().Version);
		}

		public static RegisterHelper JQueryUi(this RegisterHelper registrator)
		{
			return registrator.JQuery()
				.JavaScript("{ManagementUrl}/Resources/Js/jquery.ui.ashx?v=" + typeof(Register).Assembly.GetName().Version);
		}

		public static RegisterHelper TinyMCE(this RegisterHelper registrator)
		{
			return registrator.JavaScript("{ManagementUrl}/Resources/tiny_mce/tiny_mce.js");
		}

		public class RegisterHelper
		{
			internal HtmlHelper Html { get; set; }

			public RegisterHelper JavaScript(string resourceUrl)
			{
				Html.ViewContext.Writer.Write(N2.Resources.Register.JavaScript(Html.ViewContext.HttpContext.Items, resourceUrl));
				return this;
			}

			public RegisterHelper JavaScript(string script, ScriptOptions options)
			{
				Html.ViewContext.Writer.Write(N2.Resources.Register.JavaScript(Html.ViewContext.HttpContext.Items, script, options));
				return this;
			}

			public RegisterHelper StyleSheet(string resourceUrl)
			{
				Html.ViewContext.Writer.Write(N2.Resources.Register.StyleSheet(Html.ViewContext.HttpContext.Items, resourceUrl));
				return this;
			}
		}
	}
}
