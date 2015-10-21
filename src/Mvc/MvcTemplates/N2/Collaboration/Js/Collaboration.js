function CollaborationNotesCtrl($scope, $rootScope, $resource, Translate, Eventually) {
	var res = $resource('Api/Content.ashx/:target/:action?n2item=:n2item', { target: 'collaboration' }, {
		'notes': { method: 'GET', params: { action: 'notes' } },
		'saveNote': { method: 'POST', params: { action: 'notes' } },
	});

	var currentID;
	function loadNotes() {
		if (!$scope.Context.CurrentItem)
			return;
		if (currentID == $scope.Context.CurrentItem.ID)
			return;
		$scope.note = null;
		$scope.placeholder = null;
		Eventually(function () {
			currentID = $scope.Context.CurrentItem.ID;
			if ($scope.Context.CurrentItem.ID == 0)
				return;
			res.notes({ n2item: $scope.Context.CurrentItem.ID }, function (result) {
				$scope.note = result.Notes[0];
				$scope.placeholder = Translate("collaboration.notes.placeholder", "Write shared note on ") + $scope.Context.CurrentItem.Title;
			});
		}, 100);
	}

	$scope.saveNote = function () {
		var id = $scope.Context.CurrentItem.ID;
		var note = $scope.note;
		Eventually(function () {
			res.saveNote({ n2item: id}, { note: note })
		}, 1000);
	}

	$rootScope.$on("contextchanged", loadNotes);
}
