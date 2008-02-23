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
