
/*
 * n2contextmenu 0.3 - Copyright (c) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * Usage example:
 *
 *	<script type="text/javascript" src="jquery-1.2.1.pack.js"></script>
 *	<script type="text/javascript" src="n2contextmenu.js"></script>
 * 	<script type="text/javascript">
 *		$(document).ready(function(){
 *          // wires the menu with id="contextmenu" to elements with class="clickable"
 *			$(".clickable").n2contextmenu("#contextmenu");
 *		});
 *	</script>
 */

(function($) {
	var menus = [];
	
	var hideAll = function(){
		$.each(menus, function(){
			this.n2hide();
		})
	}
	
	var show = function(e, $m, options) {
		options.showing.call($m[0], e.target);
		
		var offsetX = $(window).width() - e.clientX - $m.width();
		var offsetY = $(window).height() - e.clientY - $m.height();
		
		if(offsetX > 0) offsetX = 0;
		if(offsetY > 0) offsetY = 0;

		var x = e.pageX + options.offsetX + offsetX;
		var y = e.pageY + options.offsetY + offsetY;

		$m.n2showat(x, y);
	};
	
	var findFrame = function(name){
		for(var i = 0; i < window.frames.length; i++){
			if(window.frames[i].name == name){
				return window.frames[i];
			}
		}
		for(var i = 0; i < window.top.frames.length; i++){
			if(window.top.frames[i].name == name){
				return window.top.frames[i];
			}
		}
		return null;
	};

	$.fn.n2contextmenu = function(menu,options) {
		options = $.extend({offsetX: -10, offsetY: -10, showing: function(){}, appendTo: document.body}, options || {});

		var $m = $(menu)
			.appendTo(options.appendTo)
			.n2hide()
			.n2rightToLeftClick();
		this.bind('contextmenu', function(e) {
			if(!e.ctrlKey) {
				hideAll();
				show(e, $m, options);
				return false;
			}
		});
		if(menus.length == 0) {
			$(document.body).click(hideAll).bind('contextmenu', hideAll);
		}
		menus.push($m);
		return this;
	}
	
	$.fn.n2showat = function(l,t) {
		return this.css({left: l + "px", top: t + "px"});
	}

	$.fn.n2hide = function() {
		return this.css({position:"absolute", left: "-9999px", top: "-9999px"});
	}

	$.fn.n2rightToLeftClick = function() {
		return this.bind('contextmenu', function(e) {
			$(e.target).click().each(function() {
				var $t = $(this);
				$t.parents().andSelf().filter("a").each(function(){
					var f = findFrame(this.target) || window;
					f.location = this.href;
					hideAll();
				});
			});
			e.stopPropagation();
			e.preventDefault();
		});
	}
})(jQuery);
