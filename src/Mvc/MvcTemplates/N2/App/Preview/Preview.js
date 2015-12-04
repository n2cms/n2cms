n2 = window.n2 || {};

n2.preview = angular.module('n2preview', ['n2.directives', 'n2.services'], function () {
});

n2.preview.factory("ZoneOperator", ["$window", "Context", "Uri", function ($window, Context, Uri) {
	
	function Organizable() {
		this.reveal = function () {
			var $dropPoint = $(this.$element).find(".n2-drop-area.n2-append");
			$("html,body").animate({ scrollTop: $dropPoint.offset().top - window.innerHeight / 3 }, function () {
				$dropPoint[0].scrollIntoViewIfNeeded();
			})
		}
	}
	function Zone(element) {
		var zone = this;
		this.name = $(element).attr("data-zone");
		this.title = element.title || this.name;
		this.allowed = $(element).attr("data-allowed").split(",");
		this.path = $(element).attr("data-item");
		this.versionKey = $(element).attr("data-versionKey");
		this.$element = element;
		this.parts = [];
		if ($(element).closest(".dropZone").attr("data-versionIndex")) {
			this.belowVersionIndex = $(element).closest(".dropZone").attr("data-versionIndex");
			this.belowVersionKey = $(element).closest(".dropZone").attr("data-versionKey");
		}
		this.createUrl = function (template, beforePart) {
			var qs = { zoneName: this.name, n2versionIndex: Context.CurrentItem.VersionIndex, n2scroll: document.body.scrollTop, belowVersionKey: this.belowVersionKey, returnUrl: encodeURIComponent(window.location.pathname + window.location.search) };
			qs[Context.Paths.SelectedQueryKey] = this.path;
			if (beforePart) {
				angular.extend(qs, { before: !beforePart.versionKey && beforePart.path, beforeSortOrder: beforePart.sortOrder, beforeVersionKey: beforePart.versionKey });
			} else {
				angular.extend(qs, { below: this.path, n2versionKey: this.versionKey });
			}
			var uri = new Uri(template.EditUrl).setQuery(qs);
			return uri.toString();
		}
		this.addPlaceholders = function (template, callback) {
			var createUrl = this.createUrl(template);
			$("<div class='n2-drop-area n2-append'><a href='" + createUrl + "'>" + "Append to <b>" + this.title + "</b></a></div>")
				.click(function (e) {
					callback && callback(e, zone);
				})
				.appendTo(this.$element);

			angular.forEach(zone.parts, function (part) {
				var createUrl = zone.createUrl(template, part);
				$("<div class='n2-drop-area n2-prepend'><a href='" + createUrl + "'>" + "Insert into <b>" + zone.title + "</b></a></div>")
					.click(function (e) {
						callback && callback(e, zone, part);
					})
					.prependTo(part.$element);
			})
		};
		this.removePlaceholders = function () {
			$(".n2-drop-area", this.$element).remove();
		}

		$(".zoneItem", element).each(function () {
			if ($(this).closest(".dropZone")[0] != element)
				return; // sub-zone's items
			var part = new Part(this);
			zone.parts.push(part);
		});

		return this;
	}
	Zone.prototype = new Organizable();

	function Part(element) {
		this.path = $(element).attr("data-item");
		this.versionKey = $(element).attr("data-versionkey")
		this.sortOrder = $(element).attr("data-sortorder");
		this.type = $(element).attr("data-sortorder");
		this.$element = element;
		return this;
	}
	Zone.prototype = new Organizable();

	function ZoneOperator($scope) {
		var operator = this;
		this.zones = [];
		this.names = [];
		this.removePlaceholders = function () {
			angular.forEach(this.zones, function (zone) {
				zone.removePlaceholders();
			});
		}

		$(".dropZone", $window.document).each(function () {
			var zone = new Zone(this);
			operator.zones.push(zone);
			operator.names.push(zone.name);
		})

		$(".titleBar .move").click(function (e) {
			e.preventDefault();
			console.log("move it")
		});
	}
	return ZoneOperator;
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
		controller: function ($scope, Mode, Fullscreen, ContentFactory, Security, Uri) {
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

n2.preview.directive("n2PreviewParts", [function () {
	return {
		controller: function ($scope, Context, ZoneOperator) {
			var operator = new ZoneOperator($scope);

			$scope.toggleParts = function () {
				$scope.adding = null;
				if ($scope.templates)
					$scope.templates = null;
				else
					$scope.templates = Context.Templates
			}

			$scope.scrollTo = function (zone) {
				zone.reveal();
			}

			$scope.beginAdding = function (template) {
				$scope.adding = {
					zones: [],
					template: template
				};
				angular.forEach(operator.zones, function (zone) {
					if (zone.allowed.indexOf(template.Discriminator) >= 0) {
						$scope.templates = null;
						$scope.adding.zones.push(zone);
						zone.addPlaceholders(template, "create");
					}
				})
			}

			$scope.cancelAdding = function () {
				$scope.adding = null;
				operator.removePlaceholders();
			}
		}
	}
}])
