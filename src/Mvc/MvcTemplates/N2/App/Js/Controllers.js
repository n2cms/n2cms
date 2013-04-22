angular.module('n2', ['n2.routes', 'n2.directives', 'n2.services'], function () {
	console.log("controllers.js");
});

function ManagementCtrl($scope, Interface) {
	$scope.Interface = Interface.get();
	$scope.Preview = { Url: "/" };
}
function SearchCtrl() {
}
function NavigationCtrl($scope) {
}
function TrunkCtrl($scope) {
	$scope.$watch("Interface.Content", function (content) {
		$scope.node = content;
	});
	$scope.toggle = function (node) {
		node.Expanded = !node.Expanded;
	};
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