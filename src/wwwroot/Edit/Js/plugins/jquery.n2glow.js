/**
 * n2glow 0.1
 */

(function($) {
    var left = 0;
    var width = 80;
    var bgWidth = 80;

    var over = function() {
        var $t = $(this);
        left = $t.position().left;
        width = $t.width();
        $t.css({ backgroundPosition: "50% 0px" });
    };

    var out = function(e) {
        var pixels = .5 * width - bgWidth / 2;
        $(this).animate({ backgroundPosition: pixels + "px 50px" });
    }

    var move = function(e) {
        var pixels = (e.clientX - left) - bgWidth / 2;
        $(this).css({ backgroundPosition: pixels + "px 10px" });
    };

    $.fn.n2glow = function() {
        return this.hover(over, out).mousemove(move);
    };
})(jQuery);
