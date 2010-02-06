(function($) {
	$.fn.n2expandable = function(args) {
		var $c = this.children();
		if (args.visible)
			$c = $c.not(args.visible);

		if ($c.length == 0)
			return;

		var text = "Details";
		if (text = this.attr("title"))
			this.attr("title", "");
			
		var $l = (args.expander)
			? $(args.expander)
			: $("<a href='#' class='expander'>" + text + "</a>");

		$l.appendTo(this);

		var self = this;
		$l.click(function(e) {
			if (self.is(".expanded")) {
				$c.hide();
				self.removeClass("expanded");
			} else {
				$c.fadeIn();
				self.addClass("expanded");
			}
			e.preventDefault();
		});

		$c.hide();
	};
})(jQuery);