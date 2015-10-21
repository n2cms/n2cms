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
			},
			targets: {
				clear: { text: " Clear targets" }
			}
		}
	});

	function appendFlag(flag) {
		$scope.Context.Flags[flag] = true;
	};
	
	function removeFlag(flag) {
		$scope.Context.Flags[flag] = false;
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

	function rotate() {
		var s = $scope.preview.size;
		var newDimensions = { Height: s.Width, Width: s.Height };
		angular.extend(s, newDimensions);
		if ($scope.isFlagged("PreviewTilted"))
			removeFlag("PreviewTilted")
		else
			appendFlag("PreviewTilted");
	}

	var targets = [];

	function reloadWithTargets() {
		var url = window.frames.preview.location.toString();
		url = $scope.appendQuery(url, "targets", targets.join());
		window.frames.preview.location = url;
	}

	function toggleTarget(name) {
		var targetIndex = targets.indexOf(name);
		if (targetIndex < 0) {
			targets.push(name);
			appendFlag("Target" + name);
		} else {
			targets.splice(targetIndex, 1);
			removeFlag("Target" + name);
		}

		$scope.setPreviewQuery("targets", targets.join());
		reloadWithTargets();
	}

	function clearTargets() {
		for (var i in targets) {
			removeFlag("Target" + targets[i]);
		}
		targets.length = 0;
		$scope.setPreviewQuery("targets", null);
		reloadWithTargets();
	}

	$rootScope.$on("contextchanged", function (scope, e) {
		if ($scope.Context.Flags.Management)
			delete $scope.preview;
		else
			appendPreviewFlag();

		for (var i in targets) {
			$scope.Context.Flags["Target" + targets[i]] = true;
		}
	});

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

	$rootScope.$on("target-preview", function (e, data) {
		toggleTarget(data.Name);
	});

	$rootScope.$on("targets-clear", function (e, data) {
		clearTargets();
	});
};
