(function ($) {
	$.fn.n2datepicker = function (options) {
		return this.datepicker($.extend({ showOn: 'button', buttonImageOnly:true, changeYear:true }, options));
	}
})(jQuery);