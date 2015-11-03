var n2framed = angular.module('n2framed', ['n2.directives', 'n2.services'], function () {
});

n2framed.controller("EditLayoutCtrl", ["$scope", function ($scope) {
	$scope.toggleSidebar = function () {
		$scope.sidebarOpen = !$scope.sidebarOpen;
	}
}])
