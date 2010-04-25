/**
* editableThemeSelection
*/

(function($) {
	$.fn.editableThemeSelection = function(thumbnails) {
		var $img = $("<img style='display:none' />")
		.insertAfter(this)
		.wrap("<span class='themeThumbnail'/>");


		$(this).hover(function() {
			var thumbnail = thumbnails[this.value];
			if (thumbnail) {
				$img.attr("src", thumbnail).show();
			} else {
				$img.hide();
			}
		}, function() {
			$img.fadeOut();
		}).change(function() {
			var thumbnail = thumbnails[this.value];
			if (thumbnail) {
				$img.attr("src", thumbnail)
					.show().fadeOut(2000);
			}
		});
	};
})(jQuery);
