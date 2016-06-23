using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Web.Hosting;
using System.Web.UI.WebControls;
using N2.Configuration;
using N2.Resources;
using N2.Web.Tokens;
using N2.Details;

namespace N2.Web.UI.WebControls
{
    /// <summary>
    /// A wrapper for the ckeditor (http://ckeditor.com).
    /// </summary>
    public class FreeTextArea : TextBox
    {
        static string contentCssUrl;
        static bool configEnabled = true;
        string configJsPath = string.Empty;
        string overwriteStylesSet = string.Empty;
        string overwriteFormatTags = string.Empty;
        string overwriteLanguage = string.Empty;
        private EditorModeSetting editorMode = EditorModeSetting.Standard;
        private string additionalFormats = string.Empty;
        private string useStylesSet = string.Empty;
        private bool advancedMenues = false;
        private bool? allowedContent;
        private KeyValueConfigurationCollection customConfig;

        public FreeTextArea()
        {
            TextMode = TextBoxMode.MultiLine;
            CssClass = "ckeditor";
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


        public EditorModeSetting EditorMode
        {
            get { return editorMode; }
            set { editorMode = value; }
        }

        public string AdditionalFormats
        {
            get { return additionalFormats; }
            set { additionalFormats = value; }
        }

        public string UseStylesSet
        {
            get { return useStylesSet; }
            set { useStylesSet = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var config = N2.Context.Current.Resolve<EditSection>();
            if (config != null)
            {
                configJsPath = Url.ResolveTokens(config.CkEditor.ConfigJsPath);
                overwriteStylesSet = Url.ResolveTokens(config.CkEditor.OverwriteStylesSet);
                overwriteFormatTags = config.CkEditor.OverwriteFormatTags;
                overwriteLanguage = config.CkEditor.OverwriteLanguage;
                contentCssUrl = Url.ResolveTokens(config.CkEditor.ContentsCssPath);
                advancedMenues = config.CkEditor.AdvancedMenus;
                allowedContent = config.CkEditor.AllowedContent;
                customConfig = config.CkEditor.Settings ?? new KeyValueConfigurationCollection();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (EnableFreeTextArea)
            {
                Register.JQuery(Page);
                Register.CkEditor(Page);

                string freeTextAreaInitScript = string.Format("CKEDITOR.replace('{0}', {1});",
                    ClientID,
                    GetOverridesJson());
                Page.ClientScript.RegisterStartupScript(GetType(), "FreeTextArea_" + ClientID, freeTextAreaInitScript, true);
            }
			Attributes["ng-non-bindable"] = "true";
        }

        private IEnumerable<TokenDefinition> GetTokens()
        {
            return Context.GetEngine().Resolve<TokenDefinitionFinder>().FindTokens();
        }

        protected virtual string GetOverridesJson()
        {
            IDictionary<string, string> overrides = new Dictionary<string, string>();
            overrides["elements"] = ClientID;
			if (string.IsNullOrEmpty(contentCssUrl))
				overrides["contentsCss"] = "{ManagementUrl}/Resources/Css/editor.css".ResolveUrlTokens();
            else
				overrides["contentsCss"] = contentCssUrl;

			overrides["filebrowserBrowseUrl"] = Url.Parse(Page.Engine().ManagementPaths.EditTreeUrl)
				.AppendQuery("location", "selection")
				.AppendQuery("availableModes", "All")
				.AppendQuery("selectableTypes", "");

			overrides["filebrowserImageBrowseUrl"] = Url.Parse(Page.Engine().ManagementPaths.MediaBrowserUrl);
            overrides["filebrowserFlashBrowseUrl"] = overrides["filebrowserImageBrowseUrl"];

            string language = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

            if (!string.IsNullOrEmpty(overwriteLanguage))
                language = overwriteLanguage;

            if (HostingEnvironment.VirtualPathProvider.FileExists(Url.ResolveTokens("{ManagementUrl}/Resources/ckeditor/lang/" + language + ".js")))
            {
                overrides["language"] = language;
            }
            else
            {
                overrides["language"] = "en";
            }



            if (!string.IsNullOrEmpty(DocumentBaseUrl))
                overrides["baseHref"] = Page.ResolveUrl(DocumentBaseUrl);

            if (advancedMenues == false)
                overrides["removeDialogTabs"] = "image:advanced;link:advanced";

            if (allowedContent.HasValue)
                overrides["allowedContent"] = allowedContent.Value.ToString().ToLower();

            if (!string.IsNullOrEmpty(configJsPath))
                overrides["customConfig"] = configJsPath;


            if (!string.IsNullOrEmpty(overwriteStylesSet))
                overrides["stylesSet"] = overwriteStylesSet;

            if (!string.IsNullOrEmpty(useStylesSet))
                overrides["stylesSet"] = useStylesSet;


            if (!string.IsNullOrEmpty(overwriteFormatTags))
                overrides["format_tags"] = overwriteFormatTags;
            else
                overrides["format_tags"] = "p;h1;h2;h3;pre";



            if (!string.IsNullOrEmpty(additionalFormats))
            {
                if (additionalFormats.StartsWith(";"))
                    overrides["format_tags"] = overrides["format_tags"] + additionalFormats;
                else
                    overrides["format_tags"] = overrides["format_tags"] + ";" + additionalFormats;

                if (overrides["format_tags"].EndsWith(";"))
                    overrides["format_tags"] = overrides["format_tags"].Remove(overrides["format_tags"].Length - 1, 1);
            }


            //switch toolbar
            switch (editorMode)
            {
                case EditorModeSetting.Basic:
                    overrides["toolbar"] = "[{ name: 'clipboard', items : [ 'Cut','Copy','Paste','PasteText','PasteFromWord','-','Undo','Redo' ] }, '/', { name: 'basicstyles', items : [ 'Bold','Italic','Underline' ] },{ name: 'paragraph', items : [ 'NumberedList','BulletedList' ] }]";
                    break;
                case EditorModeSetting.Standard:
                    overrides["toolbar"] = "[{ name: 'clipboard', items : [ 'Cut','Copy','Paste','PasteText','PasteFromWord','-','Undo','Redo' ] }, { name: 'links', items : [ 'Link','Unlink','Anchor' ] }, { name: 'insert', items : [ 'Image','Table','HorizontalRule','SpecialChar' ] },{ name: 'tools', items : [ 'Maximize'] }, { name: 'document', items : [ 'Source'] }, '/', { name: 'basicstyles', items : [ 'Bold','Italic','Underline','Strike','Subscript','Superscript','-','RemoveFormat' ] }, { name: 'paragraph', items : [ 'NumberedList','BulletedList','-','Outdent','Indent','-','Blockquote' ] },{ name: 'styles', items : [ 'Styles','Format' ] }]";
                    break;
            }

            if (customConfig.Count > 0)
                foreach (var key in customConfig.AllKeys)
                    overrides[key] = customConfig[key].Value;

            return ToJsonString(overrides);
        }

        protected static string ToJsonString(IDictionary<string, string> collection)
        {
            var sb = new StringBuilder("{");

            foreach (string key in collection.Keys)
            {
                string value = collection[key];
                int ignored = 0;
                if (value == "true")
                    value = "true";
                else if (value == "false")
                    value = "false";
                else if (value == null)
                    value = "null";
                else if (!int.TryParse(value, out ignored) && !value.StartsWith("[") && !value.StartsWith("{"))
                    value = "'" + value + "'";
                sb.Append("'").Append(key).Append("': ").Append(value).Append(",");
            }
            if (sb.Length > 1)
                sb.Length--; // remove trailing comma



            return sb.Append("}").ToString();
        }
    }
}
