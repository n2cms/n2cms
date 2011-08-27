(function ($) {
	$.fn.n2revealer = function () {
		this.each(function () {
			$("<a href='javascript:void(0);' class='revealer'/>").html(this.innerHTML)
    			.insertBefore(this)
    			.click(function () {
    				$(this).hide()
    				.siblings().show()
    				.end().closest(".editDetail").addClass("crowded");
    			}).siblings().hide();
		});
	};
})(jQuery);
