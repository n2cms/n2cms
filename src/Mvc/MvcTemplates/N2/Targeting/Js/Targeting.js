function TargetingCtrl($rootScope, $scope) {
	$rootScope.$on("contextchanged", function (scope, e) {
		if ($scope.Context.Flags.indexOf('Management') >= 0)
			delete $scope.preview;
	});

	function rotate() {
		var s = $scope.preview.size;
		var newDimensions = { Height: s.Width, Width: s.Height };
		angular.extend(s, newDimensions);
	}

	$rootScope.$on("device-preview", function (e, size) {
		if ($scope.preview && $scope.preview.size && $scope.preview.size.Height == size.Height && $scope.preview.size.Width == size.Width) {
			rotate();
			return;
		}
		$scope.preview = {
			size: size,
			url: window.frames.preview && window.frames.preview.window.location.toString()
		};
		console.log($scope.preview);
	});
	$rootScope.$on("device-close", function (e, size) {
		delete $scope.preview;
	});
	$rootScope.$on("device-rotate", function (e, size) {
		if ($scope.preview) {
			rotate();
		}
	});
	$rootScope.$on("resized", function () {
		$scope.preview.size.Title = "Custom";
	});

	$scope.frameLoaded = function (e) {
		try {
			var loco = e.target.contentWindow.location;
			$scope.$emit("preiewloaded", { path: loco.pathname, query: loco.search, url: loco.toString() });
		} catch (ex) {
			$scope.$emit("preiewaccessexception", { ex: ex });
			console.log("frame access exception", ex);
		}
	};
};