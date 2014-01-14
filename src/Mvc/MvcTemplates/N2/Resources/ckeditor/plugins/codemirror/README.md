CKEditor-CodeMirror-Plugin
==========================

Syntax Highlighting for the CKEditor (Source View) with the CodeMirror Plugin

### Available Shortcuts
* 'Ctrl-K' to comment the currently selected text
* 'Ctrl-Shift-K' to uncomment currently selected text
* 'Ctrl-Alt-K' to auto format currently selected text
* 'Ctrl-F' to perform a search
* 'Ctrl-G' to find next
* 'Ctrl-Shift-G' to find previous
* 'Ctrl-Shift-F' to find and replace
* 'Ctrl-Shift-R' to find and replace all

The Full Theme List can be found here: http://codemirror.net/demo/theme.html

![Screenshot](http://www.watchersnet.de/Portals/0/screenshots/dnn/CKEditorSourceView.png)

####License

Licensed under the terms of the MIT License.

####Installation

 1. Extract the contents of the file into the "plugins" folder of CKEditor.
 2. In the CKEditor configuration file (config.js) add the following code:

````
config.extraPlugins = '[ codemirror ]';

config.codemirror = {
	
	// Set this to the theme you wish to use (codemirror themes)
	theme: 'default',
	
	// Whether or not you want to show line numbers
	lineNumbers: true,
	
	// Whether or not you want to use line wrapping
	lineWrapping: true,
	
	// Whether or not you want to highlight matching braces
	matchBrackets: true,
	
	// Whether or not you want tags to automatically close themselves
	autoCloseTags: true,
	
	// Whether or not you want Brackets to automatically close themselves
	autoCloseBrackets: true,
	
	// Whether or not to enable search tools, CTRL+F (Find), CTRL+SHIFT+F (Replace), CTRL+SHIFT+R (Replace All), CTRL+G (Find Next), CTRL+SHIFT+G (Find Previous)
	enableSearchTools: true,
	
	// Whether or not you wish to enable code folding (requires 'lineNumbers' to be set to 'true')
	enableCodeFolding: true,
	
	// Whether or not to enable code formatting
	enableCodeFormatting: true,
	
	// Whether or not to automatically format code should be done every time the source view is opened
	autoFormatOnStart: true,
	
	// Whether or not to automatically format code which has just been uncommented
	autoFormatOnUncomment: true,
	
	// Whether or not to highlight the currently active line
	highlightActiveLine: true,
	
	// Whether or not to highlight all matches of current word/selection
	highlightMatches: true,
	
	// Whether or not to show the format button on the toolbar
	showFormatButton: true,
	
	// Whether or not to show the comment button on the toolbar
	showCommentButton: true,
	
	// Whether or not to show the uncomment button on the toolbar
	showUncommentButton: true
};

````
