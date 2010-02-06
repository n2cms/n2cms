(function($) {
	$.fn.n2expandable = function(args) {
		var $c = this.children();
		if (args.visible)
			$c = $c.not(args.visible);

		if ($c.length == 0)
			return;
		
		var $l = (args.expander) 
			? $(args.expander) 
			: $("<a href='#' class='expander'><span>Show/Hide</span></a>");
		if ($c[0].tagName == "LEGEND")
			$l.prependTo($c[0]);
		else
			$l.prependTo(this).wrap("<legend/>");

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