function TargetingCtrl($rootScope, $scope, Translate, n2translations) {
	angular.extend(n2translations, {
		targeting: {
			menu: {
				fullsize: { text: " Full size" },
				rotate: { text: " Rotate" }
			},
			preview: {
				custom: "Custom",
				viewportsize: "Viewport size",
				close: "Close"
			}
		}
	});

	function appendFlag(flag) {
		$scope.Context.Flags.push(flag);
	};
	function removeFlag(flag) {
		var index = $scope.Context.Flags.indexOf(flag);
		if (index >= 0)
			$scope.Context.Flags.splice(index, 1);
	};


	function appendPreviewFlag() {
		removeFlag("PreviewFullscreen");
		removeFlag("PreviewTilted")

		if ($scope.preview)
			appendFlag("Preview" + $scope.preview.size.Name);
		else
			appendFlag("PreviewFullscreen");
	}

	function removePreviewFlag() {
		if (!$scope.preview)
			return;

		removeFlag("Preview" + $scope.preview.size.Name);
	}

	$rootScope.$on("contextchanged", function (scope, e) {
		if ($scope.Context.Flags.indexOf('Management') >= 0)
			delete $scope.preview;
		else
			appendPreviewFlag();
	});

	function rotate() {
		var s = $scope.preview.size;
		var newDimensions = { Height: s.Width, Width: s.Height };
		angular.extend(s, newDimensions);
		if ($scope.isFlagged("PreviewTilted"))
			removeFlag("PreviewTilted")
		else
			appendFlag("PreviewTilted");
	}

	$rootScope.$on("device-preview", function (e, size) {
		removePreviewFlag();
		if ($scope.preview && $scope.preview.size && $scope.preview.size.Height == size.Height && $scope.preview.size.Width == size.Width) {
			rotate();
			return;
		}
		$scope.preview = {
			size: size,
			url: window.frames.preview && window.frames.preview.window.location.toString()
		};
		appendPreviewFlag();
	});
	$rootScope.$on("device-close", function (e, size) {
		removePreviewFlag();
		delete $scope.preview;
		appendPreviewFlag();
	});
	$rootScope.$on("device-rotate", function (e, size) {
		if ($scope.preview) {
			rotate();
		}
	});
	$rootScope.$on("resized", function () {
		$scope.preview.size.Title = Translate("targeting.preview.custom");
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

function DevicePreviewMenuCtrl($scope) {
	$scope.isPreviewing = function () {
		console.log("isPreviewing", !!$scope.preview);
		return !!$scope.preview;
	}
}