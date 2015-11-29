n2 = window.n2 || {};
$(function () {
	n2.boostrapper = {
		append: function (tag, props, attrs) {
			var el = document.createElement(tag);
			$.extend(el, props);
			attrs && $.each(attrs, function (name) { el.setAttribute(name, this); });
			document.body.appendChild(el);
			return el;
		},
		appendScripts: function (sources, callback) {
			var count = 0;
			$.each(sources, function () {
				n2.boostrapper.append("script", {
					src: this, async: false, onload: function () {
						if (++count == sources.length)
							callback && callback();
					}
				});
			});
		},
		init: function (div) {
			var result = angular.bootstrap(div, ["n2preview"]);		
		}
	};
	n2.boostrapper.append("link", { type: "text/css", rel: "stylesheet", href: "/N2/App/Preview/Preview.css" })
	var div = n2.boostrapper.append("div", null, { "class": "n2-preview n2-loading", "n2-preview": null })
	n2.boostrapper.appendScripts(['//cdnjs.cloudflare.com/ajax/libs/angular.js/1.2.20/angular.js', '//cdnjs.cloudflare.com/ajax/libs/angular.js/1.2.20/angular-resource.js', '/N2/App/i18n/en.js.ashx', '/N2/App/Js/Directives.js', '/N2/App/Js/Services.js', '/N2/App/Preview/Preview.js'], function () { n2.boostrapper.init(div) });
});
