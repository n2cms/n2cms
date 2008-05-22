/*
 * n2contextmenu - v0.1
 */

(function($) {
	var menus = [];

	var hideAll = function(){
		$.each(menus, function(){
			this.hide();
		})
	}

	var showMenu = function(e,$m) {
		if(!e.ctrlKey){
			hideAll();
			$m.show().appendTo(document.body).css({
				position:"absolute", 
				left: e.pageX + "px", 
				top: e.pageY + "px"
			});;
		}
	};

	$.fn.n2contextmenu = function(menu){
		var $m = $(menu).hide();
		this.bind('contextmenu', function(e){
			showMenu(e ,$m);
			return false;
		});
		if(menus.length == 0){
			$(document.body).click(hideAll);
		}
		menus.push($m);
	}
})(jQuery);
