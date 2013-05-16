(function (module) {
	module.factory('Interface', function ($resource) {
		var res = $resource('Api/Interface.ashx', {}, {});
		return res;
	});

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
		window.Ct = Content;
		var context = {}
		return function (scope) {
			function reload(ctx) {
				var node = ctx.scopes.to && ctx.scopes.to.node;
				if (!node) return;

				node.HasChildren = true;
				node.Loading = true;
				Content.children({ selected: node.Current.Path }, function (data) {
					//ctx.elements.selected.remove();

					node.Children = data.Children;
					node.Expanded = true;
					node.Loading = false;
					if (data.IsPaged)
						node.IsPaged = true;
				});
			}
			this.move = function (ctx) {
				console.log("moving", ctx);
				Content.move(ctx.paths, function () {
					console.log("moved", ctx);

					reload(ctx);
				});
			};
			this.sort = function (ctx) {
				console.log("sorting", ctx);
				Content.sort(ctx.paths, function () {
					console.log("sorted", ctx);

					reload(ctx);
				});
			};

			return this;
		};
	});

})(angular.module('n2.services', ['ngResource']));