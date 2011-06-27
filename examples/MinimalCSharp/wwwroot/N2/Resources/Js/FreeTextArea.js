var freeTextArea_settings = {
    mode: 'exact',
    //plugins: 'table,advimage,advlink,flash,searchreplace,print,contextmenu,paste,fullscreen,noneditable',
    theme: 'advanced',
    plugins: 'style,layer,table,advimage,advlink,iespell,spellchecker,media,searchreplace,print,contextmenu,paste,fullscreen,noneditable,inlinepopups',
    theme_advanced_buttons1_add_before: '',
    theme_advanced_buttons1_add: 'sup,|,print,fullscreen,|,search,replace,iespell,spellchecker,autosave',
    theme_advanced_buttons2_add_before: 'cut,copy,paste,pastetext,pasteword,|',
    theme_advanced_buttons2_add: '|,table,media,insertlayer,inlinepopups',
    theme_advanced_buttons3: '',
    theme_advanced_buttons3_add_before: '',
    theme_advanced_buttons3_add: '',
    theme_advanced_buttons4: '',
    theme_advanced_toolbar_location: 'top',
    theme_advanced_toolbar_align: 'left',
    theme_advanced_path_location: 'bottom',
    extended_valid_elements: 'hr[class|width|size|noshade],span[class|align|style],pre[class],code[class],iframe[src|width|height|name|align]',
    file_browser_callback: 'fileBrowserCallBack',
    theme_advanced_resize_horizontal: false,
    theme_advanced_resizing: true,
    theme_advanced_disable: 'help,fontselect,fontsizeselect,forecolor,backcolor,styleselect',
    relative_urls: false,
    noneditable_noneditable_class: 'cs,csharp,vb,js',
    tab_focus: ':prev,:next',
    theme_advanced_resize_horizontal: true
};
var fileBrowserUrl;
var srcField;
function fileBrowserCallBack(field_name, url, destinationType, win) {
    srcField = win.document.forms[0].elements[field_name];
    var modes = "All";
    var location = "selection";
    if (destinationType == "image") {
    	modes = "Files";
    	location = "filesselection";
    }
    tinymce.activeEditor.windowManager.open({
		file: fileBrowserUrl + (fileBrowserUrl.indexOf('?') >= 0 ? "&" : "?") + 'location=' + location + '&availableModes=' + modes + '&tbid=' + srcField.id + '&destinationType=' + destinationType + '&selectedUrl=' + encodeURIComponent(url),
		height: 500,
		width: 400,
		close_previous: false,
		inline: true,
		scrollbars: true,
		resizable: true,
		maximizable: true
	});
}
function onFileSelected(selectedUrl) {
	srcField.value = selectedUrl;
}
function freeTextArea_init(fileBrowser, overrides) {
    fileBrowserUrl = fileBrowser;
    jQuery.extend(freeTextArea_settings, overrides);
    tinyMCE.init(freeTextArea_settings);
}