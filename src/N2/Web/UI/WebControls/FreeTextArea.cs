using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections.Specialized;
using N2.Resources;
using N2.Configuration;
using System.Web.Configuration;
using System.Configuration;
using System.Text;

namespace N2.Web.UI.WebControls
{
	/// <summary>
	/// A wrapper for the tinyMCE free text editor (http://tinymce.moxiecode.com).
	/// </summary>
	public class FreeTextArea : TextBox
	{
        static string configCssUrl;
        static NameValueCollection configSettings = new NameValueCollection();
        static bool configEnabled = true;
        static FreeTextArea()
        {
            EditSection config = WebConfigurationManager.GetSection("n2/edit") as EditSection;
            if (config != null)
            {
                configCssUrl = Url.ToAbsolute(config.TinyMCE.CssUrl);
                configEnabled = config.TinyMCE.Enabled;
                foreach (KeyValueConfigurationElement element in config.TinyMCE.Settings)
                {
                    configSettings[element.Key] = element.Value;
                }
            }
            else
                Url.ToAbsolute("~/Edit/Css/Editor.css");
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

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

            if (EnableFreeTextArea)
            {
                Register.JQuery(Page);
                Register.JavaScript(Page, "~/edit/js/tiny_mce/tiny_mce.js");
                Register.JavaScript(Page, "~/edit/js/plugins.ashx");

                string script = string.Format("freeTextArea_init('{0}', {1});", 
                    Url.ToAbsolute("~/Edit/FileManagement/default.aspx"), 
                    GetOverridesJson());
                Page.ClientScript.RegisterStartupScript(GetType(), "FreeTextArea_" + ClientID, script, true);
            }
		}

        protected virtual string GetOverridesJson()
        {
            IDictionary<string, object> overrides = new Dictionary<string, object>();
            overrides["elements"] = ClientID;
            overrides["content_css"] = configCssUrl;
            overrides["language"] = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

            foreach (string key in configSettings.AllKeys)
                overrides[key] = configSettings[key];

            return ToJsonString(overrides);
        } 

        protected static string ToJsonString(IDictionary<string, object> collection)
        {
            if (collection.Count == 0)
                return "{}";

            StringBuilder sb = new StringBuilder("{");

            foreach (string key in collection.Keys)
            {
                object value = collection[key];
                sb.Append("'").Append(key).Append("': ");
                if (value is string)
                    sb.Append("'").Append(value).Append("'");
                else if (value is bool)
                    sb.Append(value.ToString().ToLower());
                else
                    sb.Append(value);
                sb.Append(",");
            }
			if(sb.Length > 1)
				sb.Length--; // remove trailing comma
            return sb.Append("}").ToString();
        }

	}
}