function TargetingCtrl($rootScope, $scope) {
	$scope.preview = {
		size: { w: 320, h: 480, title: "Testing 123" },
		url: "/",
		horizontal: false
	}

	$rootScope.$on("device-preview", function (e, size) {
		$scope.preview = {
			size: size,
			url: window.frames.preview && window.frames.preview.window.location.toString(),
			horizontal: size.w > size.h
		};
		console.log($scope.preview);
	});
	$rootScope.$on("device-restore", function (e, size) {
		delete $scope.preview;
	});
	$rootScope.$on("resized", function () {
		$scope.preview.size.title = "Custom";
	});
	$scope.frameLoaded = function (e) {
		try {
			var loco = e.target.contentWindow.location;
			$scope.$emit("preiewloaded", { path: loco.pathname, query: loco.search, url: loco.toString() });
		} catch (ex) {
			console.log("frame access exception", ex);
		}
	};
};