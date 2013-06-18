(function(app){
	app.value('$strapConfig', {
		datepicker: {
			language: 'en',
			format: 'M d, yyyy'
		}
	});
})(angular.module('n2', ['n2.routes', 'n2.directives', 'n2.services', 'n2.localization', 'ui', '$strap.directives']))

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

	$scope.previewUrl = function (url) {
		console.log("Previewing ", url);
		window.frames.preview.window.location = url || "Empty.aspx";
	}

	decorate(FrameContext, "refresh", function (ctx) {
		if (ctx.force) {
			$scope.reloadChildren(ctx.path);
			if (ctx.previewUrl) {
				console.log("PREVIEWING ", ctx.previewUrl);
				$scope.previewUrl(ctx.previewUrl);
				return;
			}
		}
		if (!$scope.select(ctx.path, ctx.versionIndex)) {
			$scope.reloadChildren(getParentPath(ctx.path), function () {
				$scope.select(ctx.path, ctx.versionIndex, !ctx.force);
			});
		}
	});

	var viewMatch = window.location.search.match(/[?&]view=([^?&]+)/);
	var selectedMatch = window.location.search.match(/[?&]selected=([^?&]+)/);
	$scope.Context = {
		CurrentItem: {
			PreviewUrl: "Empty.aspx"
		},
		SelectedNode: {
		},
		ContextMenu: {
		}
	}

	function translateNavigationRecursive(node) {
		var translation = node.Current && node.Current.Name && Translate(node.Current.Name);
		if (translation) {
			if (translation.text) node.Current.Title = translation.text;
			if (translation.title) node.Current.ToolTip = translation.title;
			if (translation.description) node.Current.Description = translation.description;
		}
		for (var i in node.Children) {
			translateNavigationRecursive(node.Children[i]);
		}
	}

	Context.full({
		view: viewMatch && viewMatch[1],
		selected: selectedMatch && selectedMatch[1]
	}, function (i) {
		console.log("Loading interface with", i);
		translateNavigationRecursive(i.Interface.MainMenu);
		translateNavigationRecursive(i.Interface.ActionMenu);
		translateNavigationRecursive(i.Interface.ContextMenu);
		angular.extend($scope.Context, i.Interface);
		angular.extend($scope.Context, i.Context);
		$scope.previewUrl(i.Interface.Paths.PreviewUrl);
	});

	$scope.select = function (nodeOrPath, versionIndex, keepFlags) {
		console.log("selecting", typeof nodeOrPath, nodeOrPath);
		if (typeof nodeOrPath == "string") {
			var path = nodeOrPath;
			var node = findSelectedRecursive($scope.Context.Content, path);
			if (!node) {
				var parentNode = findSelectedRecursive($scope.Context.Content, getParentPath(path));
				if (parentNode) {
					$scope.reloadChildren(parentNode, function () {
						$scope.select(path);
					});
				}
			}
			else
				return $scope.select(node, versionIndex);
		} else if (typeof nodeOrPath == "object") {
			var node = nodeOrPath;
			$scope.Context.SelectedNode = node;
			if (!node)
				return false;

			if ($scope.Context.AppliesTo == node.Current.PreviewUrl)
				return true;
			$scope.Context.AppliesTo = node.Current.PreviewUrl;

			$timeout(function () {
				Context.get({ selected: node.Current.Path, view: $scope.Context.Paths.ViewPreference, versionIndex: versionIndex }, function (ctx) {
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

		console.log("Reloading ", node);
		Content.loadChildren(node, callback);
	}

	$scope.isFlagged = function (flag) {
		return $scope.Context.Flags.indexOf(flag) >= 0;
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
}

function MainMenuCtrl($scope) {
	$scope.$watch("Context.MainMenu", function (mainMenu) {
		$scope.menu = mainMenu;
	});
	$scope.$watch("Context.User", function (user) {
		$scope.user = user;
	});
}

//function SearchCtrl($scope, $timeout, Content) {
//	$scope.searchExpression = "";
//	var cancel = null;
//	$scope.$watch("searchExpression", function (searchExpression) {
//		cancel && $timeout.cancel(cancel);
//		cancel = $timeout(function () {
//			$scope.search(searchExpression + "*");
//		}, 500);
//	});
//	$scope.clear = function () {
//		$scope.hits = [];
//		$scope.searchExpression = "";
//	};
//	$scope.search = function (searchExpression) {
//		if (!searchExpression || searchExpression == "*") {
//			return $scope.clear();
//		}
//		$scope.searching = true;
//		Content.search({ q: searchExpression, take: 20, selected: $scope.Context.CurrentItem.Path, pages: true }, function (data) {
//			$scope.hits = data.Hits;
//			$scope.searching = false;
//		});
//	}
//}

function NavigationCtrl($scope, Content, ContextMenuFactory, Eventually) {
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

	$scope.$watch("Context.User.PreferredView", function (view) {
		$scope.viewPreference = view == 0
			? "draft"
			: "published";
	});

	$scope.ContextMenu = new ContextMenuFactory($scope);
}

function TrunkCtrl($scope, $rootScope, Content, SortHelperFactory) {
	$scope.$watch("Context.Content", function (content) {
		$scope.node = content;
		if (content) {
			console.log("selecting", $scope.Context.SelectedNode);
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
		console.log("moved", content);
	});
	$scope.sort = new SortHelperFactory($scope, Content);
	$scope.parts = {
		show: function (node) {
			node.Loading = true;
			Content.children({ selected: node.Current.Path, pages: false }, function (data) {
				var zones = {};
				for (var i in data.Children) {
					var part = data.Children[i];
					var zone = zones[part.Current.ZoneName];
					if (!zone)
						zones[part.Current.ZoneName] = zone = [];
					zone.push(part);
				}

				node.Parts = [];
				for (var zone in zones) {
					if (!zone)
						continue;
					var child = {
						Current: { Title: zone, IconClass: "n2-icon-columns silver" },
						HasChildren: true,
						Children: zones[zone]
					}
					node.Parts.push(child);
				}

				console.log(node);
				delete node.Loading;
			});
		},
		hide: function (node) {
			delete node.Parts;
		}
	}
}

function BranchCtrl($scope, Content, SortHelperFactory) {
	$scope.node = $scope.child;
	$scope.toggle = function (node) {
		if (!node.Expanded && !node.Children.length) {
			Content.loadChildren(node);
		}
		node.Expanded = !node.Expanded;
	};

	$scope.sort = new SortHelperFactory($scope, Content);
}

function PageActionBarCtrl($scope, $rootScope, Security) {
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

	$scope.isDisplayable = function (item) {
		if ($scope.Context.CurrentItem && !Security.permissions.is(item.Current.RequiredPermission, $scope.Context.CurrentItem.MaximumPermission)) {
			//console.log("unauthorized", item);
			return false;
		}
		if (item.Current.HiddenBy) {
			//console.log(item.Current.Title, "hidden by", item.Current.HiddenBy, item);
			return !$scope.isFlagged(item.Current.HiddenBy);
		}
		if (item.Current.DisplayedBy) {
			//console.log(item.Current.Title, "displayed by", item.Current.DisplayedBy, $scope.isFlagged(item.Current.DisplayedBy), item);
			return $scope.isFlagged(item.Current.DisplayedBy);
		}
		return true;
	};
}

function PageActionCtrl($scope, Content) {
	$scope.dispose = function () {
		console.log("disposing", $scope.Context.CurrentItem);
		Content.delete({ selected: $scope.Context.CurrentItem.Path }, function () {
			$scope.reloadChildren(getParentPath($scope.Context.CurrentItem.Path));
			console.log("disposed");
		});
	}
}

function PreviewCtrl($scope, $rootScope) {
	$scope.frameLoaded = function (e) {
		try {
			var loco = e.target.contentWindow.location;
			$scope.$emit("preiewloaded", { path: loco.pathname, query: loco.search, url: loco.toString() });
		} catch (ex) {
			console.log("frame access exception", ex);
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
		console.log("toggleInfo", $scope.showInfo);
		$scope.$parent.showInfo = !$scope.$parent.showInfo;
	}
	$scope.definitions = {};
	Content.definitions({}, function (data) {
		for (var i in data.Definitions) {
			$scope.definitions[data.Definitions[i].TypeName] = data.Definitions[i];
		}
		console.log("got definition", $scope.definitions);
	});
}

function PagePublishCtrl($scope, $rootScope, $modal, Content) {
	$rootScope.$on("preiewloaded", function (scope, e) {
		
	});
	
	$scope.publish = function () {
		Content.publish({ selected: $scope.Context.CurrentItem.Path, versionIndex: $scope.Context.CurrentItem.VersionIndex }, function (result) {
			console.log("published", $scope.Context.CurrentItem, result);
			window.frames.preview.window.location = result.Current.PreviewUrl;
		});
	};
	$scope.schedule = function () {
		console.log("schedule");
		//$scope.showDatePicker = true;
		//Content.schedule({ selected: $scope.Context.CurrentItem.Path, versionIndex: $scope.Context.CurrentItem.VersionIndex, publishDate: '2013-07-01' });

	};
	$scope.unpublish = function () {
		console.log("unpublish");
		Content.unpublish({ selected: $scope.Context.CurrentItem.Path });
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

			console.log("scheduling", $scope.schedule);

			Content.schedule({ selected: $scope.Context.CurrentItem.Path, versionIndex: $scope.Context.CurrentItem.VersionIndex, publishDate: date });
		}
	};
}

function FrameActionCtrl($scope, $rootScope, FrameManipulatorFactory) {
	$scope.$parent.manipulator = new FrameManipulatorFactory($scope);
	$rootScope.$on("contextchanged", function (scope, e) {
		$scope.$parent.action = null;
		if ($scope.isFlagged("Management")) {
			var actions = $scope.manipulator.getFrameActions();
			if (actions && actions.length) {
				$scope.$parent.manipulator.hideToolbar();
				$scope.$parent.action = actions[0];
			}
		}
		console.log("ACTIONS", $scope.action);
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
