(function($) {
    $.fn.n2expandable = function(args) {
        var $children = this.children();
        if (args.visible) {
            $children = $children.not(args.visible);
            this.visible = args.visible;
        }

        if ($children.length == 0)
            return;

        var text = "Details";
        if (text = this.attr("title"))
            this.attr("title", "");

        var $expander = (args.expander)
			? $(args.expander)
			: $("<a href='#' class='expander'>" + text + "</a>");

        $expander.prependTo(this);

        var self = this;

        this.find('.expander').click(function(e) {
            if ($(this).parent().is('.expandable-expanded')) {
                if (self.visible) {
                    $(this).parent().children().not('.expander').not(self.visible).hide();
                }
                else {
                    $(this).parent().children().not('.expander').hide();
                }
                $(this).parent().removeClass('expandable-expanded').addClass('expandable-contracted');
            }
            else {
                if (self.visible) {
                    $(this).parent().children().not('.expander').not(self.visible).fadeIn();
                }
                else {
                    $(this).parent().children().not('.expander').fadeIn();
                }
                $(this).parent().removeClass('expandable-contracted').addClass('expandable-expanded');
            }
            e.preventDefault();
            e.stopPropagation();
        });
        this.click(function(e) {
            if (!$(this).is(".expandable-expanded")) {
                if (self.visible) {
                    $(this).children().not('.expander').not(self.visible).fadeIn();
                }
                else {
                    $(this).children().not('.expander').fadeIn();
                }
                $(this).removeClass('expandable-contracted').addClass('expandable-expanded');
            }
        });

        $children.hide();
        this.addClass("expandable-contracted");
    };
})(jQuery);;
