/// <reference path="jquery-1.5.js" />

// common helper

$.fn.plupdown = function (options) {
	options = $.extend({
		callback: function () { }
		//,
		//opener: true,
		//openerClass: "opener",
		//resultsClass: "dropmenu"
	}, options);
	var isForm = this.is("form");

	//var close = function (e) {
	//	if ($(e.target).closest(".inner").any())
	//		return;
	//	$(".dropmenu").each(function () {
	//		$($(this).data("opener")).removeClass("open");
	//	}).removeData("opener").remove();
	//	$(document).unbind("click", close);
	//	$(document).unbind("submit", close);
	//};
	var open = function (e) {
		e && e.preventDefault();
		if (isForm) {
			$(this).closest("li").addClass("open").end().siblings("ul").children("li").show();
			var form = this;
			function close() {
				$(form).closest("li").removeClass("open");
				$(document).unbind(".plupdown_closer");
			}
			$(document).bind("click.plupdown_closer", function (e) {
				close();
			}).bind("keydown.plupdown_closer", function (e) {
				if (e.keyCode != 27)
					return;
				close();
			});
		}
		else
			$(this).unbind("click.plupdown");
		//if ($(this).is(".open"))
		//	return;
		//e.stopPropagation();
		//close(e);
		//$(this).addClass("open");

		//var o = $(this).offset();
		//var h = $(this).height();

		var url = isForm ? this.action : this.href;
		var data = isForm ? $(this).serialize() : {};
		
		//$("<div/>").addClass(options.resultsClass).appendTo(document.body)
			//.css({
			//	position: "absolute",
			//	top: (o.top + h) + "px",
			//	left: Math.min(o.left, $(window).width() - 220) + "px"
			//})
			//.html("<a href='#close' class='closer'>&nbsp;</a><div class='inner'/>")
			//.data("opener", this);
		var $ul = $(this).siblings("ul");
		if (options.loadingHtml)
			$ul.html(options.loadingHtml);
		$ul.load(url, data, options.callback);
	};

	//var $o = this.bind(isForm ? "submit" : "click", open);
	//if(options.opener)
	//	$o.addClass(options.openerClass).append("<span class='arrow'>&nbsp;</span>");

	if (isForm)
		$(this).bind("submit.plupdown", open);
	else
		$(this).bind("click.plupdown", open);

	return this;
};

$.fn.any = function () {
	return this.length > 0;
}

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
			$("<script type='text/javascript' />").attr("src", "/Dinamico/Themes/Default/lib/jquery/jquery.highlight-3.js").appendTo($("head"));
		window.highlightLoaded = true;
		setTimeout(function () {
			var splits = text.split(" ");
			for (var i in splits)
				$("#main *").highlight(splits[i]);
		}, 1);
	}

	$("#searchform").plupdown({
		callback: function () {
			var value = $("#searchform input").val();
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