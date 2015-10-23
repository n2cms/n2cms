function PageStatsCtrl($rootScope, $scope, $resource) {
	var res = $resource('Api/Content.ashx/:target', { target: 'statistics' }, {
		'statistics': { method: 'GET', params: { target: 'statistics' } },
	});
	function getMaximum(localMax, globalMax) {
		while (localMax > globalMax)
			globalMax *= 10;
		return globalMax;
	}
	$scope.max = 1;
	function loadStatistics() {
		if (!$scope.Context.CurrentItem)
			return;
		res.statistics({ n2item: $scope.Context.CurrentItem.ID }, function (result) {
			$rootScope.statisticsMaximum = getMaximum(result.Max, $rootScope.statisticsMaximum || 1)
			$scope.max = $rootScope.statisticsMaximum;
			$scope.Statistics = result;
			$scope.stale = false;
		});
	};
	$rootScope.$on("contextchanged", function (scope, args) {
		$scope.stale = true;
		if ($scope.showInfo) {
			loadStatistics();
		}
	});
	$scope.$watch("showInfo", function (show) {
		if (show && $scope.stale) {
			loadStatistics();
		}
	});
	$scope.resetMax = function () {
		console.log("resetting");
		$rootScope.statisticsMaximum = 1;
		loadStatistics();
	}
};
