angular.module('n2', ['n2.routes', 'n2.directives', 'n2.services'], function () {
	console.log("controllers.js");
});

function ManagementCtrl($scope, Interface) {
	$scope.Interface = Interface.get();
	$scope.Context = {
		Node: {
			Current: {
				PreviewUrl: "Empty.aspx"
			}
		}
	}
}
function SearchCtrl() {
}
function NavigationCtrl() {
}
function TrunkCtrl($scope) {
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
}
function BranchCtrl($scope, Children) {
	$scope.node = $scope.child;
	$scope.toggle = function (node) {
		console.log(node);
		if (!node.Expanded && !node.Children.length) {
			node.Children = Children.query({ selected: node.Current.Path });
		}
		node.Expanded = !node.Expanded;
	};
}
function PageActionCtrl($scope, $interpolate) {
	$scope.evaluateExpression = function (expr) {
		return expr && $interpolate(expr)($scope);
	};
}