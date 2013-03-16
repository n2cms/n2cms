/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

(function () {
	// Adds (additional) arguments to given url.
	//
	// @param {String}
	//            url The url.
	// @param {Object}
	//            params Additional parameters.
	function addQueryString(url, params) {
		var queryString = [];

		if (!params)
			return url;
		else {
			for (var i in params)
				queryString.push(i + "=" + encodeURIComponent(params[i]));
		}

		return url + ((url.indexOf("?") != -1) ? "&" : "?") + queryString.join("&");
	}

	function modifyParams(params, editor, dialog) {
		if (editor.config.filebrowserBrowseOverrideParams) {
			params = CKEDITOR.tools.clone(params);
			if (typeof editor.config.filebrowserBrowseOverrideParams == "function") {
				params = editor.config.filebrowserBrowseOverrideParams(params, editor, dialog);
			} else if (typeof editor.config.filebrowserBrowseOverrideParams == "Object") {
				params = CKEDITOR.tools.extend(params, editor.config.filebrowserBrowseOverrideParams);
			}
		}
		return params;
	}

	// Make a string's first character uppercase.
	//
	// @param {String}
	//            str String.
	function ucFirst(str) {
		str += '';
		var f = str.charAt(0).toUpperCase();
		return f + str.substr(1);
	}

	// The onlick function assigned to the 'Browse Server' button. Opens the
	// file browser and updates target field when file is selected.
	//
	// @param {CKEDITOR.event}
	//            evt The event object.
	function browseServer(evt) {
		var dialog = this.getDialog();
		var editor = dialog.getParentEditor();

		editor._.filebrowserSe = this;

		var width = editor.config['filebrowser' + ucFirst(dialog.getName()) + 'WindowWidth'] || editor.config.filebrowserWindowWidth || '80%';
		var height = editor.config['filebrowser' + ucFirst(dialog.getName()) + 'WindowHeight'] || editor.config.filebrowserWindowHeight || '70%';

		var params = this.filebrowser.params || {};
		params.CKEditor = editor.name;
		params.CKEditorFuncNum = editor._.filebrowserFn;
		if (!params.langCode)
			params.langCode = editor.langCode;

		params = modifyParams(params, editor, dialog);

		var url = addQueryString(this.filebrowser.url, params);
		// TODO: V4: Remove backward compatibility (#8163).
		editor.popup(url, width, height, editor.config.filebrowserWindowFeatures || editor.config.fileBrowserWindowFeatures);
	}

	// The onlick function assigned to the 'Upload' button. Makes the final
	// decision whether form is really submitted and updates target field when
	// file is uploaded.
	//
	// @param {CKEDITOR.event}
	//            evt The event object.
	function uploadFile(evt) {
		var dialog = this.getDialog();
		var editor = dialog.getParentEditor();

		editor._.filebrowserSe = this;

		// If user didn't select the file, stop the upload.
		if (!dialog.getContentElement(this['for'][0], this['for'][1]).getInputElement().$.value)
			return false;

		if (!dialog.getContentElement(this['for'][0], this['for'][1]).getAction())
			return false;

		return true;
	}

	// Setups the file element.
	//
	// @param {CKEDITOR.ui.dialog.file}
	//            fileInput The file element used during file upload.
	// @param {Object}
	//            filebrowser Object containing filebrowser settings assigned to
	//            the fileButton associated with this file element.
	function setupFileElement(editor, fileInput, filebrowser) {
		var params = filebrowser.params || {};
		params.CKEditor = editor.name;
		params.CKEditorFuncNum = editor._.filebrowserFn;
		if (!params.langCode)
			params.langCode = editor.langCode;

		fileInput.action = addQueryString(filebrowser.url, params);
		fileInput.filebrowser = filebrowser;
	}

	// Traverse through the content definition and attach filebrowser to
	// elements with 'filebrowser' attribute.
	//
	// @param String
	//            dialogName Dialog name.
	// @param {CKEDITOR.dialog.definitionObject}
	//            definition Dialog definition.
	// @param {Array}
	//            elements Array of {@link CKEDITOR.dialog.definition.content}
	//            objects.
	function attachFileBrowser(editor, dialogName, definition, elements) {
		var element, fileInput;

		for (var i in elements) {
			element = elements[i];

			if (element.type == 'hbox' || element.type == 'vbox' || element.type == 'fieldset')
				attachFileBrowser(editor, dialogName, definition, element.children);

			if (!element.filebrowser)
				continue;

			if (typeof element.filebrowser == 'string') {
				var fb = {
					action: (element.type == 'fileButton') ? 'QuickUpload' : 'Browse',
					target: element.filebrowser
				};
				element.filebrowser = fb;
			}

			if (element.filebrowser.action == 'Browse') {
				var url = element.filebrowser.url;
				if (url === undefined) {
					url = editor.config['filebrowser' + ucFirst(dialogName) + 'BrowseUrl'];
					if (url === undefined)
						url = editor.config.filebrowserBrowseUrl;
				}

				if (url) {
					element.onClick = browseServer;
					element.filebrowser.url = url;
					element.hidden = false;
				}
			} else if (element.filebrowser.action == 'QuickUpload' && element['for']) {
				url = element.filebrowser.url;
				if (url === undefined) {
					url = editor.config['filebrowser' + ucFirst(dialogName) + 'UploadUrl'];
					if (url === undefined)
						url = editor.config.filebrowserUploadUrl;
				}

				if (url) {
					var onClick = element.onClick;
					element.onClick = function (evt) {
						// "element" here means the definition object, so we need to find the correct
						// button to scope the event call
						var sender = evt.sender;
						if (onClick && onClick.call(sender, evt) === false)
							return false;

						return uploadFile.call(sender, evt);
					};

					element.filebrowser.url = url;
					element.hidden = false;
					setupFileElement(editor, definition.getContents(element['for'][0]).get(element['for'][1]), element.filebrowser);
				}
			}
		}
	}

	// Updates the target element with the url of uploaded/selected file.
	//
	// @param {String}
	//            url The url of a file.
	function updateTargetElement(url, sourceElement) {
		var dialog = sourceElement.getDialog();
		var targetElement = sourceElement.filebrowser.target || null;

		// If there is a reference to targetElement, update it.
		if (targetElement) {
			var target = targetElement.split(':');
			var element = dialog.getContentElement(target[0], target[1]);
			if (element) {
				element.setValue(url);
				dialog.selectPage(target[0]);
			}
		}
	}

	// Returns true if filebrowser is configured in one of the elements.
	//
	// @param {CKEDITOR.dialog.definitionObject}
	//            definition Dialog definition.
	// @param String
	//            tabId The tab id where element(s) can be found.
	// @param String
	//            elementId The element id (or ids, separated with a semicolon) to check.
	function isConfigured(definition, tabId, elementId) {
		if (elementId.indexOf(";") !== -1) {
			var ids = elementId.split(";");
			for (var i = 0; i < ids.length; i++) {
				if (isConfigured(definition, tabId, ids[i]))
					return true;
			}
			return false;
		}

		var elementFileBrowser = definition.getContents(tabId).get(elementId).filebrowser;
		return (elementFileBrowser && elementFileBrowser.url);
	}

	function setUrl(fileUrl, data) {
		var dialog = this._.filebrowserSe.getDialog(),
			targetInput = this._.filebrowserSe['for'],
			onSelect = this._.filebrowserSe.filebrowser.onSelect;

		if (targetInput)
			dialog.getContentElement(targetInput[0], targetInput[1]).reset();

		if (typeof data == 'function' && data.call(this._.filebrowserSe) === false)
			return;

		if (onSelect && onSelect.call(this._.filebrowserSe, fileUrl, data) === false)
			return;

		// The "data" argument may be used to pass the error message to the editor.
		if (typeof data == 'string' && data)
			alert(data);

		if (fileUrl)
			updateTargetElement(fileUrl, this._.filebrowserSe);
	}

	CKEDITOR.plugins.add('filebrowser2', {
		requires: 'popup',
		init: function (editor, pluginPath) {
			editor._.filebrowserFn = CKEDITOR.tools.addFunction(setUrl, editor);
			editor.on('destroy', function () {
				CKEDITOR.tools.removeFunction(this._.filebrowserFn);
			});
		}
	});

	CKEDITOR.on('dialogDefinition', function (evt) {
		var definition = evt.data.definition,
			element;
		// Associate filebrowser to elements with 'filebrowser' attribute.
		for (var i in definition.contents) {
			if ((element = definition.contents[i])) {
				attachFileBrowser(evt.editor, evt.data.name, definition, element.elements);
				if (element.hidden && element.filebrowser) {
					element.hidden = !isConfigured(definition, element['id'], element.filebrowser);
				}
			}
		}
	});

})();

function freeTextArea_init(fileBrowser, overrides) {
	fileBrowserUrl = fileBrowser;
	//console.log("freeTextArea_init", arguments);

	//0: "/N2/Content/Navigation/Tree.aspx"
	//1: Object
	//content_css: "/N2/Resources/Css/Editor.css"
	//elements: "ctl00_ctl00_Frame_Content_ie_Text"
	//language: "en"
	//settings_set: "Simple"
	//tokencomplete_enabled: true
	//tokencomplete_settings: Object
	//tokens: Array[26]
	//0: Object
	//Name: "Author"
	//Options: null
	//__proto__: Object

	function getBrowserUrl(destinationType) {
		var modes = "All";
		var location = "selection";
		var types = "";
		if (destinationType == "image") {
			modes = "Files";
			location = "filesselection";
			types = "IFileSystemFile";
		}

		return "/N2/Content/Navigation/Tree.aspx"
			+ '?location=' + location
			+ '&availableModes=' + modes
			//+ '&tbid=' + srcField.id
			//+ '&destinationType=' + destinationType
			//+ '&selectedUrl=' + encodeURIComponent(url)
			+ '&selectableTypes=' + types;
	}

	var config = {
		uiColor: '#ddddcc',
		extraPlugins: 'filebrowser2', // divarea
		removePlugins: 'filebrowser,about', // elementspath,resize,
		filebrowserBrowseUrl: getBrowserUrl("any"),
		filebrowserImageBrowseUrl: getBrowserUrl("image"),
		filebrowserFlashBrowseUrl: getBrowserUrl("any"),
		//filebrowserUploadUrl: getBrowserUrl("any"),
		//filebrowserImageUploadUrl: getBrowserUrl("image"),
		//filebrowserFlashUploadUrl: getBrowserUrl("any"),
		filebrowserImageWindowWidth: '300',
		filebrowserImageWindowHeight: '600',
		filebrowserBrowseOverrideParams: function (params, editor, dialog) {
			params.selectedUrl = dialog.getContentElement.apply(dialog, editor._.filebrowserSe.filebrowser.target.split(":")).getValue();
			return params;
		},
	};

	if (overrides.settings_set == "Fixed") {

	} else if (overrides.settings_set == "Minimal") {
		delete config.toolbarGroups;
		config.toolbar = [['Cut', 'Copy', 'Paste', '-', 'Undo'], ['Bold', 'Italic', 'RemoveFormat'], ['NumberedList', 'BulletedList'], ['Format']];
	} else if (overrides.settings_set == "Simple") {
		config.toolbar = [['Cut', 'Copy', 'Paste', 'PasteFromWord', '-', 'Undo'], ['Link', 'Unlink'], ['Image', 'Table'], ['Bold', 'Italic', 'RemoveFormat'], ['NumberedList', 'BulletedList'], ['Format'], ['Maximize']];
	} else if (overrides.settings_set == "Extended") {
		//// Toolbar configuration generated automatically by the editor based on config.toolbarGroups.
		//config.toolbar = [
		//	{ name: 'document', groups: ['mode', 'document', 'doctools'], items: ['Source'] },
		//	{ name: 'clipboard', groups: ['clipboard', 'undo'], items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
		//	{ name: 'editing', groups: ['find', 'selection', 'spellchecker'], items: ['Scayt'] },
		//	'/',
		//	{ name: 'basicstyles', groups: ['basicstyles', 'cleanup'], items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'] },
		//	{ name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align'], items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote'] },
		//	{ name: 'links', items: ['Link', 'Unlink', 'Anchor'] },
		//	{ name: 'insert', items: ['Image', 'Table', 'HorizontalRule', 'SpecialChar'] },
		//	'/',
		//	{ name: 'styles', items: ['Styles', 'Format'] },
		//	{ name: 'tools', items: ['Maximize'] },
		//	{ name: 'others', items: ['-'] },
		//	{ name: 'about', items: ['About'] }
		//];

		//// Toolbar groups configuration.
		//config.toolbarGroups = [
		//	{ name: 'document', groups: ['mode', 'document', 'doctools'] },
		//	{ name: 'clipboard', groups: ['clipboard', 'undo'] },
		//	{ name: 'editing', groups: ['find', 'selection', 'spellchecker'] },
		//	{ name: 'forms' },
		//	{ name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
		//	{ name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align'] },
		//	'/',
		//	{ name: 'links' },
		//	{ name: 'insert' },
		//	{ name: 'styles' },
		//	{ name: 'colors' },
		//	{ name: 'tools' },
		//	{ name: 'others' },
		//	{ name: 'about' }
		//];
	}

	var editor = CKEDITOR.replace(overrides.elements, config);
}