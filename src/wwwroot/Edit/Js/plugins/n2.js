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
    if (window.n2ctx)
        window.n2ctx.setupToolbar(path);
}




// EDIT
var n2toggle = {
    show: function(btn, bar) {
        $(btn).addClass("toggled").blur();
        $(bar).show();
        $.cookie(bar, "show");
    },
    hide: function(btn, bar) {
        $(btn).removeClass("toggled").blur();
        $(bar).hide();
        $.cookie(bar, null)
    }
};

var initn2context = function(w) {
    if (w.n2ctx)
        return w.n2ctx;

    try {
        if (w.name != "top" && w != w.parent) {
            w.n2ctx = initn2context(w.parent);
            return w.n2ctx;
        }
    } catch (e) { }

    w.n2ctx = {
        selectedPath: "/",
        memorizedPath: null,
        actionType: null,

        // whether there is a top frame
        hasTop: function() {
            return false;
        },

        // selects a toolbar item by name
        toolbarSelect: function(name) {
            jQuery(document).ready(function() {
                w.n2.select(name);
                $(window).unload(function() {
                    w.n2.unselect(name);
                });
            });
        },

        // copy/paste
        memorize: function(selected, action) {
            this.memorizedPath = selected;
            this.actionType = action;
        },
        getSelected: function() {
            return this.selectedPath;
        },
        getMemory: function() {
            return encodeURIComponent(this.memorizedPath);
        },
        getAction: function() {
            return encodeURIComponent(this.actionType);
        },

        // selection memory
        setupToolbar: function(url) {
            if (!this.hasTop()) return;
            url = encodeURIComponent(url);
            var memory = this.getMemory();
            var action = this.getAction();
            this.selectedPath = url;
            for (var i = 0; i < toolbarPlugIns.length; i++) {
                var a = w.document.getElementById(toolbarPlugIns[i].linkId);
                a.href = toolbarPlugIns[i].urlFormat
		            .replace("{selected}", url)
		            .replace("{memory}", memory)
		            .replace("{action}", action);
            }
        },

        // update frames
        refreshNavigation: function(navigationUrl) {
            if (!this.hasTop()) return;
            var nav = w.document.getElementById('navigation');
            nav.src = navigationUrl;
        },
        refreshPreview: function(previewUrl) {
            if (this.hasTop()) {
                var previewFrame = w.document.getElementById('preview');
                previewFrame.src = previewUrl;
            } else {
                window.location = previewUrl;
            }
        },
        refresh: function(navigationUrl, previewUrl) {
            this.refreshNavigation(navigationUrl);
            this.refreshPreview(previewUrl);
        },

        // toolbar selection
        select: function(name) {
            jQuery("#" + name)
	            .siblings().removeClass("selected").end()
	            .addClass("selected").focus();
        },
        unselect: function(name) {
            jQuery("#" + name).removeClass("selected");
        }
    };

    return w.n2ctx;
};
window.n2 = initn2context(window);

window.n2.frameManager = {
    init: function() {
        var t = this;
        $(document).ready(function() {
            t.repaint();
            $("#splitter").splitter({
                type: 'v',
                cookie: 'n2spl',
                anchorToWindow: true,
                onStart: function() {
                    this.parent().addClass("activeSplitter");
                },
                onStop: function() {
                    this.parent().removeClass("activeSplitter");
                    t.repaint();
                },
                sizeLeft: true
            });
            $(window).bind("resize", function() {
                t.repaint();
            });
            setTimeout(function() { t.repaint.call(t); }, 100); // chrome hack
        });
    },
    repaint: function() {
        var h = this.contentHeight();
        jQuery("#splitter").trigger("resize")
            .height(h)
            .find("div,iframe").height(h);
    },
    contentHeight: function() {
        return window.document.documentElement.clientHeight - (jQuery.browser.msie ? 50 : 51);
    }
};