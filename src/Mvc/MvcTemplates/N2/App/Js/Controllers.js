angular.module('n2', ['n2.routes', 'n2.directives', 'n2.services', 'ui'], function () {
});

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

function reloadChildren($scope, Content, parentPath, selectedPath) {
	var node = findSelectedRecursive($scope.Interface.Content, parentPath);
	console.log("Reloading ", node);
	Content.loadChildren(node, function () {
		var selectedNode = findSelectedRecursive(node, selectedPath || parentPath);
		console.log("Reloaded", node, " selecting ", selectedNode);
		$scope.select(selectedNode);
	});
}

function ManagementCtrl($scope, $window, $timeout, $interpolate, Interface, Context, Content, Security, FrameContext) {
	$scope.Content = Content;
	$scope.Security = Security;

	decorate(FrameContext, "refresh", function (ctx) {
		if (ctx.force) {
			reloadChildren($scope, Content, ctx.path);
			if (ctx.previewUrl) {
				console.log("PREVIEWING ", ctx.previewUrl);
				$scope.previewUrl(ctx.previewUrl);
			} else if (!$scope.select(ctx.path)) {
				reloadChildren($scope, Content, getParentPath(ctx.path), ctx.path);
			}
		}
		else
			$scope.select(ctx.path);
	});

	var viewMatch = window.location.search.match(/[?&]view=([^?&]+)/);
	var selectedMatch = window.location.search.match(/[?&]selected=([^?&]+)/);
	$scope.Interface = Interface.get({
		view: viewMatch && viewMatch[1],
		selected: selectedMatch && selectedMatch[1]
	}, function (i) {
		console.log("Interface loaded", i);
		$scope.previewUrl(i.Paths.PreviewUrl);
	});
	$scope.Context = {
		CurrentItem: {
			PreviewUrl: "Empty.aspx"
		},
		SelectedNode: {
		},
		ContextMenu: {
		}
	}

	$scope.select = function (nodeOrPath) {
		console.log("selecting", typeof nodeOrPath, nodeOrPath);
		if (typeof nodeOrPath == "string") {
			var path = nodeOrPath;
			var node = findSelectedRecursive($scope.Interface.Content, path);
			if (node)
				return $scope.select(node);
			else
				return console.log("select: path not found", path) && false;
		} else if (typeof nodeOrPath == "object") {
			var node = nodeOrPath;
			$scope.Context.SelectedNode = node;
			if (!node)
				return false;
			$timeout(function () {
				Context.get({ selected: node.Current.Path, view: $scope.Interface.Paths.ViewPreference }, function (ctx) {
					console.log("Ctx reloaded", ctx, node);
					angular.extend($scope.Context, ctx);
				});
			}, 200);
			return true;
		}
	}

	$scope.isFlagged = function (flag) {
		return $scope.Context.Flags.indexOf(flag) >= 0;
	};

	var viewExpression = /[?&]view=[^?&]*/;
	$scope.$on("preiewloaded", function (scope, e) {
		if ($scope.Context.AppliesTo == e.url)
			return;
		$scope.Context.AppliesTo = e.url;

		$timeout(function () {
			Context.get({ selectedUrl: e.path + e.query }, function (ctx) {
				angular.extend($scope.Context, ctx);
				$scope.$broadcast("contextchanged", $scope.Context);
			});
		}, 200);
	});

	$scope.evaluateExpression = function (expr) {
		return expr && $interpolate(expr)($scope);
	};
}

function MainMenuCtrl($scope) {
	$scope.$watch("Interface.MainMenu", function (mainMenu) {
		$scope.menu = mainMenu;
	});
	$scope.$watch("Interface.User", function (user) {
		$scope.user = user;
	});
}

function SearchCtrl($scope, $timeout, Content) {
	$scope.searchExpression = "";
	var cancel = null;
	$scope.$watch("searchExpression", function (searchExpression) {
		cancel && $timeout.cancel(cancel);
		cancel = $timeout(function () {
			$scope.search(searchExpression + "*");
		}, 500);
	});
	$scope.clear = function () {
		$scope.hits = [];
		$scope.searchExpression = "";
	};
	$scope.search = function (searchExpression) {
		if (!searchExpression || searchExpression == "*") {
			return $scope.clear();
		}
		$scope.searching = true;
		Content.search({ q: searchExpression, take: 20, selected: $scope.Context.CurrentItem.Path, pages: true }, function (data) {
			$scope.hits = data.Hits;
			$scope.searching = false;
		});
	}
}

function NavigationCtrl($scope, ContextMenuFactory) {
	$scope.$watch("Interface.User.PreferredView", function (view) {
		$scope.viewPreference = view == 0
			? "draft"
			: "published";
	});

	$scope.ContextMenu = new ContextMenuFactory($scope);
}

function TrashCtrl($scope) {

}

function TrunkCtrl($scope, Content, SortHelperFactory) {
	$scope.$watch("Interface.Content", function (content) {
		$scope.node = content;
		if (content) {
			console.log("selecting", $scope.Context.SelectedNode);
			$scope.select(findSelectedRecursive(content, $scope.Interface.SelectedPath));
		}
	});
	$scope.$on("contextchanged", function (scope, ctx) {
		console.log("contextchanged", ctx);
		if (ctx.CurrentItem)
			$scope.Context.SelectedNode = findSelectedRecursive($scope.Interface.Content, ctx.CurrentItem.Path);
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
}

function BranchCtrl($scope, Content, SortHelperFactory) {
	$scope.node = $scope.child;
	$scope.toggle = function (node) {
		if (!node.Expanded && !node.Children.length) {
			Content.loadChildren(node);
		}
		node.Expanded = !node.Expanded;
	};
	//	function (node) {
	//	if (!node.Expanded && !node.Children.length) {
	//		node.Loading = true;
	//		Content.children({ selected: node.Current.Path }, function (data) {
	//			node.Children = data.Children;
	//			node.Loading = false;
	//			if (data.IsPaged)
	//				node.IsPaged = true;
	//		});
	//	}
	//	node.Expanded = !node.Expanded;
	//};
	$scope.sort = new SortHelperFactory($scope, Content);
}

function PageActionBarCtrl($scope, $rootScope, Security) {
	$scope.$watch("Interface.ActionMenu.Children", function (children) {
		var lefties = [];
		var righties = [];
		for (var i in children) {
			if (children[i].Current.Alignment == "Right")
				righties.push(children[i]);
			else
				lefties.push(children[i]);
		}
		$scope.navs =
			[
				{ Alignment: "Left", Items: lefties },
				{ Alignment: "Right", Items: righties },
			];
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
			reloadChildren($scope, Content, getParentPath($scope.Context.CurrentItem.Path));
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

	$rootScope.previewUrl = function (url) {
		console.log("Previewing ", url);
		window.frames.preview.window.location = url || "Empty.aspx";
	}
}

function AddCtrl($scope, Definitions) {
	$scope.loadDefinitions = function (node) {
		console.log("loading definitions for ", node);
		node.Selected = node.Current.Path;
		node.Loading = true;
		node.Children = Definitions.query({ selected: $scope.Context.CurrentItem.Path }, function () {
			node.Loading = false;
		});
	}
}

function LanguageCtrl($scope, Translations) {
	$scope.loadLanguages = function (node) {
		console.log("loading languages for ", node);
		node.Selected = node.Current.Path;
		node.Loading = true;
		node.Children = Translations.query({ selected: $scope.Context.CurrentItem.Path }, function () {
			node.Loading = false;
		});
	}
}

function VersionsCtrl($scope, Versions) {
	$scope.loadVersions = function (node) {
		console.log("loading versions for ", node);

		$scope.Selected = node.Current.Path;
		node.Loading = true;
		node.Children = Versions.query({ selected: $scope.Context.CurrentItem.Path }, function (versions) {
			node.Loading = false;
		});
	}
}

function PagePublishCtrl($scope, $rootScope, Content) {
	$rootScope.$on("preiewloaded", function (scope, e) {
		
	});

	$scope.publish = function () {
		console.log("publish", $scope.Context.CurrentItem);
		Content.publish({ selected: $scope.Context.CurrentItem.Path });
	};
	$scope.schedule = function () {
		console.log("schedule");
		Content.schedule({ selected: $scope.Context.CurrentItem.Path, publishDate: '2013-07-01' });
	};
	$scope.unpublish = function () {
		console.log("unpublish");
		Content.unpublish({ selected: $scope.Context.CurrentItem.Path });
	};
	$scope.toggleInfo = function () {
		console.log("toggleInfo", $scope.showInfo);
		$scope.$parent.showInfo = !$scope.$parent.showInfo;
	}
}

function FrameActionCtrl($scope, FrameManipulatorFactory) {
	$scope.$parent.manipulator = new FrameManipulatorFactory($scope);
	$scope.$on("contextchanged", function (scope, e) {
		delete $scope.$parent.action;
		if (!$scope.isFlagged("Management"))
			return;

		var actions = window.frames.preview && window.frames.preview.frameActions;
		if (actions) {
			$scope.$parent.manipulator.hideToolbar();
			$scope.$parent.action = actions[0];
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
