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
n2nav.getUrl = function(a){
	return a.rel;
}
n2nav.onTargetClick = function(el){
    n2nav.displaySelection(el);
    if(n2nav.onUrlSelected)
		n2nav.onUrlSelected(n2nav.getUrl(el));
}
n2nav.targetHandlers = new Array();
n2nav.handleLink = function(i,a){
	if(n2nav.targetHandlers[a.target]){
        n2nav.targetHandlers[a.target](a,i);
	}
}
n2nav.refreshLinks = function(container){
	if(!container){
		container = n2nav.linkContainerId;
	}
	$("a", container).each( n2nav.handleLink );
}
n2nav.setupLinks = function(containerId){
	this.linkContainerId = containerId;
	this.refreshLinks();
}
n2nav.previewClickHandler = function(event){
	var a = n2nav.findLink(event.target);
    n2nav.onTargetClick(a)
    n2nav.setupToolbar(n2nav.getUrl(a));
}
n2nav.targetHandlers["preview"] = function(a,i) {
    $(a).addClass("enabled").bind("click", null, n2nav.previewClickHandler);
}
n2nav.setupToolbar = function(path){
	if(window.top.n2)
		window.top.n2.setupToolbar(path);
}




// EDIT
var n2toggle = {
    show: function(btn, bar) {
        $(btn).addClass("toggled").blur();
        $(bar).show();
        cookie.create(bar, "show");
    },
    hide: function(btn, bar) {
        $(btn).removeClass("toggled").blur();
        $(bar).hide();
        cookie.erase(bar)
    }
};


// DEFAULT
var frameManager = function(){
	this.currentUrl = "/";
}
frameManager.prototype = {
    memorize: function(selected, action) {
        document.getElementById("memory").value = selected;
        document.getElementById("action").value = action;
    },
    initFrames: function() {
        $("#splitter").splitter({
            type: 'v',
            initA: true	// use width of A (#leftPane) from styles

        });
        var t = this;
        $(document).ready(function() {
            $(window).bind("resize", function() {
                t.repaint();
            });
            t.repaint();
        });
    },
    repaint: function() {
        $("#splitter").trigger("resize");
        $("#splitter").height(this.contentHeight());
        $("#splitter *").height(this.contentHeight());
    },
    contentHeight: function() {
        return document.documentElement.clientHeight - (jQuery.browser.msie ? 50 : 51);
    },
    getSelected: function() {
        return this.currentUrl;
    },
    getMemory: function() {
        var m = document.getElementById("memory");
        return encodeURIComponent(m.value);
    },
    getAction: function() {
        var a = document.getElementById("action");
        return encodeURIComponent(a.value);
    },
    setupToolbar: function(url) {
        url = encodeURIComponent(url);
        var memory = this.getMemory();
        var action = this.getAction;
        this.currentUrl = url;
        for (var i = 0; i < toolbarPlugIns.length; i++) {
            var a = document.getElementById(toolbarPlugIns[i].linkId);
            a.href = toolbarPlugIns[i].urlFormat
				.replace("{selected}", url)
				.replace("{memory}", memory)
				.replace("{action}", action);
        }
    },
    refreshNavigation: function(navigationUrl) {
        var nav = document.getElementById('navigation');
        nav.src = navigationUrl;
    },
    refreshPreview: function(previewUrl) {
        var prev = document.getElementById('preview');
        prev.src = previewUrl;
    },
    refresh: function(navigationUrl, previewUrl) {
        this.refreshNavigation(navigationUrl);
        this.refreshPreview(previewUrl);
    },
    select: function(name) {
        $("#" + name)
			.siblings().removeClass("selected").end()
			.addClass("selected").focus();
    },
    unselect: function(name) {
        $("#" + name).removeClass("selected");
    }
}

function toolbarSelect(name){
	if(window.top.n2)
	{
		window.top.n2.select(name);
		$(window).unload(function(){
			window.top.n2.unselect(name);
		});
	}
}

