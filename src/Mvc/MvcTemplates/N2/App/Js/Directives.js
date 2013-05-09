(function (module) {
	console.log("directives.js");

	module.directive("pageActionContent", function ($interpolate) {
		return {
			restrict: "E",
			replace: true,
			scope: true,
			template: "<div x-compile='node.Current.Template'>\
	<a ng-class=\"{ 'page-action-icon': node.Current.IconUrl, 'page-action-description': node.Current.Description }\" \
		href='{{evaluateExpression(node.Current.Url)}}' \
		target='{{evaluateExpression(node.Current.Target)}}'\
		x-background-image='node.Current.IconUrl' \
		title='{{evaluateExpression(node.Current.ToolTip)}}'>\
		<i ng-show='node.Current.IconClass' class='{{node.Current.IconClass}}'></i>\
		{{evaluateExpression(node.Current.Title)}}\
		<span ng-show='node.Current.Description'>{{evaluateExpression(node.Current.Description)}}</span>\
	</a>\
</div>",
			link: function compile(scope, element, attrs) {
				scope.$watch(attrs.node, function (node) {
					scope.node = node;
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
						style += "background-image:url(" + backgroundImage + ")";
						element.attr("style", style);
					}
				});
			}
		}
	});

	module.directive("sortable", function (uiSortable) {
		console.log("sortable", uiSortable);

		return {
			restrict: "A",
			link: function compile(scope, element, attrs) {
				//ui-sortable="{ receive: sort.receive, remove: sort.remove, update: sort.update, start: sort.start, stop: sort.stop, connectWith: '.targettable', placeholder: 'sortable-placeholder', handle: '.item', distance: 10 }" 

				//element.sortable({
				//	connectWith: '.targettable',
				//	distance: 15,
				//	placeholder: 'drop-placeholder',
				//	appendTo: "#main-wrapper"
				//});
				//element.on("update", function (event, ui) {
				//	scope.$emit("update", {});
				//	console.log("update", ui);
				//});
				//element.on("start", function (event, ui) {
				//	console.log("start", ui);
				//});
			}
		}
	});

	//module.directive("sortStartStopClassify", function () {
	//	return {
	//		restrict: "A",
	//		link: function compile(scope, element, attrs) {
	//			console.log("sortStartStopClassify");
	//			element.on("start", function (event, ui) {
	//				console.log("sortStartStopClassify start");
	//				element.addClass("sorting");
	//			});
	//			element.on("stop", function (event, ui) {
	//				console.log("sortStartStopClassify stop");
	//				element.removeClass("sorting");
	//			});
	//		}
	//	}
	//});

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