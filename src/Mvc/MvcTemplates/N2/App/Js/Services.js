(function (module) {
	console.log("services.js");

	module.factory('Interface', function ($resource) {
		var res = $resource('Api/Interface.ashx', {}, {});
		return res;
	});

	module.factory('Children', function ($resource) {
		var res = $resource('Api/Children.ashx', {}, {});
		return res;
	});

	module.factory('Translations', function ($resource) {
		var res = $resource('Api/Translations.ashx', {}, {});
		return res;
	});

	module.factory('Context', function ($resource) {
		var res = $resource('Api/Context.ashx', {}, {});
		return res;
	});

	module.factory('Versions', function ($resource) {
		var res = $resource('Api/Versions.ashx', {}, {});
		return res;
	});

})(angular.module('n2.services', ['ngResource']));