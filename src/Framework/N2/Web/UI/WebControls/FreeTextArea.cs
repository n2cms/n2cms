using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Web.Hosting;
using System.Web.UI.WebControls;

using System.Linq;

using N2.Configuration;
using N2.Resources;

namespace N2.Web.UI.WebControls
{
	/// <summary>
	/// A wrapper for the tinyMCE free text editor (http://tinymce.moxiecode.com).
	/// </summary>
	public class FreeTextArea : TextBox
	{
		static string configCssUrl;
		static string configScriptUrl;
		static NameValueCollection configSettings = new NameValueCollection();
		static bool configEnabled = true;
		static bool isInitalized = false;
		private Dictionary<string, string> customOverrides_ = new Dictionary<string, string>();

		public FreeTextArea()
		{
			TextMode = TextBoxMode.MultiLine;
			CssClass = "freeTextArea";
		}

		public virtual bool EnableFreeTextArea
		{
			get { return (bool)(ViewState["EnableFreeTextArea"] ?? configEnabled); }
			set { ViewState["EnableFreeTextArea"] = value; }
		}

		public virtual string DocumentBaseUrl
		{
			get { return (string)(ViewState["DocumentBaseUrl"]); }
			set { ViewState["DocumentBaseUrl"] = value; }
		}

		/// <summary> Custom TinyMCE config options </summary>  
		/// <remarks> Will override Web.config TinyMCE settings.
		///   See: http://tinymce.moxiecode.com/wiki.php/Configuration
		/// </remarks>
		public IDictionary<string, string> CustomOverrides { get { return customOverrides_; } }

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if (!isInitalized)
			{
				isInitalized = true;
				var config = N2.Context.Current.Resolve<EditSection>();
				if (config != null)
				{
					configCssUrl = Url.ResolveTokens(config.TinyMCE.CssUrl);
					configScriptUrl = Url.ResolveTokens(config.TinyMCE.ScriptUrl);
					configEnabled = config.TinyMCE.Enabled;
					foreach (KeyValueConfigurationElement element in config.TinyMCE.Settings)
					{
						configSettings[element.Key] = element.Value;
					}
				}
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (EnableFreeTextArea)
			{
				Register.JQuery(Page);
				Register.TinyMCE(Page);
				Register.JavaScript(Page, configScriptUrl ?? Url.ResolveTokens("{ManagementUrl}/Resources/Js/FreeTextArea.js"));

				string freeTextAreaInitScript = string.Format("freeTextArea_init('{0}', {1});",
					Url.Parse(Page.Engine().ManagementPaths.EditTreeUrl),
					GetOverridesJson());
                Page.ClientScript.RegisterStartupScript(GetType(), "FreeTextArea_" + ClientID, freeTextAreaInitScript, true);
			}
		}

        // TODO: Hämta riktiga
        private IEnumerable<string> GetTokens()
        {
            return "brantmo, libardo, magnus".Split(',');
        }

		protected virtual string GetOverridesJson()
		{
			IDictionary<string, string> overrides = new Dictionary<string, string>();
			overrides["elements"] = ClientID;
			overrides["content_css"] = configCssUrl ?? Url.ResolveTokens("{ManagementUrl}/Resources/Css/Editor.css");

            string displayableTokens = string.Format("{0}",
                String.Join(",",
                    GetTokens().Select(t => t.Trim())
                    .ToArray()
                )
            );

            overrides["autocomplete_options"] = displayableTokens;

			string language = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
			if (HostingEnvironment.VirtualPathProvider.FileExists(Url.ResolveTokens("{ManagementUrl}/Resources/tiny_mce/langs/" + language + ".js")))
				overrides["language"] = language;

			if (!string.IsNullOrEmpty(DocumentBaseUrl))
				overrides["document_base_url"] = Page.ResolveUrl(DocumentBaseUrl);

			foreach (string key in configSettings.AllKeys)
				overrides[key] = configSettings[key];

			foreach (var item in CustomOverrides)
				overrides[item.Key] = item.Value;

			return ToJsonString(overrides);
		}

		protected static string ToJsonString(IDictionary<string, string> collection)
		{
			var sb = new StringBuilder("{");

			foreach (string key in collection.Keys)
			{
				string value = collection[key];
				if (value == "true")
					value = "true";
				else if (value == "false")
					value = "false";
				else if (value == null)
					value = "null";
				else if (!value.StartsWith("[") && !value.StartsWith("{"))
					value = "'" + value + "'";
				sb.Append("'").Append(key).Append("': ").Append(value).Append(",");
			}
			if (sb.Length > 1)
				sb.Length--; // remove trailing comma
			return sb.Append("}").ToString();
		}
	}
}