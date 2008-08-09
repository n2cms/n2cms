using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections.Specialized;
using N2.Resources;
using N2.Configuration;
using System.Web.Configuration;
using System.Configuration;

namespace N2.Web.UI.WebControls
{
	/// <summary>
	/// A wrapper for the tinyMCE free text editor. <see cref="http://tinymce.moxiecode.com"/>
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

        private static string ElementsKey = "TinyMCE.Elements";
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.Items[ElementsKey] += "," + ClientID;
        }

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

            if (EnableFreeTextArea)
            {
                Register.JQuery(Page);
                Register.JavaScript(Page, "~/edit/js/tiny_mce/tiny_mce.js");
                Register.JavaScript(Page, typeof(FreeTextArea), "N2.Resources.FreeTextArea.js");

                string script = string.Format("freeTextArea_init('{0}', {1});", 
                    N2.Web.Url.ToAbsolute("~/Edit/FileManagement/default.aspx"), 
                    GetOverridesJson());
                Page.ClientScript.RegisterStartupScript(GetType(), "startup", script, true);
            }
		}

        protected virtual string GetOverridesJson()
        {
            IDictionary<string, object> overrides = new Dictionary<string, object>();
            overrides["elements"] = ((string)Page.Items[ElementsKey]).TrimStart(',');
            overrides["content_css"] = configCssUrl;
            foreach (string key in configSettings.AllKeys)
                overrides[key] = configSettings[key];
            return Json.ToObject(overrides);
        }
	}
}