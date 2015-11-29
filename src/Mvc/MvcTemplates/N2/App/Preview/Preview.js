n2 = window.n2 || {};

n2.preview = angular.module('n2preview', ['n2.directives', 'n2.services'], function () {
});

n2.preview.factory("Mode", ["$window", function ($window) {
	return (/edit=([^&]*)/.exec($window.location.search) || [])[1] || "view";
}]);

n2.preview.factory("Context", ["$window", function ($window) {
	return $window.n2.settings;
}]);

n2.preview.factory("Fullscreen", ["$window", function ($window) {
	return !window.top.n2ctx.hasTop();
}]);

n2.preview.directive("n2Preview", ["$http", "$templateCache", "$compile", function ($http, $templateCache, $compile) {
	return {
		link: function(scope, element){
			$http.get("/N2/App/Preview/PreviewBar.html", { cache: $templateCache }).success(function (response) {
				element.html(response);
				$compile(element.contents())(scope);
				element.removeClass("n2-loading").addClass("n2-loaded");
			});
		},
		controller: function ($scope, Mode, Context, Fullscreen) {
			$scope.mode = Mode;
			$scope.fullscreen = Fullscreen;
			$scope.Context = Context;
		}
	}
}])

n2.preview.directive("n2PreviewBar", [function () {
	return {
		controller: function ($scope) {
			//console.log("PreviewBarCtrl", $scope);
		}
	}
}])
