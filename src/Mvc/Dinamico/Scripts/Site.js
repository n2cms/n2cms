/// <reference path="jquery-1.5.js" />

// Search

$(document).ready(function () {
	function close(e) {
		if ($(e.target).closest("#searchform").is("*"))
			return;
		$("#searchresults").remove();
		$(document).unbind("click", close);
	}
	$("#searchform input")
		.each(function () { this.title = this.value; })
		.focus(function () { if (this.value === this.title) this.value = ""; })
		.blur(function () { if (this.value === "") this.value = this.title; });
	$("#searchform").submit(function (e) {
		e.preventDefault();
		var data = $(this).serialize();
		var hash = "#" + data;
		$("<ul id='searchresults'><li class='loading'>Searching...</li></ul>").appendTo(this)
			.load(this.action, data, function () {
				var here = location.href.replace(/#.*/, "");
				$("a", this).each(function (i) {
					if (i === 0) this.focus();
					if (this.href === here) this.focus();
					this.href += hash;
				});
			});
		$(document).click(close);
	});
	if (location.hash.match("^#q=")) {
		var text = location.hash.substr(3).replace(/[+]/g, " ");

		$("#searchform input").attr("value", text);
		$("#searchform").submit();

		$("<script type='text/javascript' />").attr("src", "/Scripts/jquery.highlight-3.js").appendTo($("head"));
		setTimeout(function () {
			var splits = text.split(" ");
			for (var i in splits)
				$("#main *").highlight(splits[i]);
		}, 1);
	}
});

// Sitemap

$(document).ready(function () {
	var hash = "#sm";
	function close(e) {
		if ($(e.target).closest("#sitemap").is("*"))
			return;
		$("#sitemapresults").remove();
		$(document).unbind("click", close);
	}
	$("#sitemapopener").click(function (e) {
		e.preventDefault();
		$("<ul id='sitemapresults'><li class='loading'>Loading...</li></ul>").appendTo("#sitemap")
			.load(this.href, null, function () {
				var here = location.href.replace(/#.*/, "");
				$("a", this).each(function (i) {
					if (i === 0) this.focus();
					if (this.href === here) this.focus();
					this.href += hash;
				});
			});
		$(document).click(close);
	});
	if (location.hash === hash)
		$("#sitemapopener").click();
});

$(document).ready(function () {
	function close(e) {
		if ($(e.target).closest("#translations").is("*"))
			return;
		$("#translationsresults").remove();
		$(document).unbind("click", close);
	}
	$("#translationsopener").click(function (e) {
		e.preventDefault();
		$("<ul id='translationsresults'><li class='loading'>Loading...</li></ul>").appendTo("#translations")
			.load(this.href, null, function () {
				$("a[href='" + location.href + "']", this).focus();
			});
		$(document).click(close);
	});
});