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

namespace N2.Web.UI.WebControls
{
	/// <summary>
	/// A wrapper for the ckeditor (http://ckeditor.com).
	/// </summary>
	public class FreeTextArea : TextBox
	{

		public enum EditorModeSetting
		{
			Basic,
			Standard,
			Full,
		}

		static string contentCssUrl;
		static bool configEnabled = true;
		static bool isInitalized = false;
		static string configJsPath = string.Empty;
		static string overwriteStylesSet = string.Empty;
		static string overwriteFormatTags = string.Empty;
		private EditorModeSetting editorMode = EditorModeSetting.Standard;
		private string additionalFormats = string.Empty;
		private string useStylesSet = string.Empty;
		private bool advancedMenues = false;

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

			if (!isInitalized)
			{
				isInitalized = true;
				var config = N2.Context.Current.Resolve<EditSection>();
				if (config != null)
				{
					configJsPath = Url.ResolveTokens(config.CkEditor.ConfigJsPath );
					overwriteStylesSet = Url.ResolveTokens(config.CkEditor.OverwriteStylesSet);
					overwriteFormatTags = config.CkEditor.OverwriteFormatTags;
					contentCssUrl = Url.ResolveTokens(config.CkEditor.ContentsCssPath);
					advancedMenues = config.CkEditor.AdvancedMenues;
				}
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (EnableFreeTextArea)
			{
				Register.JQuery(Page);
				Register.CKEditor(Page);

				string freeTextAreaInitScript = string.Format("CKEDITOR.replace('{0}', {1});",
					ClientID,
					GetOverridesJson());
				Page.ClientScript.RegisterStartupScript(GetType(), "FreeTextArea_" + ClientID, freeTextAreaInitScript, true);
			}
		}

		private IEnumerable<TokenDefinition> GetTokens()
		{
			return Context.GetEngine().Resolve<TokenDefinitionFinder>().FindTokens();
		}

		protected virtual string GetOverridesJson()
		{
			IDictionary<string, string> overrides = new Dictionary<string, string>();
			overrides["elements"] = ClientID;
			overrides["contentsCss"] = contentCssUrl ?? Register.TwitterBootstrapCssPath;

			overrides["filebrowserBrowseUrl"] = Url.Parse(Page.Engine().ManagementPaths.EditTreeUrl)
				.AppendQuery("location", "selection")
				.AppendQuery("availableModes", "All")
				.AppendQuery("selectableTypes", "");

			overrides["filebrowserImageBrowseUrl"] = Url.Parse(Page.Engine().ManagementPaths.EditTreeUrl)
				.AppendQuery("location", "filesselection")
				.AppendQuery("availableModes", "Files")
				.AppendQuery("selectableTypes", "IFileSystemFile");

			overrides["filebrowserFlashBrowseUrl"] = overrides["filebrowserImageBrowseUrl"];


			string language = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
			if (HostingEnvironment.VirtualPathProvider.FileExists(Url.ResolveTokens("{ManagementUrl}/Resources/ckeditor/lang/" + language + ".js")))
				overrides["language"] = language;

			if (!string.IsNullOrEmpty(DocumentBaseUrl))
				overrides["baseHref"] = Page.ResolveUrl(DocumentBaseUrl);

			if (advancedMenues==false)
				overrides["removeDialogTabs"] = "image:advanced;link:advanced";

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
