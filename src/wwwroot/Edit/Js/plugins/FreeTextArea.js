var freeTextArea_settings = {
	mode : 'exact',
	plugins : 'table,advimage,advlink,flash,searchreplace,print,contextmenu,paste,fullscreen,noneditable',
	theme : 'advanced',
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
	extended_valid_elements : 'hr[class|width|size|noshade],span[class|align|style],pre[class],code[class],iframe[src|width|height|name|align]',
	file_browser_callback : 'fileBrowserCallBack',
	theme_advanced_resize_horizontal : false,
	theme_advanced_resizing : true,
    theme_advanced_disable : 'help,fontselect,fontsizeselect,forecolor,backcolor,styleselect',
	relative_urls : false,
	noneditable_noneditable_class : 'cs,csharp,vb,js',
	encoding: 'xml',
	/*decode:*/cleanup_callback: function(type, value) { return 'insert_to_editor' == type ? tinymce.DOM.decode(value) : value; }
};
var fileBrowserUrl;
var srcField;
function fileBrowserCallBack(field_name, url, destinationType, win) {
    srcField = win.document.forms[0].elements[field_name];
    var fileSelectorWindow = window.open(fileBrowserUrl + '?availableModes=All&tbid='+ srcField.id +'&destinationType='+ destinationType +'&selectedUrl='+ encodeURIComponent(url), 'FileBrowser', 'height=600,width=400,resizable=yes,status=yes,scrollbars=yes');
    fileSelectorWindow.focus();
}
function onFileSelected(selectedUrl) {
    srcField.value = selectedUrl;
}
function freeTextArea_init(fileBrowser, overrides) {
    fileBrowserUrl = fileBrowser;
    jQuery.extend(freeTextArea_settings, overrides);
    tinyMCE.init(freeTextArea_settings);
}