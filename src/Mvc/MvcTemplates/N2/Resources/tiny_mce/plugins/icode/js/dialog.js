tinyMCEPopup.requireLangPack();

var icodeDialog = {
	init: function () {
		var f = document.forms[0];

		// Get the selected contents as text and place it in the input
		//f.someval.value = tinyMCEPopup.editor.selection.getContent({format : 'text'});		
	},

	insert: function () {
		// Insert the contents from the input into the document
		tinyMCEPopup.editor.execCommand('mceInsertContent', false, GetFormatedCode());
		tinyMCEPopup.close();
	},

	trydDetect: function (box, ddl) {
		var t = box.value;
		function sel(val) {
			for (var i = 0; i < ddl.options.length; i++ ) {
				if (ddl.options[i].value === val) {
					ddl.selectedIndex = i;
				}
			}
		}
		if (t.match(/function[(]/))
			sel('js');
		else if (t.match(/(public)|(protected)/))
			sel('csharp');
		else if (t.match(/begin/))
			sel('vb');
		else if (t.match(/select/))
			sel('Sql');
		else if (t.match(/(<div)|(<span)|(<a)/))
			sel('html');
		else if (t.match(/<.*?>/))
			sel('html');
		else if (t.match(/((background)|(font)|(border)|(padding)|(margin)).*[:]/))
			sel('Css');
	}
};

function GetFormatedCode() {
    var strCode = document.forms[0].txtCode.value;

    strCode = strCode.replace(/</gi,"&lt;");
    strCode = strCode.replace(/>/gi, "&gt;");
    //strCode = strCode.replace(/&gt;/gi, ">");
    var strCodeText = '<div id="CodeDiv" dir="ltr"><pre  class="brush: ' + document.forms[0].selctLanguage.value + '">';
    strCodeText += strCode;
    strCodeText += '</pre></div><br/>'    
    return strCodeText;
    //alert("done");
}

tinyMCEPopup.onInit.add(icodeDialog.init, icodeDialog);
