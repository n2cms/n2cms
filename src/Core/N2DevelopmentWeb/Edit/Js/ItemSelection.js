n2nav.onUrlSelected = function(rewrittenUrl) {
	for(var i=0; i<linkArray.length; i++) {
		if(linkArray[i].key == rewrittenUrl) {
			this.setOpenerSelected(linkArray[i].value);
			return;
		}
	}
	this.setOpenerSelected(rewrittenUrl);
}
n2nav.linkClickHandler = function(event){
    $("#toolbar").className = "toolbar link";
    var a = n2nav.findLink(event.target);
    n2nav.onTargetClick(a)
    event.preventDefault();
}
n2nav.targetHandlers["link"] = function(a,i) {
    var relativeUrl = n2nav.toRelativeUrl(a.href);
    $(a).addClass("enabled").bind("click", null, n2nav.linkClickHandler);
    a.target = "";
}
