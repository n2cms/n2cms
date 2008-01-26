#region License

/* Copyright (C) 2006 Cristian Libardo
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 */

#endregion

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
			CssClass = "FreeTextArea";
		}

		#endregion

		#region Properties

		private string fileBrowserCallBack =
			@"
    var fileBrowserUrl;
    var srcField;
	function fileBrowserCallBack(field_name, url, destinationType, win) {{
        srcField = win.document.forms[0].elements[field_name];
        var fileSelectorWindow = window.open(fileBrowserUrl + '?availableModes=All&tbid='+ srcField.id +'&destinationType='+ destinationType +'&selected='+ encodeURIComponent(url), 'FileBrowser', popupOptions);
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
		theme : 'advanced',
		plugins : 'style,layer,table,advimage,advlink,iespell,flash,searchreplace,print,contextmenu,paste,fullscreen,noneditable',
		theme_advanced_buttons1_add_before : '',
		theme_advanced_buttons1_add : 'sup,|,print,fullscreen,|,search,replace,iespell',
		theme_advanced_buttons2_add_before: 'cut,copy,paste,pastetext,pasteword,|',
		theme_advanced_buttons2_add : '|,table,flash,insertlayer',
		theme_advanced_buttons3 : '',
        theme_advanced_buttons3_add_before : '',
		theme_advanced_buttons3_add : '',
		theme_advanced_buttons4 : '',
		theme_advanced_toolbar_location : 'top',
		theme_advanced_toolbar_align : 'left',
		theme_advanced_path_location : 'bottom',
	    plugin_insertdate_dateFormat : '%Y-%m-%d',
	    plugin_insertdate_timeFormat : '%H:%M:%S',
		extended_valid_elements : 'hr[class|width|size|noshade],font[face|size|color|style],span[class|align|style],pre[class],code[class]',
		external_link_list_url : 'example_link_list.js',
		external_image_list_url : 'example_image_list.js',
		flash_external_list_url : 'example_flash_list.js',
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
			get { return (string) ViewState["FileBrowserUrl"] ?? Utility.ToAbsolute("~/Edit/FileManagement/default.aspx"); }
			set { ViewState["FileBrowserUrl"] = value; }
		}

		public string CssFile
		{
			get
			{
				return (string) ViewState["CssFile"]
				       ?? N2.Context.Instance.EditManager.GetEditorCssUrl();
			}
			set { ViewState["CssFile"] = value; }
		}

		public virtual string PopupOptions
		{
			get { return (string) ViewState["PopupOptions"] ?? "height=600,width=400,resizable=yes,status=yes,scrollbars=yes"; }
			set { ViewState["PopupOptions"] = value; }
		}

		#endregion

		#region Methods

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			Page.ClientScript.RegisterClientScriptInclude("tiny_mce.js",
			                                              Utility.ToAbsolute("~/edit/js/tiny_mce/tiny_mce.js?v3"));
			Page.ClientScript.RegisterClientScriptBlock(typeof (FreeTextArea), "fileBrowserCallBack", fileBrowserCallBack, true);

			string script = string.Format(scriptFormat,
			                              ClientID,
			                              Plugins,
			                              CssFile,
			                              PopupOptions,
			                              FileBrowserUrl
				);
			Page.ClientScript.RegisterClientScriptBlock(typeof (FreeTextArea), ClientID, script, true);
		}

		#endregion
	}
}