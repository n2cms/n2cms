// NAVIGATION
var n2nav = new Object();

n2nav.linkContainerId = null;
n2nav.hostName = window.location.hostname;
n2nav.toRelativeUrl = function(absoluteUrl) {
    if(absoluteUrl.indexOf(n2nav.hostName)>0)
        return absoluteUrl.replace(/.*?:\/\/.*?\//, "/");
    return absoluteUrl;
}
n2nav.onUrlSelected = null;
n2nav.findLink = function(el) {
	while(el && el.tagName != "A")
		el = el.parentNode;
	return el;
}
n2nav.displaySelection = function(el){
    $(".selected").removeClass("selected");
    $(el).addClass("selected");
}
n2nav.onTargetClick = function(el){
    n2nav.displaySelection(el);
    if(n2nav.onUrlSelected)
		n2nav.onUrlSelected(el.rel);
}
n2nav.getJQuery = function(){
	return n2nav.linkContainerId + " a";
}
n2nav.targetHandlers = new Array();
n2nav.handleLink = function(i,a){
	if(n2nav.targetHandlers[a.target])
		n2nav.targetHandlers[a.target](a,i);
}
n2nav.refreshLinks = function(){
	$(this.getJQuery()).each( n2nav.handleLink );
}
n2nav.setupLinks = function(containerId){
	this.linkContainerId = containerId;
	this.refreshLinks();
}
n2nav.previewClickHandler = function(event){
	var a = n2nav.findLink(event.target);
    n2nav.onTargetClick(a)
    n2nav.setupToolbar(a.rel);
}
n2nav.targetHandlers["preview"] = function(a,i) {
    $(a).addClass("enabled").bind("click", null, n2nav.previewClickHandler);
}
n2nav.setupToolbar = function(url){
	if(window.top.n2)window.top.n2.setupToolbar(url.replace(/.*?:\/\/.*?\//, "/"));
}











// EDIT
function edit(){}
edit.show = function(btn, bar){
    $(btn).addClass("toggled").blur();
    $(bar).show();
    cookie.create(bar, "show");
}
edit.hide = function(btn, bar){
    $(btn).removeClass("toggled").blur();
    $(bar).hide();
    cookie.erase(bar);
}

$(document).ready( function() {
	$(".right fieldset").hide();
	
	$(".showInfo").toggle(function(){
	    edit.show(this, ".infoBox");
	}, function(){
	    edit.hide(this, ".infoBox");
	});
	
	$(".showZones").toggle(function(){
        edit.show(this, ".zonesBox");
	}, function(){
        edit.hide(this, ".zonesBox");
	});
	
	if(cookie.read(".infoBox"))
	    $(".showInfo").click();
	if(cookie.read(".zonesBox"))
	    $(".showZones").click();
});










// DEFAULT
var frameManager = function(){
	this.currentUrl = "/";
}
frameManager.prototype = {
	memorize: function(selected,action){
		document.getElementById("memory").value = selected;
		document.getElementById("action").value = action;
	},
	initFrames: function() {
		$("#splitter").splitter({
			type: 'v',
			initA: true,	// use width of A (#leftPane) from styles
			accessKey: '|'
		});
		var t = this;
		$(document).ready(function(){
			$(window).bind("resize", function(){
				t.repaint();
			}).trigger("resize");
		});
	},
	repaint: function() {
		$("#splitter").trigger("resize"); 
		$("#splitter").height(this.contentHeight());
		$("#splitter *").height(this.contentHeight());
	},
	contentHeight: function() {
		return document.documentElement.clientHeight - (jQuery.browser.msie?32:33);
	},
	getSelected: function(){
		return this.currentUrl;
	},
	getMemory: function(){
		var m = document.getElementById("memory");
		return encodeURIComponent(m.value);
	},
	getAction: function(){
		var a = document.getElementById("action");
		return encodeURIComponent(a.value);
	},
	setupToolbar: function(url) {
		url = encodeURIComponent(url);
		var memory = this.getMemory();
		var action = this.getAction;
		this.currentUrl = url;
		for(var i=0; i<toolbarPlugIns.length; i++)
		{
			var a = document.getElementById(toolbarPlugIns[i].linkId);
			a.href = toolbarPlugIns[i].urlFormat
				.replace("{selected}", url)
				.replace("{memory}", memory)
				.replace("{action}", action);
		}
	},
	refreshNavigation: function(navigationUrl){
		document.getElementById('navigation').src = navigationUrl;
	},
	refreshPreview: function(previewUrl){
		document.getElementById('preview').src = previewUrl;
	},
	refresh: function(navigationUrl, previewUrl){
		this.refreshNavigation(navigationUrl);
		this.refreshPreview(previewUrl);
	}
}



