(function(app){
	app.value('$strapConfig', {
		datepicker: {
			language: 'en',
			format: 'M d, yyyy'
		}
	});
})(angular.module('n2', ['n2.directives', 'n2.services', 'n2.localization', 'ui', '$strap.directives']))

function findSelectedRecursive(node, selectedPath) {
	if (!node)
		return null;
	if (node.Current.Path == selectedPath) {
		return node;
	}
	for (var i in node.Children) {
		var n = findSelectedRecursive(node.Children[i], selectedPath);
		if (n) return n;
	}
	return null;
}

function decorate(obj, name, callback) {
	var original = obj[name] || function () { };
	obj[name] = function () {
		original.apply(this, arguments);
		callback.apply(this, arguments);
	};
};

function getParentPath(path) {
	var parentPathExpr = /((.*)[/])[^/]+[/]/;
	return parentPathExpr.exec(path) && parentPathExpr.exec(path)[1];;
}

function ManagementCtrl($scope, $window, $timeout, $interpolate, Context, Content, Security, FrameContext, Translate) {
	$scope.Content = Content;
	$scope.Security = Security;

	$scope.appendPreviewOptions = function (url) {
		if (url == "Empty.aspx")
			return url;

		for (var key in $scope.Context.PreviewQueries) {
			url = $scope.appendQuery(url, key, $scope.Context.PreviewQueries[key]);
		}

		return url;
	}

	$scope.setPreviewQuery = function (key, value) {
		if (value)
			$scope.Context.PreviewQueries[key] = value;
		else
			delete $scope.Context.PreviewQueries[key];
	}
	
	$scope.appendQuery = function(url, key, value)
	{
		if (!url) return url;

		var hashIndex = url.indexOf("#");
		if (hashIndex >= 0)
			return $scope.appendQuery(url.substr(0, hashIndex), key, value) + url.substr(hashIndex);

		var keyValue = key + (value ? ("=" + value) : "");

		var re = new RegExp("([?|&])" + key + "=.*?(&|$)", "i");
		if (url.match(re))
			return url.replace(re, '$1' + keyValue + '$2');
		else
			return url + (url.indexOf("?") < 0 ? "?" : "&") + keyValue
	}

	$scope.appendSelection = function (url, appendVersionIndex) {
		var ctx = $scope.Context;
		if (!ctx.CurrentItem)
			return url;
		url = $scope.appendQuery(url, ctx.Paths.SelectedQueryKey + "=" + ctx.CurrentItem.Path + "&" + ctx.Paths.ItemQueryKey + "=" + ctx.CurrentItem.ID);
		if (appendVersionIndex)
			url += "&versionIndex=" + ctx.CurrentItem.VersionIndex;
		return url;
	}

	$scope.previewUrl = function (url) {
		if (window.frames.preview)
			window.frames.preview.window.location = $scope.appendPreviewOptions(url) || "Empty.aspx";
	}

	decorate(FrameContext, "refresh", function (ctx) {
		// legacy refresh call from frame
		if (ctx.force) {
			$scope.reloadChildren(ctx.path);
			if (ctx.previewUrl) {
				$scope.previewUrl(ctx.previewUrl);
				return;
			}
		}

		if (ctx.mode && ctx.mode.indexOf('DragDrop') >= 0)
			// the context will be reoloaded anyway due to PreviewUrl != url with edit=drag
			return;

		if (!$scope.select(ctx.path, ctx.versionIndex)) {
			$scope.reloadChildren(getParentPath(ctx.path), function () {
				$scope.select(ctx.path, ctx.versionIndex, !ctx.force);
			});
		}
	});

	var viewMatch = window.location.search.match(/[?&]view=([^?&]+)/);
	var selectedMatch = window.location.search.match(/[?&]selected=([^?&]+)/);
	var organizeMatch = window.location.search.match(/[?&]mode=([^?&#]+)/);
	$scope.Context = {
		CurrentItem: {
			PreviewUrl: "Empty.aspx"
		},
		SelectedNode: {
		},
		ContextMenu: {
		},
		Partials: {
			Management: "App/Partials/Loading.html"
		},
		PreviewQueries: {}
	}

	function translateMenuRecursive(node) {
		var translation = node.Current && node.Current.Name && Translate(node.Current.Name);
		if (translation) {
			if (translation.text) node.Current.Title = translation.text;
			if (translation.title) node.Current.ToolTip = translation.title;
			if (translation.description) node.Current.Description = translation.description;
		}
		for (var i in node.Children) {
			translateMenuRecursive(node.Children[i]);
		}
	}

	$scope.extendSelection = function (settings) {
		
	};

	Context.full({
		view: viewMatch && viewMatch[1],
		selected: selectedMatch && selectedMatch[1]
	}, function (i) {
		$scope.Context.Partials.Management = "App/Partials/Management.html";
		translateMenuRecursive(i.Interface.MainMenu);
		translateMenuRecursive(i.Interface.ActionMenu);
		translateMenuRecursive(i.Interface.ContextMenu);
		angular.extend($scope.Context, i.Interface);
		angular.extend($scope.Context, i.Context);
		if (organizeMatch && organizeMatch[1] == "Organize")
			$scope.Context.Paths.PreviewUrl = $scope.appendQuery($scope.Context.Paths.PreviewUrl, "edit", "drag");
	});

	$scope.select = function (nodeOrPath, versionIndex, keepFlags, forceContextRefresh, preventReload) {
		if (typeof nodeOrPath == "string") {
			var path = nodeOrPath;
			var node = findSelectedRecursive($scope.Context.Content, path);
			if (!node) {
				var parentNode = findSelectedRecursive($scope.Context.Content, getParentPath(path));
				if (!preventReload && parentNode) {
					$scope.reloadChildren(parentNode, function () {
						$scope.select(path, versionIndex, keepFlags, forceContextRefresh, /*preventReload*/true);
					});
				}
			}
			else
				return $scope.select(node, versionIndex, keepFlags, forceContextRefresh);
		} else if (typeof nodeOrPath == "object") {
			var node = nodeOrPath;
			$scope.Context.SelectedNode = node;
			if (!node)
				return false;

			if ($scope.Context.AppliesTo == node.Current.PreviewUrl && !forceContextRefresh)
				return true;
			$scope.Context.AppliesTo = node.Current.PreviewUrl;

			$timeout(function () {
				Context.get({ selected: node.Current.Path, view: $scope.Context.User.ViewPreference, versionIndex: versionIndex }, function (ctx) {
					if (keepFlags)
						angular.extend($scope.Context, ctx, { Flags: $scope.Context.Flags });
					else
						angular.extend($scope.Context, ctx);
					$scope.$emit("contextchanged", $scope.Context);
				});
			}, 200);
			return true;
		}
	}

	$scope.reloadChildren = function(parentPathOrNode, callback) {
		var node = typeof parentPathOrNode == "string"
			? findSelectedRecursive($scope.Context.Content, parentPathOrNode)
			: parentPathOrNode;

		Content.loadChildren(node, callback);
	}

	$scope.isFlagged = function (flag) {
		return jQuery.inArray(flag, $scope.Context.Flags) >= 0;
	};
	
	var viewExpression = /[?&]view=[^?&]*/;
	$scope.$on("preiewloaded", function (scope, e) {
		if ($scope.Context.AppliesTo == (e.path + e.query))
			return;
		$scope.Context.AppliesTo = e.path + e.query;

		$timeout(function () {
			Context.get({ selectedUrl: e.path + e.query }, function (ctx) {
				angular.extend($scope.Context, ctx);
				$scope.$emit("contextchanged", $scope.Context);
			});
		}, 200);
	});

	$scope.evaluateExpression = function (expr) {
		return expr && $interpolate(expr)($scope);
	};

	$scope.isDisplayable = function (item) {
		if ($scope.Context.CurrentItem && !Security.permissions.is(item.Current.RequiredPermission, $scope.Context.CurrentItem.MaximumPermission)) {
			return false;
		}
		if (item.Current.DisplayedBy && item.Current.HiddenBy) {
			return $scope.isFlagged(item.Current.DisplayedBy)
				&& !$scope.isFlagged(item.Current.HiddenBy);
		}
		if (item.Current.HiddenBy) {
			return !$scope.isFlagged(item.Current.HiddenBy);
		}
		if (item.Current.DisplayedBy) {
			return $scope.isFlagged(item.Current.DisplayedBy);
		}
		return true;
	};
}

function NavigationCtrl($rootScope, $scope, Content, ContextMenuFactory, Eventually) {
	$scope.search = {
		execute: function (searchQuery) {
			if (!searchQuery)
				return $scope.search.clear();
			else if (searchQuery == $scope.search.searching)
				return;

			$scope.search.searching = searchQuery;
			Content.search({ q: searchQuery, take: 20, selected: $scope.Context.CurrentItem.Path, pages: true }, function (data) {
				$scope.search.hits = data.Hits;
				$scope.search.searching = "";
			});
		},
		clear: function () {
			$scope.search.query = "";
			$scope.search.searching = "";
			$scope.search.hits = null;
			$scope.search.focused = -1;
		},
		hits: null,
		query: "",
		searching: false,
		focused: undefined,
	};
	$scope.$watch("search.query", function (searchQuery) {
		Eventually(function () {
			$scope.search.execute(searchQuery);
			$scope.$digest();
		}, 400);
	});
	$scope.ContextMenu = new ContextMenuFactory($scope);
}

function TrunkCtrl($scope, $rootScope, Content, SortHelperFactory) {
	$scope.$watch("Context.Content", function (content) {
		$scope.node = content;
		if (content) {
			$scope.select(findSelectedRecursive(content, $scope.Context.SelectedPath));
		}
	});
	$rootScope.$on("contextchanged", function (scope, ctx) {
		if (ctx.CurrentItem)
			$scope.Context.SelectedNode = findSelectedRecursive($scope.Context.Content, ctx.CurrentItem.Path);
		else
			$scope.Context.SelectedNode = null;
	});

	$scope.toggle = function (node) {
		node.Expanded = !node.Expanded;
	};
	$scope.loadRemaining = function (node) {
		node.Loading = true;
		Content.children({ selected: node.Current.Path, skip: node.Children.length }, function (data) {
			node.Children.length--;
			for (var i in data.Children)
				node.Children.push(data.Children[i]);
			node.Loading = false;
			node.IsPaged = false;
		});
	}
	$scope.$on("moved", function (e, content) {
	});
	$scope.sort = new SortHelperFactory($scope, Content);
	$scope.parts = {
		show: function (node) {
			node.Loading = true;
			Content.children({ selected: node.Current.Path, pages: false }, function (data) {
				var zones = {};
				for (var i in data.Children) {
					var part = data.Children[i];
					var zoneName = part.Current.ZoneName || "(empty)";
					var zone = zones[zoneName];
					if (!zone)
						zones[zoneName] = zone = [];
					zone.push(part);
				}

				node.Parts = [];
				for (var zone in zones) {
					if (!zone)
						continue;
					var child = {
						Current: { Title: zone, IconClass: "n2-icon-columns silver", MetaInformation: [] },
						HasChildren: true,
						Children: zones[zone]
					}
					node.Parts.push(child);
				}

				delete node.Loading;
			});
		},
		hide: function (node) {
			delete node.Parts;
		}
	}
}

function BranchCtrl($scope, Content, Translate, SortHelperFactory) {
	$scope.node = $scope.child;
	$scope.toggle = function (node) {
		if (!node.Expanded && !node.Children.length) {
			Content.loadChildren(node);
		}
		node.Expanded = !node.Expanded;
	};
	$scope.sort = new SortHelperFactory($scope, Content);
	$scope.tags = [];
	if ($scope.node.Current) {
		var mi = $scope.node.Current.MetaInformation;
		if (mi) {
			if (mi.authority) $scope.tags.push({ ToolTip: Translate("branch.tags.authority", "Site: ") + (mi.authority.ToolTip || " (*)"), IconClass: "n2-icon-home", Url: "#" });
			if (mi.hidden) $scope.tags.push({ ToolTip: Translate("branch.tags.hidden", "Hidden"), IconClass: "n2-icon-eraser", Url: "#" });
			if (mi.language) $scope.tags.push({ ToolTip: Translate("branch.tags.language", "Language: ") + mi.language.Text, IconClass: "n2-icon-globe", Url: "#" });
			if (mi.locked) $scope.tags.push({ ToolTip: Translate("branch.tags.locked", "Access restrictions"), IconClass: "n2-icon-lock", Url: "#" });
			if (mi.zone) $scope.tags.push({ ToolTip: Translate("branch.tags.zone", "In zone: ") + mi.zone.Text, IconClass: "n2-icon-columns", Url: "#" });
			if (mi.draft) $scope.tags.push({ ToolTip: Translate("branch.tags.draft", "Has draft: ") + mi.draft.ToolTip, IconClass: "n2-icon-circle-blank", Url: "#" });
			if (mi.system) $scope.tags.push({ ToolTip: mi.system.ToolTip, IconClass: "n2-icon-qrcode", Url: "#" });
			if ($scope.node.Current.State == Content.states.Unpublished) $scope.tags.push({ ToolTip: Translate("branch.tags.unpublished", "Unpublished"), IconClass: "n2-icon-stop", Url: "#" });
		}
	}
}

function MenuCtrl($rootScope, $scope, Security) {
	$scope.$watch("Context.ActionMenu.Children", function (children) {
		var lefties = [];
		var righties = [];
		for (var i in children) {
			if (children[i].Current.Alignment == "Right")
				righties.push(children[i]);
			else
				lefties.push(children[i]);
		}
		$scope.primaryNavigation = lefties;
		$scope.secondaryNavigation = righties;
	});

	$scope.setViewPreference = function (viewPreference) {
		$scope.Context.User.ViewPreference = viewPreference;
	};
	$scope.$watch("Context.User.ViewPreference", function (viewPreference, previousPreference) {
		$scope.setPreviewQuery("view", viewPreference);
		var existingIndex = jQuery.inArray("View" + previousPreference, $scope.Context.Flags);
		if (existingIndex >= 0)
			$scope.Context.Flags.splice(existingIndex, 1);
		$scope.Context.Flags.push("View" + viewPreference);
	});
	$rootScope.$on("contextchanged", function (scope, ctx) {
		ctx.Flags.push("View" + ctx.User.ViewPreference)
	});
}

function PageActionCtrl($scope, Content) {
	$scope.dispose = function () {
		Content.remove({ selected: $scope.Context.CurrentItem.Path }, function () {
			$scope.reloadChildren(getParentPath($scope.Context.CurrentItem.Path));
		});
	}
}

function PreviewCtrl($scope, $rootScope) {
	$scope.frameLoaded = function (e) {
		try {
			var loco = e.target.contentWindow.location;
			$scope.$emit("preiewloaded", { path: loco.pathname, query: loco.search, url: loco.toString() });
		} catch (ex) {
			window.console && console.log("frame access exception", ex);
		}
	};
}

function AddCtrl($scope, Content) {
	$scope.loadDefinitions = function (node) {
		node.Selected = node.Current.Path;
		node.Loading = true;
		Content.definitions({ selected: $scope.Context.CurrentItem.Path }, function (data) {
			node.Loading = false;
			node.Children = data.Definitions;
		});
	}
}

function LanguageCtrl($scope, Content) {
	$scope.loadLanguages = function (node) {
		node.Selected = node.Current.Path;
		node.Loading = true;
		Content.translations({ selected: $scope.Context.CurrentItem.Path }, function (data) {
			node.Loading = false;
			node.Children = data.Translations;
		});
	}
}

function VersionsCtrl($scope, Content) {
	$scope.loadVersions = function (node) {
		$scope.Selected = node.Current.Path;
		node.Loading = true;
		Content.versions({ selected: $scope.Context.CurrentItem.Path }, function (data) {
			node.Loading = false;
			node.Children = data.Versions;
		});
	}
}

function PageInfoCtrl($scope, Content) {
	$scope.exctractLanguage = function (language) {
		return language && language.replace(/[(].*?[)]/, "");
	}
	$scope.toggleInfo = function () {
		$scope.$parent.showInfo = !$scope.$parent.showInfo;
	}
	$scope.definitions = {};
	Content.definitions({}, function (data) {
		for (var i in data.Definitions) {
			$scope.definitions[data.Definitions[i].TypeName] = data.Definitions[i];
		}
	});
}

function PagePublishCtrl($scope, $rootScope, $modal, Content) {
	$scope.publish = function () {
		Content.publish({ selected: $scope.Context.CurrentItem.Path, versionIndex: $scope.Context.CurrentItem.VersionIndex }, function (result) {
			$scope.previewUrl(result.Current.PreviewUrl);

			$scope.reloadChildren(getParentPath(result.Current.Path), function () {
				$scope.select(result.Current.Path, result.Current.VersionIndex, /*keepFlags*/false, /*forceContextRefresh*/true);
			});
		});
	};
	$scope.unpublish = function () {
		Content.unpublish({ selected: $scope.Context.CurrentItem.Path }, function (result) {
			$scope.previewUrl(result.Current.PreviewUrl);
			
			$scope.reloadChildren(getParentPath(result.Current.Path), function () {
				$scope.select(result.Current.Path, result.Current.VersionIndex, /*keepFlags*/false, /*forceContextRefresh*/true);
			});
		});
	};
}

function PageScheduleCtrl($scope, Content) {
	$scope.schedule = {
		"date": new Date(),
		"time": "00:00",
		submit: function () {
			var time = $scope.schedule.time;
			var hour = parseInt(time.substr(0, 2));
			var min = parseInt(time.substr(3, 2));
			var meridian = time.substr(5).replace(/^\s+|\s+$/g, '');
			if (meridian == "PM")
				hour += 12;

			var date = $scope.schedule.date;
			date.setHours(hour, min);

			Content.schedule({ selected: $scope.Context.CurrentItem.Path, versionIndex: $scope.Context.CurrentItem.VersionIndex, publishDate: date });
		}
	};
}

function FrameActionCtrl($scope, $rootScope, $timeout, FrameManipulator) {
	$scope.$parent.manipulator = FrameManipulator;
	$rootScope.$on("contextchanged", function (scope, e) {
		$scope.$parent.action = null;
		$scope.$parent.item.Children = [];
		var extraFlags = FrameManipulator.getFlags();
		for (var i in extraFlags) {
			$scope.Context.Flags.push(extraFlags[i]);
		}

		if ($scope.isFlagged("Management")) {
			function loadActions() {
				var actions = $scope.manipulator.getFrameActions();
				if (actions && actions.length) {
					$scope.$parent.manipulator.hideToolbar();

					$scope.$parent.action = actions[0];
					if (actions.length == 1)
						$scope.$parent.item.Children = actions[0].Children;
					else
						$scope.$parent.item.Children = actions;
				}
			}
			if (!FrameManipulator.isReady()) {
				var iterations = 0;
				var handle = setInterval(function () {
					iterations++;
					try {
						if (iterations < 10 || !FrameManipulator.isReady())
							return;
						loadActions();
						clearInterval(handle);
					} catch (e) {
						window.console && console.log("Error loading actions", e);
						clearInterval(handle);
					}
				}, 500);
			} else
				loadActions();
		}
	});
};

function NotifyCtrl($scope, $timeout, Notify) {
	var defaults = { visible: true, type: "warning" };

	function clear() {
		$scope.Notify = {};
	};

	Notify.subscribe(function (options) {
		setTimeout(function () {
			$scope.Notify = angular.extend({}, defaults, options);
			options.timeout && $timeout(clear, options.timeout);
			$scope.$digest();
		}, 10);
	});
}
