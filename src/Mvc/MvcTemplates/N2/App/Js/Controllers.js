angular.module('n2', ['n2.routes', 'n2.directives', 'n2.services', 'ui'], function () {
});

function ManagementCtrl($scope, $window, $timeout, $interpolate, Interface, Context, Content) {
	$scope.Content = Content;

	var viewMatch = window.location.search.match(/[?&]view=([^?&]+)/);
	var selectedMatch = window.location.search.match(/[?&]selected=([^?&]+)/);
	$scope.Interface = Interface.get({
		view: viewMatch && viewMatch[1],
		selected: selectedMatch && selectedMatch[1],
	});
	$scope.Context = {
		CurrentItem: {
			PreviewUrl: "Empty.aspx"
		},
		SelectedNode: {
		},
		ContextMenu: {
		}
	}

	$scope.select = function (node) {
		$scope.Context.SelectedNode = node;
		$timeout(function () {
			Context.get({ selected: node.Current.Path, view: $scope.Interface.Paths.ViewPreference }, function (ctx) {
				angular.extend($scope.Context, ctx);
			});
		}, 200);
		//$scope.Context.CurrentItem = node.Current;
	}

	//$scope.$watch("Context.CurrentItem.Path", function (path) {
	//	if (path) {
	//		var item = $scope.Context.CurrentItem;
	//		var loco = window.preview.location;
	//		if (loco.pathname != item.PreviewUrl && loco.pathname + loco.search != item.PreviewUrl) {
	//			console.log("path changed", path, " -- ", loco.pathname + loco.search, item.PreviewUrl);
	//			Context.get({ selected: item.Path }, function (ctx) {
	//				angular.extend($scope.Context, ctx);
	//			});
	//		}
	//	}
	//});
	$scope.$watch("Context.ContextMenu", function (menu) {
		console.log("context menu", menu);
	});

	var viewExpression = /[?&]view=[^?&]*/;
	$scope.$on("loaded", function (scope, e) {
		var ctxUrl = $scope.Context.CurrentItem.PreviewUrl;
		if (e.path + e.query != ctxUrl && e.path != ctxUrl.replace(viewExpression, "")) {
			// reload
			$timeout(function () {
				console.log("url changed", e.path + e.query, "!=", ctxUrl, e.path, "!=", ctxUrl.replace(viewExpression, ""));
				Context.get({ selectedUrl: e.path + e.query }, function (ctx) {
					if (ctx.NotFound)
						return;
					
					console.log("reloaded", ctx);
					angular.extend($scope.Context, ctx);
					$scope.$broadcast("contextchanged", $scope.Context);
				});
			}, 200);
		}
	});

	$scope.evaluateExpression = function (expr) {
		return expr && $interpolate(expr)($scope);
	};
}

function MainMenuCtrl($scope) {
	$scope.$watch("Interface.MainMenu", function (mainMenu) {
		$scope.menu = mainMenu;
	});
	$scope.$watch("Interface.User", function (user) {
		$scope.user = user;
	});
}

function SearchCtrl($scope, $timeout, Content) {
	$scope.searchExpression = "";
	var cancel = null;
	$scope.$watch("searchExpression", function (searchExpression) {
		cancel && $timeout.cancel(cancel);
		cancel = $timeout(function () {
			$scope.search(searchExpression + "*");
		}, 500);
	});
	$scope.clear = function () {
		$scope.hits = [];
		$scope.searchExpression = "";
	};
	$scope.search = function (searchExpression) {
		if (!searchExpression || searchExpression == "*") {
			return $scope.clear();
		}
		$scope.searching = true;
		Content.search({ q: searchExpression, take: 20, selected: $scope.Context.CurrentItem.Path, pages: true }, function (data) {
			$scope.hits = data.Hits;
			$scope.searching = false;
		});
	}
}

function NavigationCtrl($scope, ContextMenuFactory) {
	$scope.$watch("Interface.User.PreferredView", function (view) {
		$scope.viewPreference = view == 0
			? "draft"
			: "published";
	});

	$scope.ContextMenu = ContextMenuFactory($scope);
}

function TrashCtrl($scope) {

}

function TrunkCtrl($scope, Content, SortHelperFactory) {
	$scope.$watch("Interface.Content", function (content) {
		$scope.node = content;
		if (content)
			$scope.Context.SelectedNode = findSelectedRecursive(content, $scope.Interface.SelectedPath);
	});
	$scope.$on("contextchanged", function (scope, ctx) {
		$scope.Context.SelectedNode = findSelectedRecursive($scope.Interface.Content, ctx.CurrentItem.Path);
	});

	$scope.toggle = function (node) {
		node.Expanded = !node.Expanded;
	};
	function findSelectedRecursive(node, selectedPath) {
		if (!node)
			return null;
		if (node.Current.Path == selectedPath) {
			return node;
		}
		for (var i in node.Children) {
			var n = findSelectedRecursive(node.Children[i], selectedPath);
			if (n) return n;
		}
		return null;
	}
	$scope.loadRemaining = function (node) {
		node.Loading = true;
		Content.children({ selected: node.Current.Path, skip: node.Children.length }, function (data) {
			node.Children.length--;
			for (var i in data.Children)
				node.Children.push(data.Children[i]);
			node.Loading = false;
			node.IsPaged = false;
		});
	}
	$scope.$on("moved", function (e, content) {
		console.log("moved", content);
	});
	$scope.sort = new SortHelperFactory($scope, Content);
}

function BranchCtrl($scope, Content, SortHelperFactory) {
	$scope.node = $scope.child;
	$scope.toggle = function (node) {
		if (!node.Expanded && !node.Children.length) {
			node.Loading = true;
			Content.children({ selected: node.Current.Path }, function (data) {
				node.Children = data.Children;
				node.Loading = false;
				if (data.IsPaged)
					node.IsPaged = true;
			});
		}
		node.Expanded = !node.Expanded;
	};
	$scope.sort = new SortHelperFactory($scope, Content);
}

function PageActionBarCtrl($scope, Content) {
}

function PageActionCtrl($scope) {
}

function PreviewCtrl($scope) {
	$scope.frameLoaded = function (e) {
		try {
			var loco = e.target.contentWindow.location;
			$scope.$emit("loaded", { path: loco.pathname, query: loco.search, url: loco.toString() });
		} catch (ex) {
			console.log(ex);
		}
	};

	//if ($scope.Context.CurrentItem) {
	//	$scope.src = $scope.Context.CurrentItem.PreviewUrl;
	//}
	//var remove = $scope.$watch("Context.CurrentItem", function (item) {
	//	if (item && item.PreviewUrl != "Empty.aspx") {
	//		$scope.src = item.PreviewUrl;
	//		remove || setTimeout(remove, 10);
	//	}
	//});
}

function AddCtrl($scope, Definitions) {
	$scope.loadDefinitions = function (node) {
		console.log("loading definitions for ", node);
		node.Selected = node.Current.Path;
		node.Loading = true;
		node.Children = Definitions.query({ selected: $scope.Context.CurrentItem.Path }, function () {
			node.Loading = false;
		});
	}
}

function LanguageCtrl($scope, Translations) {
	$scope.loadLanguages = function (node) {
		console.log("loading languages for ", node);
		node.Selected = node.Current.Path;
		node.Loading = true;
		node.Children = Translations.query({ selected: $scope.Context.CurrentItem.Path }, function () {
			node.Loading = false;
		});
	}
}

function VersionsCtrl($scope, Versions) {
	$scope.loadVersions = function (node) {
		console.log("loading versions for ", node);

		$scope.Selected = node.Current.Path;
		node.Loading = true;
		node.Children = Versions.query({ selected: $scope.Context.CurrentItem.Path }, function (versions) {
			node.Loading = false;
		});
	}
}

function PagePublishCtrl($scope, $rootScope) {
	$rootScope.$on("loaded", function (scope, e) {
		
	});
}

function AlertCtrl($scope, $timeout, Alert) {
	var defaults = { visible: true, type: "warning" };

	function clear() {
		$scope.Alert = {};
	};

	Alert.subscribe(function (options) {
		setTimeout(function () {
			$scope.Alert = angular.extend({}, defaults, options);
			options.timeout && $timeout(clear, options.timeout);
			$scope.$digest();
		}, 10);
	});
}
