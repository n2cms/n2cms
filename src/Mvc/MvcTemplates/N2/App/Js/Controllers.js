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
	$scope.node = $scope.Interface.Content;
}
function BranchCtrl($scope) {
	$scope.node = $scope.child;
}