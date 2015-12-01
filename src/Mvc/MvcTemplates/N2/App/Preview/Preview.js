n2 = window.n2 || {};

n2.preview = angular.module('n2preview', ['n2.directives', 'n2.services'], function () {
});

n2.preview.factory("Zone", ["$window", function ($window) {
	var result = {
		zones: [],
		names: []
	};
	$(".dropZone", $window.document).each(function () {
		var zone = {
			zone: $(this).attr("data-zone"),
			allowed: $(this).attr("data-allowed").split(","),
			$element: this,
			parts: [],
			appendPlaceholder: function (template, callback) {
				$("<a href class='dropPoint append'></a>")
					.text(this.zone + ": Click to append " + template.Title)
					.click(callback)
					.appendTo(this.$element);
				$("<a href class='dropPoint prepend'></a>")
					.text(this.zone + ": Click to prepend " + template.Title)
					.click(callback)
					.appendTo($(".zoneItem", this.$element));
			}
		};
		$(".zoneItem", this).each(function () {
			var part = {
				path: $(this).attr("data-item"),
				sortOrder: $(this).attr("data-sortorder"),
				type: $(this).attr("data-sortorder"),
				$element: this
			};
			zone.parts.push(part);
		});
		result.zones.push(zone);
		result.names.push(zone.name);
	})
	return result;
}]);

n2.preview.factory("Mode", ["$window", function ($window) {
	return (/edit=([^&]*)/.exec($window.location.search) || [])[1] || "view";
}]);

n2.preview.factory("Context", ["$window", function ($window) {
	return $window.n2.settings;
}]);

n2.preview.factory("Fullscreen", ["$window", function ($window) {
	try {
		return !window.top.n2ctx || !window.top.n2ctx.hasTop();
	} catch (e) {
		console.warn(e);
		return true;
	}
}]);

n2.preview.directive("n2Preview", ["$http", "$templateCache", "$compile", "Paths", "Context", function ($http, $templateCache, $compile, Paths, Context) {
	Paths.initialize(Context.Paths);

	return {
		link: function(scope, element){
			$http.get("/N2/App/Preview/PreviewBar.html", { cache: $templateCache }).success(function (response) {
				element.html(response);
				$compile(element.contents())(scope);
				element.removeClass("n2-loading").addClass("n2-loaded");
			});
		},
		controller: function ($scope, Mode, Fullscreen, ContentFactory, Security, Uri, Zone) {
			var Content = ContentFactory(Context.Paths);

			function appendSelection(url, ci, appendVersionIndex) {
				var uri = new Uri(url).appendQuery(Context.Paths.SelectedQueryKey, ci.Path).appendQuery(Context.Paths.ItemQueryKey, ci.ID);
				if (appendVersionIndex)
					uri = uri.appendQuery("n2versionIndex", ci.VersionIndex);
				return uri.toString();
			}
			
			$scope.mode = Mode;
			$scope.dragging = $scope.mode == "drag";
			$scope.fullscreen = Fullscreen;
			$scope.Context = Context;
			$scope.$watch("Context.CurrentItem", function (ci) {
				$scope.Paths = {
					management: appendSelection(Context.Paths.Management, ci),
					create: appendSelection(Context.Paths.Create, ci),
					edit: appendSelection(Context.Paths.Edit, ci, /*appendVersionIndex*/true),
					remove: appendSelection(Context.Paths.Delete, ci),
					discard: appendSelection(Context.Paths.Management + "Content/DiscardPreview.aspx", ci, /*appendVersionIndex*/true),
					preview: Context.Paths.PreviewUrl,
					organize: new Uri(Context.Paths.PreviewUrl).appendQuery("edit", "drag", /*appendVersionIndex*/true).toString(),
					publish: appendSelection(Context.Paths.Management + "Content/PublishPreview.aspx", ci, /*appendVersionIndex*/true)
				}
				var permissions = $scope.Permissions = {
					write: ci.MaximumPermission >= Security.permissions.Write,
					publish: ci.MaximumPermission >= Security.permissions.Publish,
					administer: ci.MaximumPermission >= Security.permissions.Administer
				}
				var states = $scope.States = {
					draft: Content.states.is(ci.State, Content.states.Draft),
					waiting: Content.states.is(ci.State, Content.states.Waiting),
					published: Content.states.is(ci.State, Content.states.Published),
					unpublished: Content.states.is(ci.State, Content.states.Unpublished),
					deleted: Content.states.is(ci.State, Content.states.Deleted)
				}
				$scope.publishable = permissions.publish && Content.states.is(ci.State, Content.states.Draft);
				$scope.publishableFuture = permissions.publish && Content.states.is(ci.State, Content.states.waiting);
				$scope.deletable = permissions.publish;
				$scope.discardable = permissions.write && Content.states.is(ci.State, Content.states.Draft);

				console.log("dragging", ci, Context)
			});

			if (Context.ActivityTracking.Path) {
				var handle = setInterval(function () {
					$.get(appendSelection(Context.ActivityTracking.Path + '?activity=View', Context.CurrentItem), function (result) {
						try { n2 && n2.context && n2.context(result) } catch (ex) { console.log(ex); }
					}).fail(function (result) {
						try { n2 && n2.failure && n2.failure(result) } catch (ex) { console.log(ex); }
					});
				}, Context.ActivityTracking.Interval * 1000);
				$scope.$on("$destroy", function () {
					clearInterval(handle);
				});
			}

			$scope.toggleParts = function () {
				if ($scope.templates)
					$scope.templates = null;
				else
					$scope.templates = Context.Templates
			}

			$scope.beginAdding = function (template) {
				console.log(template)
				angular.forEach(Zone.zones, function (zone) {
					if (zone.allowed.indexOf(template.Discriminator)) {
						$scope.templates = null;
						zone.appendPlaceholder(template, function () {
							console.log("clicked", arguments, this)
						});
					}
				})
			}
			
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
