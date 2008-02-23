/*
 * n2contextmenu 0.2 - Copyright (c) 2007 Cristian Libardo
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

