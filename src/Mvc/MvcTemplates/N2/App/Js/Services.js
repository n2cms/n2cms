(function (module) {
	console.log("services.js");

	module.factory('Interface', function ($resource) {
		var res = $resource('Api/Interface.ashx', {}, {});
		return res;
	});

	//module.factory('Children', function ($resource) {
	//	var res = $resource('Api/Children.ashx', {}, {});
	//	return res;
	//});

	module.factory('Content', function ($resource) {
		var res = $resource('Api/Content.ashx/:target', { target: '' }, {
			children: { method: 'GET', params: { target: 'children' } },
			move: { method: 'POST', params: { target: 'move' } },
			sort: { method: 'POST', params: { target: 'sort' } }
		});

		res.states = {
			None: 0,
			New: 1,
			Draft: 2,
			Waiting: 4,
			Published: 16,
			Unpublished: 32,
			Deleted: 64,
			All: 2 + 4 + 8 + 16 + 32 + 64,
			is: function (actual, expected) {
				return (actual & expected) == expected;
			}
		};
		
		return res;
	});

	module.factory('Translations', function ($resource) {
		var res = $resource('Api/Translations.ashx', {}, {});
		return res;
	});

	module.factory('Context', function ($resource) {
		var res = $resource('Api/Context.ashx', {}, {});
		return res;
	});

	module.factory('Versions', function ($resource) {
		var res = $resource('Api/Versions.ashx', {}, {});
		return res;
	});

	module.factory('SortHelperFactory', function (Content) {
		var context = {}
		return function (scope) {
			var isOrigin = false;
			this.start = function (e, args) {
				var $element = $(args.item.context).closest(".ng-scope");
				context.from = angular.element($element).scope().node;
				context.parent = scope.node;
				context.siblingCount = scope.node.Children.length;
				isOrigin = true;
			};
			this.stop = function (e, args) {
				setTimeout(function () {
					context = {};
					isOrigin = false;
				}, 11);
			};
			this.remove = function (e, args) {
			};
			this.receive = function (e, args) {
				var $element = $(args.item.context).closest(".ng-scope");
				var $next = $element.next(".ng-scope");

				var from = angular.element($element).scope().node;
				var to = scope.node;
				var before = $next.length ? angular.element($next).scope().node : null;

				console.log("moving ", from.Current.Title, " -> ", to.Current.Title, " (before ", before && before.Current && before.Current.Title, ")");
				Content.move({ selected: from.Current.Path, to: to.Current.Path, before: before && before.Current && before.Current.Path }, {}, function () {
					to.HasChildren = true;
					Content.query({ selected: to.Current.Path }, function (children) {
						to.Children = children;
						to.Expanded = true;
						scope.$emit("moved", from);
					});
				});

			};
			this.update = function (e, args) {
				if (!isOrigin || scope.node.Current.Path != context.parent.Current.Path)
					return;

				var $element = $(args.item.context).closest(".ng-scope");
				var $next = $element.next(".ng-scope");

				var from = angular.element($element).scope().node;
				var to = scope.node;
				var before = $next.length ? angular.element($next).scope().node : null;

				setTimeout(function () {
					if (context.siblingCount != context.parent.Children.length)
						return;

					console.log("sorting ", from.Current.Title, " -> ", to.Current.Title, " (before ", before && before.Current && before.Current.Title, ")");
					Content.move({ selected: from.Current.Path, to: to.Current.Path, before: before && before.Current && before.Current.Path }, {});
				}, 0);
			}
			return this;
		};
	});

})(angular.module('n2.services', ['ngResource']));