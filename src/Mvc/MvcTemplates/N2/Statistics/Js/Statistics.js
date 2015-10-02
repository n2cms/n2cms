function PageStatsCtrl($rootScope, $scope, $resource) {
	var res = $resource('Api/Content.ashx/:target', { target: 'statistics' }, {
		'statistics': { method: 'GET', params: { target: 'statistics' } },
	});
	$rootScope.$on("contextchanged", function (scope, e) {
		res.statistics({}, function (result) {
			$scope.Statistics = result;
			//$scope.Views = [];

			//$scope.Max = 0;
			//angular.forEach(result.Views, function (v, i) {
			//	$scope.Max = Math.max($scope.Max, v);
			//	$scope.Views.push({ Views: v });
			//})
		});
	});
};
