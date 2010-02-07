(function($) {
	$.fn.n2expandable = function(args) {
		var $children = this.children();
		if (args.visible)
			$children = $children.not(args.visible);

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
		$expander.click(function(e) {
			if (self.is(".expandable-expanded")) {
				$children.hide();
				self.removeClass("expandable-expanded");
				self.addClass("expandable-contracted");
			} else {
				$children.fadeIn();
				self.addClass("expandable-expanded");
				self.removeClass("expandable-contracted");
			}
			e.preventDefault();
			e.stopPropagation();
		});
		this.click(function(e) {
			if (!self.is(".expandable-expanded")) {
				$children.fadeIn();
				self.addClass("expandable-expanded");
				self.removeClass("expandable-contracted");
			}
		});

		$children.hide();
		this.addClass("expandable-contracted");
	};
})(jQuery);