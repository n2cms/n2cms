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
//n2nav.getUrl = function(a){
//	return a.rel;
//}
n2nav.getPath = function(a) {
	return $(a).attr("data-path");
}
n2nav.onTargetClick = function(el){
    n2nav.displaySelection(el);
    if(n2nav.onUrlSelected)
    	n2nav.onUrlSelected(n2nav.getPath(el));
}
//n2nav.targetHandlers = new Array();
n2nav.handlers = {
	fallback: function(e) {
		n2nav.onTargetClick(this)
		n2nav.setupToolbar(n2nav.getPath(this), this.href);
	}
};
//n2nav.handleLink = function(i,a){
//	if(n2nav.targetHandlers[a.target]){
//        n2nav.targetHandlers[a.target](a,i);
//	}
//}
//n2nav.refreshLinks = function(container) {
//	console.log("refreshLinks ", n2nav);
//	if (!container) {
//		container = n2nav.linkContainerId;
//	}
//	$("a", container).each(n2nav.handleLink);
//}
//n2nav.setupLinks = function(containerId){
//	this.linkContainerId = containerId;
//	//this.refreshLinks();
//}
//n2nav.previewClickHandler = function(event){
//	var a = n2nav.findLink(event.target);
//    n2nav.onTargetClick(a)
//    n2nav.setupToolbar(n2nav.getUrl(a), a.href);
//}

//n2nav.targetHandlers["preview"] = function(a,i) {
//    $(a).addClass("enabled").bind("click", null, n2nav.previewClickHandler);
//}
n2nav.setupToolbar = function(path,url){
    if (window.n2ctx)
        window.n2ctx.setupToolbar(path,url);
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
		_path: "/",
		selectedUrl: null,
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
		path: function(value) {
			if (arguments.length == 0)
				return this._path;

			window.location.hash = this._path;
			
			this._path = value;
			return this;
		},
		getSelectedUrl: function() {
			return this.selectedUrl;
		},
		getMemory: function() {
			return encodeURIComponent(this.memorizedPath);
		},
		getAction: function() {
			return encodeURIComponent(this.actionType);
		},

		initToolbar: function() {
			$("a.command").click(function(e) {
				if (this.hash == "#stop")
					e.preventDefault();
			});
		},

		// selection memory
		setupToolbar: function(path, url) {
			if (!this.hasTop()) return;

			url = url || this.selectedUrl;
			var memory = this.getMemory();
			var action = this.getAction();
			this.selectedPath = path;
			this.selectedUrl = url;

			if (typeof (toolbarPlugIns) == "undefined")
				return;
			for (var i = 0; i < toolbarPlugIns.length; i++) {
				var a = w.document.getElementById(toolbarPlugIns[i].linkId);
				var href = toolbarPlugIns[i].urlFormat;
				var formats = { url: url, selected: path, memory: memory, action: action };
				for (var key in formats) {
					var format = "{" + key + "}";
					if (href.indexOf(format) >= 0 && formats[key] == "null") {
						href = "#stop";
						$(a).addClass("disabled");
						break;
					}
					else $(a).removeClass("disabled");

					href = href.replace(format, formats[key]);
				}
				a.href = href;

			}
		},
		
		append: function(url, data){
			return url + (url.indexOf('?')>=0 ? "&" : "?") + jQuery.param(data); 
		},

		// update frames
		refreshNavigation: function(values) {
			if (!this.hasTop()) return;
			if (this.path() == values.path) return;

			this.path(values.path);

			var nav = w.document.getElementById("navigationFrame");
			nav.src = this.append(values.navigationUrl, { location: this.location });
		},
		refreshPreview: function(previewUrl) {
			if (this.hasTop()) {
				var previewFrame = w.document.getElementById("previewFrame");
				previewFrame.src = previewUrl;
			} else {
				window.location = previewUrl;
			}
		},
		refresh: function(navigationUrl, previewUrl) {
			this.refreshNavigation({ navigationUrl: navigationUrl });
			this.refreshPreview(previewUrl);
		},

		// toolbar selection
		select: function(name) {
			if (!name) return;

			$s = jQuery("#" + name);
			var selectedTarget = $s.find("a").attr("target");
			$(".selected a").filter(function() { return this.target === selectedTarget || !this.target; })
                .closest(".selected")
                .each(function() {
                	n2.unselect(this.id);
                });
			$s.addClass("selected");
			jQuery(document.body).addClass(name + "Selected");
		},
		unselect: function(name) {
			if (!name) return;

			jQuery("#" + name).removeClass("selected");
			jQuery(document.body).removeClass(name + "Selected");
		}
	};

	return w.n2ctx;
};
window.n2 = initn2context(window);

window.n2.frameManager = {
    init: function() {
        var self = this;
        self.repaint();
        $("#splitter").splitter({
            type: 'v',
            cookie: 'n2spl',
            anchorToWindow: true,
            onStart: function() {
                this.parent().addClass("activeSplitter");
            },
            onStop: function() {
                this.parent().removeClass("activeSplitter");
                self.repaint();
            },
            sizeLeft: true
        });
        $(window).bind("resize", function() {
            self.repaint();
        });
        setTimeout(function() { self.repaint.call(self); }, 100); // chrome hack
    },
    repaint: function() {
        var h = $(document).height() - $('#top').height();
        jQuery("#splitter,.pane").height(h);
    },
    contentHeight: function() {
        return ;
    }
};