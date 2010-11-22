using System;
using System.Web.Hosting;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections.Specialized;
using N2.Edit;
using N2.Resources;
using N2.Configuration;
using System.Configuration;
using System.Text;

namespace N2.Web.UI.WebControls
{
	/// <summary>
	/// A wrapper for the tinyMCE free text editor (http://tinymce.moxiecode.com).
	/// </summary>
	public class FreeTextArea : TextBox
	{
		static readonly string configCssUrl;
		static readonly string configScriptUrl;
		static readonly NameValueCollection configSettings = new NameValueCollection();
		static readonly bool configEnabled = true;

		static FreeTextArea()
		{
			var config = N2.Context.Current.Resolve<EditSection>();
			if (config != null)
			{
				configCssUrl = Url.ToAbsolute(config.TinyMCE.CssUrl);
				configScriptUrl = Url.ToAbsolute(config.TinyMCE.ScriptUrl);
				configEnabled = config.TinyMCE.Enabled;
				foreach (KeyValueConfigurationElement element in config.TinyMCE.Settings)
				{
					configSettings[element.Key] = element.Value;
				}
			}
			else
			{
				configCssUrl = EditManager.ResolveManagementInterfaceUrl("/Resources/Css/Editor.css");
				configScriptUrl = EditManager.ResolveManagementInterfaceUrl("/Resources/Js/FreeTextArea.js");
			}
		}

		private static IEditManager EditManager
		{
			get { return N2.Context.Current.Resolve<IEditManager>(); }
		}

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

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (EnableFreeTextArea)
			{
				Register.JQuery(Page);
				Register.TinyMCE(Page);
				Register.JavaScript(Page, configScriptUrl);

				string script = string.Format("freeTextArea_init('{0}', {1});",
					Url.Parse(EditManager.EditTreeUrl),
					GetOverridesJson());
				Page.ClientScript.RegisterStartupScript(GetType(), "FreeTextArea_" + ClientID, script, true);
			}
		}

		protected virtual string GetOverridesJson()
		{
			IDictionary<string, string> overrides = new Dictionary<string, string>();
			overrides["elements"] = ClientID;
			overrides["content_css"] = configCssUrl;

			string language = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
			if (HostingEnvironment.VirtualPathProvider.FileExists(EditManager.ResolveManagementInterfaceUrl("Resources/tiny_mce/langs/" + language + ".js")))
				overrides["language"] = language;

			if (!string.IsNullOrEmpty(DocumentBaseUrl))
				overrides["document_base_url"] = Page.ResolveUrl(DocumentBaseUrl);

			foreach (string key in configSettings.AllKeys)
				overrides[key] = configSettings[key];

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
				else
					value = "'" + value + "'";
				sb.Append("'").Append(key).Append("': ").Append(value).Append(",");
			}
			if (sb.Length > 1)
				sb.Length--; // remove trailing comma
			return sb.Append("}").ToString();
		}
	}
}