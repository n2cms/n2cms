/*
 * jquery.splitter.js - two-pane splitter window plugin
 *
 * version 1.01 (01/05/2007) 
 *
 * Dual licensed under the MIT and GPL licenses: 
 *   http://www.opensource.org/licenses/mit-license.php 
 *   http://www.gnu.org/licenses/gpl.html 
 *
 * @author Dave Methvin (dave.methvin@gmail.com)
 */
 jQuery.fn.splitter = function(opts){
	opts = jQuery.extend({
		type: 'v',				// v=vertical, h=horizontal split
		activeClass: 'active',	// class name for active splitter
		pxPerKey: 5,			// splitter px moved per keypress
		tabIndex: 0,			// tab order indicator
		accessKey: ''			// accelerator key for splitter
//		initA  initB			// initial A/B size (pick ONE)
//		minA maxA  minB maxB	// min/max pane sizes
	},{
		v: {					// Vertical splitters:
			keyGrowA: 39,		//	left arrow key
			keyShrinkA: 37,		//	right arrow key
			cursor: "e-resize",	//	double-arrow horizontal
			splitbarClass: "vsplitbar",
			eventPos: "pageX", set: "left", 
			adjust: "width",  offsetAdjust: "offsetWidth",  adjSide1: "Left", adjSide2: "Right",
			fixed:  "height", offsetFixed:  "offsetHeight", fixSide1: "Top",  fixSide2: "Bottom"
		},
		h: {					// Horizontal splitters:
			keyGrowA: 40,		//	down arrow key
			keyShrinkA: 38,		//	up arrow key
			cursor: "n-resize",	//	double-arrow vertical
			splitbarClass: "hsplitbar",
			eventPos: "pageY", set: "top", 
			adjust: "height", offsetAdjust: "offsetHeight", adjSide1: "Top",  adjSide2: "Bottom",
			fixed:  "width",  offsetFixed:  "offsetWidth",  fixSide1: "Left", fixSide2: "Right"
		}
	}[((opts||{}).type||'v').charAt(0).toLowerCase()], opts||{});

	return this.each(function() {
		function startSplit(e) {
			$("iframe").hide();
			splitbar.addClass(opts.activeClass);
			if ( e.type == "mousedown" ) {
				paneA._posAdjust = paneA[0][opts.offsetAdjust] - e[opts.eventPos];
				jQuery(document)
					.bind("mousemove", doSplitMouse)
					.bind("mouseup", endSplit);
			}
			return true;	// required???
		}
		function doSplitKey(e) {
			var key = e.which || e.keyCode;
			var dir = key==opts.keyGrowA? 1 : key==opts.keyShrinkA? -1 : 0;
			if ( dir )
				moveSplitter(paneA[0][opts.offsetAdjust]+dir*opts.pxPerKey);
			return true;	// required???
		}
		function doSplitMouse(e) {
			moveSplitter(paneA._posAdjust+e[opts.eventPos]);
		}
		function endSplit(e) {
			$("iframe").show();
			splitbar.removeClass(opts.activeClass);
			jQuery(document)
				.unbind("mousemove", doSplitMouse)
				.unbind("mouseup", endSplit);
		}
		function moveSplitter(np) {
			// Constrain new position to fit pane size limits; 16=scrollbar fudge factor
			// TODO: enforce group width in IE6 since it lacks min/max css properties?
			np = Math.max(paneA._min+paneA._padAdjust, group._adjust - (paneB._max||9999), 16,
				Math.min(np, paneA._max||9999, group._adjust - splitbar._adjust - 
					Math.max(paneB._min+paneB._padAdjust, 16)));

			// Resize/position the two panes and splitbar
			splitbar.css(opts.set, np+"px");
			paneA.css(opts.adjust, np-paneA._padAdjust+"px");
			paneB.css(opts.set, np+splitbar._adjust+"px")
				.css(opts.adjust, group._adjust-splitbar._adjust-paneB._padAdjust-np+"px");

			// IE fires resize for us; all others pay cash
			if ( !jQuery.browser.msie ) {
				paneA.trigger("resize");
				paneB.trigger("resize");
			}
		}
		function cssCache(jq, n, pf, m1, m2) {
			// IE backCompat mode thinks width/height includes border and padding
			jq[n] = jQuery.boxModel? (parseInt(jq.css(pf+m1))||0) + (parseInt(jq.css(pf+m2))||0) : 0;
		}
		function optCache(jq, pane) {
			// Opera returns -1px for min/max dimensions when they're not there!
			jq._min = Math.max(0, opts["min"+pane] || parseInt(jq.css("min-"+opts.adjust)) || 0);
			jq._max = Math.max(0, opts["max"+pane] || parseInt(jq.css("max-"+opts.adjust)) || 0);
		}

		// Create jQuery object closures for splitter group and both panes
		var group = jQuery(this).css({position: "relative"});
		var divs = jQuery(">div", group).css({
			position: "absolute", 			// positioned inside splitter container
			margin: "0", 					// remove any stylesheet margin or ...
			border: "0", 					// ... border added for non-script situations
			"-moz-user-focus": "ignore"		// disable focusability in Firefox
		});
		var paneA = jQuery(divs[0]);		// left  or top
		var paneB = jQuery(divs[1]);		// right or bottom

		// Focuser element, provides keyboard support
		var focuser = jQuery('<a href="javascript:void(0)"></a>')
			.bind("focus", startSplit).bind("keydown", doSplitKey).bind("blur", endSplit)
			.attr({accessKey: opts.accessKey, tabIndex: opts.tabIndex});

		// Splitbar element, displays actual splitter bar
		// The select-related properties prevent unintended text highlighting
		var splitbar = jQuery('<div></div>')
			.insertAfter(paneA).append(focuser)
			.attr({"class": opts.splitbarClass, unselectable: "on"})
			.css({position: "absolute", "-khtml-user-select": "none",
				"-moz-user-select": "none", "user-select": "none"})
			.bind("mousedown", startSplit);
		if ( /^(auto|default)$/.test(splitbar.css("cursor") || "auto") )
			splitbar.css("cursor", opts.cursor);

		// Cache several dimensions for speed--assume these don't change
		splitbar._adjust = splitbar[0][opts.offsetAdjust];
		cssCache(group, "_borderAdjust", "border", opts.adjSide1+"Width", opts.adjSide2+"Width");
		cssCache(group, "_borderFixed",  "border", opts.fixSide1+"Width", opts.fixSide2+"Width");
		cssCache(paneA, "_padAdjust", "padding", opts.adjSide1, opts.adjSide2);
		cssCache(paneA, "_padFixed",  "padding", opts.fixSide1, opts.fixSide2);
		cssCache(paneB, "_padAdjust", "padding", opts.adjSide1, opts.adjSide2);
		cssCache(paneB, "_padFixed",  "padding", opts.fixSide1, opts.fixSide2);
		optCache(paneA, 'A');
		optCache(paneB, 'B');

		// Initial splitbar position as measured from left edge of splitter
		paneA._init = (opts.initA==true? parseInt(jQuery.curCSS(paneA[0],opts.adjust)) : opts.initA) || 0;
		paneB._init = (opts.initB==true? parseInt(jQuery.curCSS(paneB[0],opts.adjust)) : opts.initB) || 0;
		if ( paneB._init )
			paneB._init = group[0][opts.offsetAdjust] - group._borderAdjust - paneB._init - splitbar._adjust;

		// Set up resize event handler and trigger immediately to set initial position
		group.bind("resize", function(e,size){
			// Determine new width/height of splitter container
			group._fixed  = group[0][opts.offsetFixed]  - group._borderFixed;
			group._adjust = group[0][opts.offsetAdjust] - group._borderAdjust;
			// Bail if splitter isn't visible or content isn't there yet
			if ( group._fixed <= 0 || group._adjust <= 0 ) return;
			// Set the fixed dimension (e.g., height on a vertical splitter)
			paneA.css(opts.fixed, group._fixed-paneA._padFixed+"px");
			paneB.css(opts.fixed, group._fixed-paneB._padFixed+"px");
			splitbar.css(opts.fixed, group._fixed+"px");
			// Re-divvy the adjustable dimension; maintain size of the preferred pane
			moveSplitter(size || (!opts.initB? paneA[0][opts.offsetAdjust] :
				group._adjust-paneB[0][opts.offsetAdjust]-splitbar._adjust));
		}).trigger("resize" , [paneA._init || paneB._init || 
			Math.round((group[0][opts.offsetAdjust] - group._borderAdjust - splitbar._adjust)/2)]);
	});
};










/*
 * Treeview 1.2? - jQuery plugin to hide and show branches of a tree
 *
 * Copyright (c) 2006 Jörn Zaefferer, Myles Angell
 *
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 *
 */

(function($) {

	// classes used by the plugin
	// need to be styled via external stylesheet, see first example
	var CLASSES = {
		open: "open",
		closed: "closed",
		expandable: "expandable",
		collapsable: "collapsable",
		lastCollapsable: "lastCollapsable",
		lastExpandable: "lastExpandable",
		last: "last",
		hitarea: "hitarea"
	};
	
	// styles for hitareas
	var hitareaCSS = {
		height: 15,
		width: 15,
		marginLeft: "-15px",
		"float": "left",
		cursor: "pointer"
	};
	
	// ie specific styles for hitareas
	if( $.browser.msie )
		$.extend( hitareaCSS, {
			background: "#fff",
			filter: "alpha(opacity=0)",
			display: "inline"
		});

	$.extend($.fn, {
		swapClass: function(c1, c2) {
			return this.each(function() {
				var $this = $(this);
				if ( $.className.has(this, c1) )
					$this.removeClass(c1).addClass(c2);
				else if ( $.className.has(this, c2) )
					$this.removeClass(c2).addClass(c1);
			});
		},
		replaceClass: function(c1, c2) {
			return this.each(function() {
				var $this = $(this);
				if ( $.className.has(this, c1) )
					$this.removeClass(c1).addClass(c2);
			});
		},
		hoverClass: function(className) {
			className = className || "hover";
			return this.hover(function() {
				$(this).addClass(className);
			}, function() {
				$(this).removeClass(className);
			});
		},
		heightToggle: function(animated, callback) {
			animated ?
				this.animate({ height: "toggle" }, animated, callback) :
				this.each(function(){
					jQuery(this)[ jQuery(this).is(":hidden") ? "show" : "hide" ]();
					if(callback)
						callback.apply(this, arguments);
				});
		},
		heightHide: function(animated, callback) {
			if (animated) {
				this.animate({ height: "hide" }, animated, callback)
			} else {
				this.hide();
				if (callback)
					this.each(callback);				
			}
		},
		prepareBranches: function(settings) {
			// mark last tree items
			this.filter(":last-child").addClass(CLASSES.last);
			// collapse whole tree, or only those marked as closed, anyway except those marked as open
			this.filter((settings.collapsed ? "" : "." + CLASSES.closed) + ":not(." + CLASSES.open + ")").find(">ul").hide();
			// return all items with sublists
			return this.filter(":has(>ul)");
		},
		applyClasses: function(settings, toggler) {
			this.filter(":has(ul):not(:has(>a))").find(">span").click(function(event) {
				if ( this == event.target ) {
					toggler.apply($(this).next());
				}
			}).add( $("a", this) ).hoverClass()
			
			// handle closed ones first
			this.filter(":has(>ul:hidden)")
					.addClass(CLASSES.expandable)
					.replaceClass(CLASSES.last, CLASSES.lastExpandable);
					
			// handle open ones
			this.not(":has(>ul:hidden)")
					.addClass(CLASSES.collapsable)
					.replaceClass(CLASSES.last, CLASSES.lastCollapsable);
					
			// append hitarea
			this.prepend("<div class=\"" + CLASSES.hitarea + "\">")
				// find hitarea
				.find("div." + CLASSES.hitarea)
				// apply styles to hitarea
				.css(hitareaCSS)
				// apply click event to hitarea
				.click( toggler );
		},
		treeview: function(settings) {
			
			// currently no defaults necessary, all implicit
			settings = $.extend({}, settings);
			
			if (settings.add) {
				return this.trigger("add", [settings.add]);
			}
			
			if (settings.toggle ) {
				var callback = settings.toggle;
				settings.toggle = function() {
					return callback.apply($(this).parent()[0], arguments);
				}
			}
		
			// factory for treecontroller
			function treeController(tree, control) {
				// factory for click handlers
				function handler(filter) {
					return function() {
						// reuse toggle event handler, applying the elements to toggle
						// start searching for all hitareas
						toggler.apply( $("div." + CLASSES.hitarea, tree).filter(function() {
							// for plain toggle, no filter is provided, otherwise we need to check the parent element
							return filter ? $(this).parent("." + filter).length : true;
						}) );
						return false;
					}
				}
				// click on first element to collapse tree
				$(":eq(0)", control).click( handler(CLASSES.collapsable) );
				// click on second to expand tree
				$(":eq(1)", control).click( handler(CLASSES.expandable) );
				// click on third to toggle tree
				$(":eq(2)", control).click( handler() ); 
			}
		
			// handle toggle event
			function toggler() {
				// this refers to hitareas, we need to find the parent lis first
				$(this).parent()
					// swap classes
					.swapClass( CLASSES.collapsable, CLASSES.expandable )
					.swapClass( CLASSES.lastCollapsable, CLASSES.lastExpandable )
					// find child lists
					.find( ">ul" )
					// toggle them
					.heightToggle( settings.animated, settings.toggle );
				if ( settings.unique ) {
					$(this).parent()
						.siblings()
						.replaceClass( CLASSES.collapsable, CLASSES.expandable )
						.replaceClass( CLASSES.lastCollapsable, CLASSES.lastExpandable )
						.find( ">ul" )
						.heightHide( settings.animated, settings.toggle );
				}
			}
			
			function serialize() {
				function binary(arg) {
					return arg ? 1 : 0;
				}
				var data = [];
				branches.each(function(i, e) {
					data[i] = $(e).is(":has(>ul:visible)") ? 1 : 0;
				});
				$.cookie("treeview", data.join("") );
			}
			
			function deserialize() {
				var stored = $.cookie("treeview");
				if ( stored ) {
					var data = stored.split("");
					branches.each(function(i, e) {
						$(e).find(">ul")[ parseInt(data[i]) ? "show" : "hide" ]();
					});
				}
			}
			
			// add treeview class to activate styles
			this.addClass("treeview");
			
			// prepare branches and find all tree items with child lists
			var branches = this.find("li").prepareBranches(settings);
			
			switch(settings.persist) {
			case "cookie":
				var toggleCallback = settings.toggle;
				settings.toggle = function() {
					serialize();
					if (toggleCallback) {
						toggleCallback.apply(this, arguments);
					}
				} 
				deserialize();
				break;
			case "location":
				var current = this.find("a").filter(function() { return this.href == location.href; });
				if ( current.length ) {
					current.addClass("selected").parents("ul, li").add( current.next() ).show();
				}
				break;
			}
			
			branches.applyClasses(settings, toggler);
				
			// if control option is set, create the treecontroller
			if ( settings.control )
				treeController(this, settings.control);
			
			return this.bind("add", function(event, branches) {
				$(branches).prev().removeClass(CLASSES.last).removeClass(CLASSES.lastCollapsable).removeClass(CLASSES.lastExpandable);
				$(branches).find("li").andSelf().prepareBranches(settings).applyClasses(settings, toggler);
			});
		}
	});
})(jQuery);






/*
 * n2contextmenu 0.2 - Copyright (c) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 */

(function($) {
	var menus = [];
	
	var hideAll = function(){
		$.each(menus, function(){
			this.n2hide();
		})
	}
	
	var show = function(e,$m,options) {
		hideAll();
		$m.n2showat(e.pageX + options.offsetX, e.pageY + options.offsetY);
	};

	$.fn.n2contextmenu = function(menu,options){
		var settings = {offsetX: -10, offsetY: -10};
		$.extend(settings, options);

		var $m = $(menu).appendTo(document.body).n2hide();
		this.bind('contextmenu', function(e){
			if(!e.ctrlKey){
				show(e, $m, settings);
				return false;
			}
		});
		if(menus.length == 0){
			$(document.body).click(function(){	
				hideAll();
			});
		}
		menus.push($m);
		return this;
	}
	
	$.fn.n2showat = function(l,t){
		return this.css({left: l + "px", top: t + "px"});
	}

	$.fn.n2hide = function(){
		return this.css({position:"absolute", left: "-9999px", top: "-9999px"});
	}
})(jQuery);





/**
 * n2optionmenu 0.1
 */

(function($) {
	$.fn.n2optionmenu = function(options){
		settings = {
			wrapper: "<div class='commandOptions closed'></div>",
			opener: "<span class='opener'><img src='img/ico/bullet_arrow_down.gif' alt='more options'/></span>",
			closedClass: "closed"
		};
		$.extend(settings, options || {});
		
		var closable = false;
		var $menu = this;

		var $wrapper = $menu.wrap(settings.wrapper).parent();
		$menu.children().slice(0,1).clone(true).insertBefore($menu)
			.after(settings.opener).next().click(function(){
				closable = false;
				$wrapper.toggleClass(settings.closedClass);
				setTimeout(function(){closable = true;}, 10);
			});
		$(document.body).click(function(){
			if(closable)
				$wrapper.addClass(settings.closedClass);
		});
		return $menu;
	}
})(jQuery);








//http://www.quirksmode.org/js/cookies.html

function cookie(){}

cookie.create = function(name,value,days) {
    if (days) {
	    var date = new Date();
	    date.setTime(date.getTime()+(days*24*60*60*1000));
	    var expires = "; expires="+date.toGMTString();
    }
    else var expires = "";
    document.cookie = name+"="+value+expires+"; path=/";
}

cookie.read = function(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for(var i=0;i < ca.length;i++) {
	    var c = ca[i];
	    while (c.charAt(0)==' ') c = c.substring(1,c.length);
	    if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length,c.length);
    }
    return null;
}

cookie.erase = function(name) {
    cookie.create(name,"",-1);
}











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
		n2nav.onUrlSelected(n2nav.toRelativeUrl(el.href));
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
    n2nav.setupToolbar(a.href);
}
n2nav.targetHandlers["preview"] = function(a,i) {
    var relativeUrl = n2nav.toRelativeUrl(a.href);
    $(a).addClass("enabled").bind("click", null, n2nav.previewClickHandler);
}
n2nav.setupToolbar = function(href){
	if(window.top.n2)window.top.n2.setupToolbar(href.replace(/.*?:\/\/.*?\//, "/"));
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



