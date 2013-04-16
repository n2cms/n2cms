angular.module('n2', ['n2.routes', 'n2.directives', 'n2.services'], function () {
	console.log("controllers.js");
});

function ManagementCtrl($scope, Interface) {
	$scope.Interface = Interface.get();
}
function SearchCtrl() {
}
function NavigationCtrl($scope) {
}