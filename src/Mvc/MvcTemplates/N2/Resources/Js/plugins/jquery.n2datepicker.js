(function ($) {
    $.fn.n2datepicker = function (options) {
        options = $.extend({ showOn: 'button', changeYear: true, buttonText: "Hello" }, options);
        return this.datepicker(options);
    }
})(jQuery);