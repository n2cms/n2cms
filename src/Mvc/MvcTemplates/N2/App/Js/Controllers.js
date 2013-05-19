angular.module('n2', ['n2.routes', 'n2.directives', 'n2.services', 'ui'], function () {
});

function ManagementCtrl($scope, Interface, Context, $window) {
	
	var viewMatch = window.location.search.match(/[?&]view=([^?&]+)/);
	var selectedMatch = window.location.search.match(/[?&]selected=([^?&]+)/);
	$scope.Interface = Interface.get({
		view: viewMatch && viewMatch,
		selected: selectedMatch && selectedMatch[1],
	});
	$scope.Context = {
		Node: {
			Current: {
				PreviewUrl: "Empty.aspx"
			}
		}
	}
	$scope.$watch("Context.Node.Current.Path", function (path) {
		if (path) {
			var item = $scope.Context.Node.Current;
			Context.get({ selected: item.Path }, function (ctx) {
				angular.extend($scope.Context, ctx);
			});
		}
	});
}

function SearchCtrl() {
}

function NavigationCtrl($scope) {
	$scope.$watch("Interface.User.PreferredView", function (view) {
		$scope.viewPreference = view == 0
			? "draft"
			: "published";
	});
}

function TrashCtrl($scope) {

}

function TrunkCtrl($scope, Content, SortHelperFactory) {
	$scope.$watch("Interface.Content", function (content) {
		$scope.node = content;
		if (content)
			$scope.Context.Node = findSelectedRecursive(content);
	});
	$scope.toggle = function (node) {
		node.Expanded = !node.Expanded;
	};
	function findSelectedRecursive(node) {
		if (node.Selected) {
			return node;
		}
		for (var i in node.Children) {
			var n = findSelectedRecursive(node.Children[i]);
			if (n) return n;
		}
		return null;
	}
	$scope.select = function (node) {
		$scope.Context.Node.Selected = false;
		$scope.Context.Node = node;
		node.Selected = true;
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
function PageActionCtrl($scope, $interpolate) {
	$scope.evaluateExpression = function (expr) {
		return expr && $interpolate(expr)($scope);
	};
}
function LanguageCtrl($scope, Translations) {
	$scope.onOver = function (node) {
		if (node.Children.length && node.Selected == node.Current.Path)
			return;
		node.Selected = node.Current.Path;
		node.Loading = true;
		node.Children = Translations.query({ selected: node.Current.Path }, function () {
			node.Loading = false;
		});
	}
}

function VersionsCtrl($scope, Versions) {
	$scope.onOver = function (node) {
		if (node.Children.length && $scope.Selected == node.Current.Path)
			return;
		$scope.Selected = node.Current.Path;
		node.Loading = true;
		node.Children = Versions.query({ selected: node.Current.Path }, function (versions) {
			node.Loading = false;
		});
	}
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