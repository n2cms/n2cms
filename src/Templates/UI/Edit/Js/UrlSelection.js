n2nav.parentInputId = null;
n2nav.setOpenerSelected = function(relativeUrl) {
	if(window.opener){
		if(window.opener.onFileSelected && window.opener.srcField)
			window.opener.onFileSelected(relativeUrl);
		else
			window.opener.document.getElementById(this.parentInputId).value = relativeUrl;
		window.close();
    }
}
n2nav.onUrlSelected = function(rewrittenUrl) {
	this.setOpenerSelected(rewrittenUrl);
}
