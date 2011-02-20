/// <reference path="jquery-1.5.js" />

// common helper

$.fn.plupdown = function (options) {
	options = $.extend({
		callback: null,
		loadingHtml: "<ul><li class='loading'>Loading...</li></ul>",
		resultsClass: "results"
	}, options);
	var close = null;
	var isForm = this.is("form");

	var f = function (e) {
		e.preventDefault();
		e.stopPropagation();

		var o = $(this).offset();
		var h = $(this).height();

		var url = isForm ? this.action : this.href;
		var data = isForm ? $(this).serialize() : {};
		$("<div class='" + options.resultsClass + "'></div>").appendTo(document.body)
			.css({
				position: "absolute",
				top: (o.top + h) + "px",
				left: o.left + "px"
			})
			.html(options.loadingHtml)
			.load(url, data, options.callback);

		if (close)
			return;
		else
			close = function (e) {
				if ($(e.target).closest(".results").any())
					return;
				$(".results").remove();
				$(document).unbind("click", close);
			};
		$(document.body).click(close);
	};

	if (isForm)
		this.submit(f);
	else
		this.click(f);

	return this;
};

$.fn.any = function () {
	return this.length > 0;
}

$.fn.clearonfocus = function () {
	this.each(function () { this.title = this.value; })
		.focus(function () { if (this.value === this.title) this.value = ""; })
		.blur(function () { if (this.value === "") this.value = this.title; });
};

$(document).ready(function () {

	// Search

	function highlight(text) {
		if (!window.highlightLoaded)
			$("<script type='text/javascript' />").attr("src", "/Scripts/jquery.highlight-3.js").appendTo($("head"));
		window.highlightLoaded = true;
		setTimeout(function () {
			var splits = text.split(" ");
			for (var i in splits)
				$("#main *").highlight(splits[i]);
		}, 1);
	}

	$("#searchform input").clearonfocus();

	$("#searchform").plupdown({
		callback: function () {
			var value = $("#searchform input").attr("value");
			var here = location.href.replace(/#.*/, "");
			$("a", this).each(function (i) {
				if (i === 0) this.focus();
				if (this.href === here)
					$(this).focus().click(function () { $("span.highlight").removeClass("highlight"); highlight(value); });
				this.href += "#q=" + value;
			});
		}
	});

	if (location.hash.match("^#q=")) {
		var text = location.hash.substr(3).replace(/[+]/g, " ");

		$("#searchform input").attr("value", text);
		$("#searchform").submit();

		highlight(text);
	}



	// Sitemap

	$("#sitemapopener").plupdown({
		resultsClass: "results sitemapresults",
		callback: function () {
			var here = location.href.replace(/#.*/, "");
			$("a", this).each(function (i) {
				if (i === 0) this.focus();
				if (this.href === here) this.focus();
				this.href += "#sm";
			});
		}
	});

	if (location.hash === "#sm")
		$("#sitemapopener").click();



	// Translations

	$("#translationsopener").plupdown();
});