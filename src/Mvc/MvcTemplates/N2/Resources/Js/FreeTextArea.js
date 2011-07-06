// Extended by janpub, 11.6.2011 JH
// See TinyMCE settings, http://tinymce.moxiecode.com/wiki.php/Configuration
// Requires PDW plugin, http://www.neele.name/pdw_toggle_toolbars/
//
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

// Settings: clear 1..4 toolbars
var settingsClr = {
	theme_advanced_buttons1: '',
	theme_advanced_buttons1_add_before: '',
	theme_advanced_buttons1_add: '',
	theme_advanced_buttons2: '',
	theme_advanced_buttons2_add_before: '',
	theme_advanced_buttons2_add: '',
	theme_advanced_buttons3: '',
	theme_advanced_buttons3_add_before: '',
	theme_advanced_buttons3_add: '',
	theme_advanced_buttons4: '',
	theme_advanced_buttons4_add_before: '',
	theme_advanced_buttons4_add: ''
};

// Settings: toolbars
var settingsSimple = {
	theme: 'advanced',

	plugins: 'style,layer,table,advimage,advlink,advhr,media,'
       + 'searchreplace,print,contextmenu,paste,fullscreen,noneditable,inlinepopups,'
       + 'emotions,fullscreen,visualchars,safari,nonbreaking,xhtmlxtras,template',

	theme_advanced_buttons1: 'newdocument,|,undo,redo,|,bold,italic,underline,strikethrough,|,'
       + 'justifyleft,justifycenter,justifyright,justifyfull,|,,formatselect,styleselect,removeformat,code,',
	theme_advanced_disable: 'help,save'
};

var settingsExtended = {
	theme: 'advanced',

	plugins: 'pdw,style,layer,table,advimage,advlink,advhr,media,'
       + 'searchreplace,print,contextmenu,paste,fullscreen,noneditable,inlinepopups,'
       + 'emotions,fullscreen,visualchars,safari,nonbreaking,xhtmlxtras,template',

	theme_advanced_buttons1: 'pdw_toggle,|,newdocument,|,undo,redo,|,bold,italic,underline,strikethrough,|,'
       + 'justifyleft,justifycenter,justifyright,justifyfull,|,,formatselect,styleselect,removeformat,code,',
	theme_advanced_buttons2: 'cut,copy,paste,pastetext,pasteword,|,'
       + 'bullist,numlist,|,outdent,indent,blockquote,|,'
       + 'link,unlink,anchor,image,|,forecolor,backcolor,fontselect,fontsizeselect,',
	theme_advanced_buttons3: 'sub,sup,|,charmap,emotions,'
       + 'media,,hr,advhr,ltr,rtl,visualaid,|,cite,abbr,acronym,del,ins,attribs,cleanup,print,fullscreen',
	theme_advanced_buttons4: 'tablecontrols,|,insertlayer,moveforward,movebackward,absolute,|,styleprops,|,'
       + 'visualchars,|,search,replace,|,',
	theme_advanced_disable: 'help,save'
};

// Enable PDW plugin, toolbar1 shown by default
var settingsPDW1 = {
	pdw_toggle_on: 1,
	pdw_toggle_toolbars: "2,3,4"
};

// Enable PDW plugin, toolbars1,2 shown by default
var settingsPDW2 = {
	pdw_toggle_on: 1,
	pdw_toggle_toolbars: "3,4"
};

var staticOverrides = [];
staticOverrides["MINIMAL"] = [settingsClr, settingsSimple];
staticOverrides["SIMPLE"] = [settingsClr, settingsExtended, settingsPDW1];
staticOverrides["EXTENDED"] = [settingsClr, settingsExtended, settingsPDW2];

function freeTextArea_init(fileBrowser, overrides) {
	fileBrowserUrl = fileBrowser;

	// Default settings:
	var settings = {};  // keep freeTextArea_settings unmodified
	jQuery.extend(settings, freeTextArea_settings);

	// JH: nonstandard setting value selects a set of pre-prepared overrides: 
	var settings_set = (overrides != null ? overrides["settings_set"] : null);
	if (settings_set)
		settings_set = settings_set.toUpperCase();
	if (!settings_set || (settings_set == 'UNDEFINED'))
		settings_set = null;

	if (settings_set && staticOverrides) {
		var extras = staticOverrides[settings_set];
		if (extras) {
			for (var idx = 0; idx < extras.length; ++idx) {
				var extra1 = extras[idx];
				jQuery.extend(settings, extra1);
			}
		}
	}

	// Explicit overrides:
	jQuery.extend(settings, overrides);
	tinyMCE.init(settings);
}