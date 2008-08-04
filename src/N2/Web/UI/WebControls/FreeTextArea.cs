using System;
using System.Web;
using System.Web.UI.WebControls;

namespace N2.Web.UI.WebControls
{
	/// <summary>
	/// A wrapper for the tinyMCE free text editor. <see cref="http://tinymce.moxiecode.com"/>
	/// </summary>
	public class FreeTextArea : TextBox
	{
		#region Constructors

		public FreeTextArea()
		{
			TextMode = TextBoxMode.MultiLine;
			CssClass = "freeTextArea";
		}

		#endregion

		#region Properties

		private string fileBrowserCallBack =
			@"
    var fileBrowserUrl;
    var srcField;
	function fileBrowserCallBack(field_name, url, destinationType, win) {{
        srcField = win.document.forms[0].elements[field_name];
        var fileSelectorWindow = window.open(fileBrowserUrl + '?availableModes=All&tbid='+ srcField.id +'&destinationType='+ destinationType +'&selectedUrl='+ encodeURIComponent(url), 'FileBrowser', popupOptions);
        fileSelectorWindow.focus();
	}}
    function onFileSelected(selectedUrl){{
        srcField.value = selectedUrl;
    }}
";

		private string scriptFormat =
			@"
	tinyMCE.init({{
		mode : 'exact',
        elements : '{0}',
		plugins : '{1}',
	    content_css : '{2}',
		theme : '{5}',
		plugins : 'style,layer,table,advimage,advlink,iespell,media,searchreplace,print,contextmenu,paste,fullscreen,noneditable,inlinepopups',
		theme_advanced_buttons1_add_before : '',
		theme_advanced_buttons1_add : 'sup,|,print,fullscreen,|,search,replace,iespell',
		theme_advanced_buttons2_add_before: 'cut,copy,paste,pastetext,pasteword,|',
		theme_advanced_buttons2_add : '|,table,media,insertlayer,inlinepopups',
		theme_advanced_buttons3 : '',
        theme_advanced_buttons3_add_before : '',
		theme_advanced_buttons3_add : '',
		theme_advanced_buttons4 : '',
		theme_advanced_toolbar_location : 'top',
		theme_advanced_toolbar_align : 'left',
		theme_advanced_path_location : 'bottom',
		extended_valid_elements : 'hr[class|width|size|noshade],span[class|align|style],pre[class],code[class]',
		file_browser_callback : 'fileBrowserCallBack',
		theme_advanced_resize_horizontal : false,
		theme_advanced_resizing : true,
        theme_advanced_disable : 'help,fontselect,fontsizeselect,forecolor,backcolor,styleselect',
		relative_urls : false,
		noneditable_noneditable_class : 'cs,csharp,vb,js'

	}});
    var popupOptions='{3}';
    fileBrowserUrl = '{4}';
";

		public string Plugins
		{
			get
			{
				return
					(string) ViewState["Plugins"] ??
					"table,advimage,advlink,flash,searchreplace,print,contextmenu,paste,fullscreen,noneditable";
			}
			set { ViewState["Plugins"] = value; }
		}

		public string FileBrowserUrl
		{
			get { return (string) ViewState["FileBrowserUrl"] ?? N2.Web.Url.ToAbsolute("~/Edit/FileManagement/default.aspx"); }
			set { ViewState["FileBrowserUrl"] = value; }
        }

        public string Theme
        {
            get { return (string)ViewState["Theme"] ?? "advanced"; }
            set { ViewState["Theme"] = value; }
        }

		public string CssFile
		{
			get
			{
				return (string) ViewState["CssFile"]
				       ?? N2.Context.Current.EditManager.GetEditorCssUrl();
			}
			set { ViewState["CssFile"] = value; }
        }

        public virtual string PopupOptions
        {
            get { return (string)ViewState["PopupOptions"] ?? "height=600,width=400,resizable=yes,status=yes,scrollbars=yes"; }
            set { ViewState["PopupOptions"] = value; }
        }

        public virtual bool EnableFreeTextArea
        {
            get { return (bool)(ViewState["EnableFreeTextArea"] ?? true); }
            set { ViewState["EnableFreeTextArea"] = value; }
        }

        #endregion

		#region Methods

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

            if (EnableFreeTextArea)
            {
                Page.ClientScript.RegisterClientScriptInclude("tiny_mce.js", N2.Web.Url.ToAbsolute("~/edit/js/tiny_mce/tiny_mce.js"));
                Page.ClientScript.RegisterClientScriptBlock(typeof(FreeTextArea), "fileBrowserCallBack", fileBrowserCallBack, true);

                string script = string.Format(scriptFormat,
                                              ClientID,
                                              Plugins,
                                              CssFile,
                                              PopupOptions,
                                              FileBrowserUrl, // {4}
                                              Theme
                    );
                Page.ClientScript.RegisterClientScriptBlock(typeof(FreeTextArea), ClientID, script, true);
            }
		}

		#endregion
	}
}