/**
 * n2glow 0.1
 */

(function($) {
    var left = 0;
    var width = 80;

    var over = function() {
        var $t = $(this);
        left = $t.position().left;
        width = $t.width();
        $t.css({ backgroundPosition: "50% 0px" });
    };

    var out = function(e) {
        $(this).css({ backgroundPosition: "50% 100px" });
    }

    var move = function(e) {
        var percent = (width - e.clientX + left) / width * 100;
        $(this).css({ backgroundPosition: percent + "% 0px" });
    };

    $.fn.n2glow = function() {
        return this.hover(over, out).mousemove(move);
    };
})(jQuery);
