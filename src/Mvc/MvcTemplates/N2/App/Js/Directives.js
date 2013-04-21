(function (module) {
	console.log("directives.js");

	module.directive("treenode", function () {
		return {
			restrict: "E",
			replace: true,
			scope: {
				model: "="
			},
			template: "<div class='item'>\
	<a href='{{model.Current.Url}}' target='{{model.Current.Target}}'>\
		<span class='icon-custom' style='background-image:url({{model.Current.IconUrl}}); background-position:0 0; width:16px; height:16px;'></span>\
		{{model.Current.Title}}</a>\
	<div class='toggle'>\
		<img src='redesign/img/minus.png' class='collapse' /><img src='redesign/img/plus.png' class='expand' />\
	</div>\
	<div class='tools'>\
		<a href='#'>tools</a>\
	</div>\
</div>",
			link: function compile(scope, element, attrs) {
			}
		};
	});

	module.filter('pretty', function () {
		function syntaxHighlight(json) {
			if (typeof json != 'string') {
				json = angular.toJson(json, true);
			}
			json = json.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
			return json.replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g, function (match) {
				var cls = 'number';
				if (/^"/.test(match)) {
					if (/:$/.test(match)) {
						cls = 'key';
					} else {
						cls = 'string';
					}
				} else if (/true|false/.test(match)) {
					cls = 'boolean';
				} else if (/null/.test(match)) {
					cls = 'null';
				}
				return '<span class="' + cls + '">' + match + '</span>';
			});
		}
		return function (obj) {
			return "<style>\
span.key {color:blue}\
span.number {color:green}\
span.string {color:red}\
span.boolean {color:green}\
span.null {color:silver}\
</style><pre>" + syntaxHighlight(obj) + "</pre>";
		};
	});

})(angular.module('n2.directives', []));