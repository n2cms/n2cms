/// <reference path="jquery-1.5.js" />

// common helper

$.fn.plupdown = function (options) {
	options = $.extend({
		callback: function () { },
		loadingHtml: "<ul><li class='loading'>Loading...</li></ul>",
		opener: true,
		openerClass: "opener",
		resultsClass: "dropmenu"
	}, options);
	var isForm = this.is("form");

	var close = function (e) {
		if ($(e.target).closest(".inner").any())
			return;
		$(".dropmenu").each(function () {
			$($(this).data("opener")).removeClass("open");
		}).removeData("opener").remove();
		$(document).unbind("click", close);
		$(document).unbind("submit", close);
	};

	var open = function (e) {
		e.preventDefault();
		if ($(this).is(".open"))
			return;
		e.stopPropagation();
		close(e);
		$(this).addClass("open");

		var o = $(this).offset();
		var h = $(this).height();

		var url = isForm ? this.action : this.href;
		var data = isForm ? $(this).serialize() : {};
		var $r = $("<div/>").addClass(options.resultsClass).appendTo(document.body)
			.css({
				position: "absolute",
				top: (o.top + h) + "px",
				left: Math.min(o.left, $(window).width() - 220) + "px"
			})
			.html("<a href='#close' class='closer'>&nbsp;</a><div class='inner'/>")
			.data("opener", this);
		$r.children(".inner")
			.html(options.loadingHtml)
			.load(url, data, options.callback);

		$(document.body).bind("click", close);
		$(document.body).bind("submit", close);
	};

	var $o = this.bind(isForm ? "submit" : "click", open);
	if(options.opener)
		$o.addClass(options.openerClass).append("<span class='arrow'>&nbsp;</span>");

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

$.fn.menudown = function (options) {
	options = $.extend({ submenuclass: "submenu" }, options);
	var top = this;
	var open = function () {
		$(top).find(".opened").children(".closer").trigger("click");
		$(this).siblings("a").addClass("expanded");
		$(this.parentNode).addClass("opened");
		$(this).siblings("ul").clone().addClass(options.submenuclass).insertAfter(top).each(function () { $(top).data("submenu", this) });
	};
	var close = function () {
		$(top.data("submenu")).remove();
		$(this.parentNode).removeClass("opened");
		$(this).siblings("a").removeClass("expanded");
	};
	$(this).addClass("menudown");
	$(this).find("ul").each(function () {
		$("<a href='#submenu' class='opener toggler'>&nbsp;</a>").prependTo(this.parentNode).click(open);
		$("<a href='#submenu' class='closer toggler'>&nbsp;</a>").prependTo(this.parentNode).click(close);
		$(this.parentNode).addClass("openable");
	});
	$(this).find(".current,.trail").siblings(".opener").trigger("click");
}

$(document).ready(function () {

	// Search

	function highlight(text) {
		if (!window.highlightLoaded)
			$("<script type='text/javascript' />").attr("src", "/Dinamico/Themes/Default/Scripts/jquery.highlight-3.js").appendTo($("head"));
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
		},
		opener: false
	});

	if (location.hash.match("^#q=")) {
		var text = location.hash.substr(3).replace(/[+]/g, " ");

		$("#searchform input").attr("value", text);
		$("#searchform").submit();

		highlight(text);
	}



	// Sitemap

	$("#sitemapopener").plupdown({
		resultsClass: "dropmenu sitemapresults",
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