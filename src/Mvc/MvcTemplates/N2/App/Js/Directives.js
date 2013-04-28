(function (module) {
	console.log("directives.js");

	module.directive("pageActionContent", function ($interpolate) {
		return {
			restrict: "E",
			replace: true,
			scope: true,
			template: "<div x-compile='pageActionNode.Template'>\
	<a ng-class=\"{ 'page-action-icon': pageActionNode.IconUrl, 'page-action-description': pageActionNode.Description }\" \
		href='{{evaluateExpression(pageActionNode.Url)}}' \
		target='{{evaluateExpression(pageActionNode.Target)}}'\
		x-background-image='pageActionNode.IconUrl' \
		title='{{evaluateExpression(pageActionNode.ToolTip)}}'>\
		<i ng-show='pageActionNode.IconClass' class='{{pageActionNode.IconClass}}'></i>\
		{{evaluateExpression(pageActionNode.Title)}}\
		<span ng-show='pageActionNode.Description'>{{evaluateExpression(pageActionNode.Description)}}</span>\
	</a>\
</div>",
			link: function compile(scope, element, attrs) {
				scope.$watch(attrs.node, function (node) {
					scope.pageActionNode = node;
				});
				scope.evaluateExpression = function (expr) {
					return expr && $interpolate(expr)(scope);
				};
			}
		}
	});

	module.directive("backgroundImage", function () {
		return {
			restrict: "A",
			link: function compile(scope, element, attrs) {
				scope.$watch(attrs.backgroundImage, function (backgroundImage) {
					if (backgroundImage) {
						var style = element.attr("style");
						if (style)
							style += ";";
						else
							style = "";
						style += backgroundImage;
						element.attr("style", "background-image:url(" + backgroundImage + ")");
					}
				});
			}
		}
	});

	module.directive('compile', function ($compile) {
		return function (scope, element, attrs) {
			scope.$watch(
				function (scope) {
					// watch the 'compile' expression for changes
					return scope.$eval(attrs.compile);
				},
				function (value) {
					if (value === null)
						return;

					// when the 'compile' expression changes assign it into the current DOM
					element.html(value);

					// compile the new DOM and link it to the current scope.
					// NOTE: we only compile .childNodes so that we don't get into infinite loop compiling ourselves
					$compile(element.contents())(scope);
				}
			);
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