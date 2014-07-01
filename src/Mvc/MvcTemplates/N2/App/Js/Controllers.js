(function (n2Module) {
	n2Module.value('$strapConfig', {
		datepicker: {
			language: 'en',
			format: 'M d, yyyy'
		}
	});
})(angular.module('n2', ['n2.directives', 'n2.services', 'n2.localization', 'ui', '$strap.directives', "ngRoute"], function ($routeProvider, $locationProvider) {
    if (history.pushState) {
        $locationProvider.html5Mode(true);
        $locationProvider.hashPrefix("!");
    }
	$routeProvider.otherwise({
	    templateUrl: "App/Partials/Framework.html",
	    controller: "ManagementCtrl",
	    reloadOnSearch: false
	});
}))

function findBranch(node, selectedPath) {
	if (!node)
		return null;
	if (node.Current.Path == selectedPath) {
		return [node];
	}
	if (selectedPath.indexOf(node.Current.Path) < 0) {
		return null;
	}
	
	for (var i in node.Children) {
		var n = findBranch(node.Children[i], selectedPath);
		if (n) {
			n.push(node);
			return n;
		}
	}
	return null;
}

function findNodeRecursive(node, selectedPath) {
	if (!node)
		return null;
	if (node.Current.Path == selectedPath) {
		return node;
	}
	if (selectedPath.indexOf(node.Current.Path) < 0) {
		return null;
	}

	for (var i in node.Children) {
		var n = findNodeRecursive(node.Children[i], selectedPath);
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

function Uri(uri) {
	this.uri = uri;
	this.appendQuery = function(key, value) {
		if (uri.indexOf("?") >= 0)
			this.uri += "&" + key + "=" + value;
		else
			this.uri += "?" + key + "=" + value;
		return this;
	};
	this.toString = function() {
		return this.uri;
	};
};

function ManagementCtrl($scope, $window, $timeout, $interpolate, $location, Context, Content, Profile, Security, FrameContext, Translate, Eventually, LocationKeeper) {
	$scope.Content = Content;
	$scope.Security = Security;

	$scope.appendPreviewOptions = function(url) {
		if (url == "Empty.aspx")
			return url;

		for (var key in $scope.Context.PreviewQueries) {
			url = $scope.appendQuery(url, key, $scope.Context.PreviewQueries[key]);
		}

		return url;
	};

	$scope.setPreviewQuery = function(key, value) {
		if (value)
			$scope.Context.PreviewQueries[key] = value;
		else
			delete $scope.Context.PreviewQueries[key];
	};

	$scope.appendQuery = function(url, key, value) {
		if (!url) return url;

		var hashIndex = url.indexOf("#");
		if (hashIndex >= 0)
			return $scope.appendQuery(url.substr(0, hashIndex), key, value) + url.substr(hashIndex);

		var keyValue = key + (value ? ("=" + value) : "");

		var re = new RegExp("([?|&])" + key + "=.*?(&|$)", "i");
		if (url.match(re))
			return url.replace(re, '$1' + keyValue + '$2');
		else
			return url + (url.indexOf("?") < 0 ? "?" : "&") + keyValue;
	};

	$scope.appendSelection = function(url, appendVersionIndex) {
		var ctx = $scope.Context;
		if (!ctx.CurrentItem)
			return url;
		url = $scope.appendQuery(url, ctx.Paths.SelectedQueryKey + "=" + ctx.CurrentItem.Path + "&" + ctx.Paths.ItemQueryKey + "=" + ctx.CurrentItem.ID);
		if (appendVersionIndex)
			url += "&n2versionIndex=" + ctx.CurrentItem.VersionIndex;
		return url;
	};

	$scope.previewUrl = function(url) {
		if (window.frames.preview)
			window.frames.preview.window.location = $scope.appendPreviewOptions(url) || "Empty.aspx";
	};

	decorate(FrameContext, "refresh", function (ctx) {
		// legacy refresh call from frame
	    if (ctx.force) {
			$scope.reloadNode(ctx.path);
			$scope.reloadChildren(ctx.path);
			if (ctx.previewUrl) {
				$scope.previewUrl(ctx.previewUrl);
				return;
			}
		}

		if (ctx.mode && ctx.mode.indexOf('DragDrop') >= 0)
			// the context will be reoloaded anyway due to PreviewUrl != url with edit=drag
			return;

		if (!findNodeRecursive($scope.Context.Content, ctx.path)) {
			$scope.reloadChildren(getParentPath(ctx.path), /*callback*/function () {
			    $scope.expandTo(ctx.path, /*select*/true);
			}, /*pathNotFound*/function () {
				$scope.reloadTree(/*selectedPath*/ctx.path);
			});
		} else if (ctx.force) {
		    $scope.expandTo(ctx.path, /*select*/true);
		}
	});

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
		PreviewQueries: {},
		User: {
			Settings: {}
		}
	};

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

	$scope.watchChanges = function(watchExpression, listener, objectEquality) {
		var firstTime = true;
		$scope.$watch(watchExpression, function() {
			if (firstTime)
				firstTime = false;
			else
				listener.apply($scope, arguments);
		}, objectEquality);
	};

	var query = $location.search();
	Context.full(query, function (i) {
		$scope.Context.Partials.Management = "App/Partials/Management.html";
		Content.paths = i.Interface.Paths;
		translateMenuRecursive(i.Interface.MainMenu);
		translateMenuRecursive(i.Interface.ActionMenu);
		translateMenuRecursive(i.Interface.ContextMenu);
		angular.extend($scope.Context, i.Interface);
		angular.extend($scope.Context, i.Context);

		if (query.mode == "Organize")
			$scope.Context.Paths.PreviewUrl = $scope.appendQuery($scope.Context.Paths.PreviewUrl, "edit", "drag");

		$scope.watchChanges("Context.User", function (user) {
			Eventually(function () {
				if (user.$saved){
					delete user.$saved;
					return;
				}

				Profile.save({}, user, function (data) {
				});

			}, 10000);
		}, true);
		$scope.saveUserSettings = function () {
			$scope.Context.User.$saved = true;
			Profile.save({}, $scope.Context.User, function (data) {
			});
		}
	});

	$scope.refreshContext = function(node, versionIndex, keepFlags, callback) {
		Context.get(Content.applySelection({ view: $scope.Context.User.Settings.ViewPreference, n2versionIndex: versionIndex }, node.Current), function(ctx) {
			//console.log("select -> contextchanged", node, versionIndex, ctx);
			if (keepFlags)
				angular.extend($scope.Context, ctx, { Flags: $scope.Context.Flags });
			else
				angular.extend($scope.Context, ctx);
			callback && callback(ctx);
			$scope.$emit("contextchanged", $scope.Context);
		});
	};

	$scope.expandTo = function (nodeOrPath, select) {
	    var path = typeof nodeOrPath == "string" ? nodeOrPath : nodeOrPath && nodeOrPath.Current && nodeOrPath.Current.Path;
		if (!path)
			return;
		var branch = findBranch($scope.Context.Content, path);
		for (var i in branch) {
			if (i == 0)
				$scope.Context.SelectedNode = branch[0];
			else
				branch[i].Expanded = true;
		}
	}

	$scope.reloadTree = function (selectedPath) {
		Content.branch(Content.applySelection({}, selectedPath), function (data) {
			$scope.Context.Content = data.Branch;
			$scope.select(selectedPath);
		});
	}

	$scope.select = function (nodeOrPath, versionIndex, keepFlags, forceContextRefresh, preventReload, disregardNodeUrl) {
	    if (typeof nodeOrPath == "string") {
			var path = nodeOrPath;
			var node = findNodeRecursive($scope.Context.Content, path);
			if (!node) {
				var parentNode = findNodeRecursive($scope.Context.Content, getParentPath(path));
				if (!preventReload && parentNode) {
					$scope.reloadChildren(parentNode, function() {
						// this is meant to refresh an item with changed path
						$scope.select(path, versionIndex, keepFlags, forceContextRefresh, /*preventReload*/true, /*disregardNodeUrl*/true);
					});
				}
			} else
				return $scope.select(node, versionIndex, keepFlags, forceContextRefresh, preventReload, disregardNodeUrl);
		} else if (typeof nodeOrPath == "object") {
			var node = nodeOrPath;
			$scope.Context.SelectedNode = node;
			if (!node)
				return false;

			if (!forceContextRefresh) {
				if ($scope.Context.AppliesTo == node.Current.PreviewUrl) {
					//console.log("exiting due to same", node.Current.PreviewUrl);
					return true;
				}
			}
			if (!disregardNodeUrl) {
				//console.log("setting appliesTo (1)", node.Current.PreviewUrl);
				$scope.Context.AppliesTo = node.Current.PreviewUrl;
			}

			$timeout(function() {
				$scope.refreshContext(node, versionIndex, keepFlags)
			}, 200);
			return true;
		}
	};

	$scope.reloadChildren = function(parentPathOrNode, callback, pathNotFound) {
		var node = typeof parentPathOrNode == "string"
			? findNodeRecursive($scope.Context.Content, parentPathOrNode)
			: parentPathOrNode;

		if (node)
		    Content.loadChildren(node, callback);
		else if (pathNotFound)
		    pathNotFound(parentPathOrNode);
	};

	$scope.reloadNode = function (pathOrNode, callback) {
		var node = typeof pathOrNode == "string"
			? findNodeRecursive($scope.Context.Content, pathOrNode)
			: pathOrNode;

		Content.reload(node, function(node) {
			callback && callback(node);
		});
	};

	$scope.isFlagged = function (flag) {
		return jQuery.inArray(flag, $scope.Context.Flags) >= 0;
	};
	
	var viewExpression = /[?&]view=[^?&]*/;
	$scope.$on("preiewloaded", function (scope, e) {
		if ($scope.Context.AppliesTo == (e.path + e.query)) {
			//console.log("bailing out", $scope.Context.AppliesTo, "==", (e.path + e.query));
			return;
		}
		//console.log("setting appliesTo (2)", e.path + e.query);
		$scope.Context.AppliesTo = e.path + e.query;

		$timeout(function () {
			Context.get({ selectedUrl: e.path + e.query }, function (ctx) {
				//console.log("previewloaded -> contextchanged", e, ctx);
				angular.extend($scope.Context, ctx);
				$scope.$emit("contextchanged", $scope.Context);
			});
		}, 200);
	});

	$scope.evaluateExpression = function (expr) {
		return expr && $interpolate(expr)($scope);
	};

	$scope.isDisplayable = function (item) {
	    if (item.IsHidden) {
	        return false;
	    }
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

function ManagementConfirmCtrl($rootScope, $scope) {
    $scope.confirm = function () {
        $scope.settings.confirmed && $scope.settings.confirmed();
        delete $scope.settings;
    }
    $scope.close = function () {
        $scope.settings.cancelled && $scope.settings.cancelled();
        delete $scope.settings;
    }
    $rootScope.$on("confirm", function (e, settings) {
        $scope.settings = settings;
        if (!$scope.$$phase) {
            // specific sceanrio: move
            $scope.$digest();
        }
    });
}

function NavigationCtrl($scope, ContextMenuFactory) {
	$scope.ContextMenu = new ContextMenuFactory($scope);
}

function ScopeHandler($scope, Content) {
	this.from = false;
	this.here = function (node) {
		this.from = true;
		$scope.node = node;
		$scope.Context.User.Settings.Scope = node.Current.Path;
		$scope.saveUserSettings();
	};
	this.clear = function () {
		$scope.node = $scope.Context.Content;
		delete $scope.Context.User.Settings.Scope;
		$scope.saveUserSettings();
		this.from = false;
	};
	if ($scope.Context.User.Settings.Scope) {
		var t = this;
		Content.tree(Content.applySelection({}, $scope.Context.User.Settings.Scope), function (data) {
			$scope.node = data.Tree;
			t.from = true;
		});
	}
	return this;
}

function TrunkCtrl($scope, $rootScope, Content, SortHelperFactory) {
	$scope.$watch("Context.Content", function (content) {
		$scope.node = content;
	});
	$rootScope.$on("contextchanged", function (scope, ctx) {
		if (ctx.Actions.refresh) {
			$scope.reloadChildren(ctx.Actions.refresh, function () {
				$scope.select(ctx.CurrentItem.Path, ctx.CurrentItem.VersionIndex, /*keepFlags*/true);
				$scope.Context.SelectedNode = findNodeRecursive($scope.Context.Content, ctx.CurrentItem.Path);
			});
		}
		else if (ctx.CurrentItem)
			$scope.Context.SelectedNode = findNodeRecursive($scope.Context.Content, ctx.CurrentItem.Path);
		else
			$scope.Context.SelectedNode = null;
	});
	$scope.nodeClicked = function (node) {
		$scope.Context.User.Settings.Selected = node.Current.Path;
		$scope.select(node);
	}
	$scope.toggle = function (node) {
		if (!node.Expanded && !node.Children.length) {
			Content.loadChildren(node);
		}
		node.Expanded = !node.Expanded;
	};
	$scope.loadRemaining = function (node) {
		node.Loading = true;
		Content.children(Content.applySelection({ skip: node.Children.length }, node.Current), function (data) {
			node.Children.length--;
			for (var i in data.Children)
				node.Children.push(data.Children[i]);
			node.Loading = false;
			node.IsPaged = false;
		});
	}
	$scope.sort = new SortHelperFactory($scope, Content);
	$scope.parts = {
		show: function(node) {
			node.Loading = true;
			Content.children(Content.applySelection({ pages: false }, node.Current), function(data) {
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
						Current: { Title: zone, IconClass: "fa fa-columns silver", MetaInformation: [] },
						HasChildren: true,
						Children: zones[zone]
					};
					node.Parts.push(child);
				}
				node.Expanded = true;
				delete node.Loading;
			});
		},
		hide: function(node) {
			delete node.Parts;
			if (!node.HasChildren)
				node.Expanded = false;
		}
	};
	$scope.scope = new ScopeHandler($scope, Content);
}

function BranchCtrl($scope, Content, Translate, SortHelperFactory) {
	$scope.node = $scope.child;
	$scope.sort = new SortHelperFactory($scope, Content);
	$scope.tags = [];
	if ($scope.node.Current) {
		var mi = $scope.node.Current.MetaInformation;
		if (mi) {
			if (mi.authority) $scope.tags.push({ ToolTip: Translate("branch.tags.authority", "Site: ") + (mi.authority.ToolTip || " (*)"), IconClass: "fa fa-home", Url: "#" });
			if (mi.hidden) $scope.tags.push({ ToolTip: Translate("branch.tags.hidden", "Hidden"), IconClass: "fa fa-eraser", Url: "#" });
			if (mi.language) $scope.tags.push({ ToolTip: Translate("branch.tags.language", "Language: ") + mi.language.Text, IconClass: "fa fa-globe", Url: "#" });
			if (mi.locked) $scope.tags.push({ ToolTip: Translate("branch.tags.locked", "Access restrictions"), IconClass: "fa fa-lock", Url: "#" });
			if (mi.zone) $scope.tags.push({ ToolTip: Translate("branch.tags.zone", "In zone: ") + mi.zone.Text, IconClass: "fa fa-columns", Url: "#" });
			if (mi.draft) $scope.tags.push({ ToolTip: Translate("branch.tags.draft", "Has draft: ") + mi.draft.ToolTip, IconClass: "fa fa-circle-o", Url: "#" });
			if (mi.system) $scope.tags.push({ ToolTip: mi.system.ToolTip, IconClass: "fa fa-qrcode", Url: "#" });
			if ($scope.node.Current.State == Content.states.Unpublished) $scope.tags.push({ ToolTip: Translate("branch.tags.unpublished", "Unpublished"), IconClass: "fa fa-stop", Url: "#" });
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
		$scope.Context.User.Settings.ViewPreference = viewPreference;
		$scope.saveUserSettings();
	};
	$scope.$watch("Context.User.Settings.ViewPreference", function (viewPreference, previousPreference) {
		$scope.setPreviewQuery("view", viewPreference);
		var existingIndex = jQuery.inArray("View" + previousPreference, $scope.Context.Flags);
		if (existingIndex >= 0)
			$scope.Context.Flags.splice(existingIndex, 1);
		$scope.Context.Flags.push("View" + viewPreference);
	});
	$rootScope.$on("contextchanged", function (scope, ctx) {
		ctx.Flags.push("View" + ctx.User.Settings.ViewPreference);
	});
}

function MenuNodeLastChildCtrl($scope, $timeout) {
    function replace(item, replacement) {
        var r = replacement.Current;
        var copy = angular.copy(item.Current);
        item.Current = angular.extend(copy, { Description: r.Title, Url: r.Url, Target: r.Target, IconClass: r.IconClass, ToolTip: r.ToolTip, IconUrl: r.IconUrl, RequiredPermission: r.RequiredPermission, ClientAction: r.ClientAction });
    }

    $scope.$watch("item", function (item) {
        if (!item.Children || !item.Children.length) {
            item.IsHidden = true;
            return;
        }
        var preferredItem = item.Children[0];
        var preferredEditAction = $scope.Context.User.Settings.PreferredEditAction;
        if (preferredEditAction) {
        	for (var i in item.Children) {
        		if (item.Children[i].Current.Name == preferredEditAction) {
        			preferredItem = item.Children[i];
        		}
        	}
        }
        replace(item, preferredItem);
    });
    $scope.$on("nodeclicked", function (scope, node) {
    	replace($scope.item, node);
    	$scope.Context.User.Settings.PreferredEditAction = node.Current.Name;
    	$scope.saveUserSettings();
    });
}

function PageActionCtrl($scope, Content) {
	$scope.dispose = function() {
		Content.remove(Content.applySelection({}, node.Current), function() {
			$scope.reloadChildren(getParentPath($scope.Context.CurrentItem.Path));
		});
	};
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
	$scope.loadDefinitions = function(node) {
		node.Selected = node.Current.Path;
		node.Loading = true;
		Content.definitions(Content.applySelection({}, $scope.Context.CurrentItem), function(data) {
			node.Loading = false;
			node.Children = data.Definitions;
		});
	};
}

function LanguageCtrl($scope, Content) {
	$scope.loadLanguages = function(node) {
		node.Selected = node.Current.Path;
		node.Loading = true;
		Content.translations(Content.applySelection({}, $scope.Context.CurrentItem), function(data) {
			node.Loading = false;
			node.Children = data.Translations;
		});
	};
}

function VersionsCtrl($scope, Content) {
	$scope.loadVersions = function(node) {
		$scope.Selected = node.Current.Path;
		node.Loading = true;
		Content.versions(Content.applySelection({}, $scope.Context.CurrentItem), function(data) {
			node.Loading = false;
			node.Children = data.Versions;
		});
	};
}

function SearchCtrl($scope, $rootScope, Content, Eventually) {
    $scope.item.Children = [{}];

    $scope.$parent.toggleSearch = function () {
        $scope.$parent.search.show = !$scope.$parent.search.show;
        $scope.$parent.search.query = null;
    }

    $scope.$parent.search = {
        execute: function (searchQuery) {
            if (!searchQuery)
                return $scope.search.clear();
            else if (searchQuery == $scope.search.searching)
                return;

            $scope.search.searching = searchQuery;
            Content.search(Content.applySelection({ q: searchQuery, take: 20, pages: true }, $scope.Context.CurrentItem), function (data) {
                $scope.search.hits = data.Hits;
                $scope.item.Expanded = true;
                $scope.search.searching = "";
            });
        },
        clear: function () {
            $scope.search.query = "";
            $scope.search.searching = "";
            $scope.search.hits = null;
            delete $scope.item.Expanded;
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
}

function PageInfoCtrl($scope, Content) {
	$scope.exctractLanguage = function(language) {
		return language && language.replace(/[(].*?[)]/, "");
	};
	$scope.$parent.showInfo = $scope.Context.User.Settings.ShowInfo;
	$scope.toggleInfo = function() {
		$scope.$parent.showInfo = !$scope.$parent.showInfo;
		$scope.Context.User.Settings.ShowInfo = $scope.$parent.showInfo;
	};
	$scope.definitions = {};
	Content.definitions({}, function (data) {
		for (var i in data.Definitions) {
			$scope.definitions[data.Definitions[i].TypeName] = data.Definitions[i];
		}
	});
}

function PagePublishCtrl($scope, $rootScope, $modal, Content, Confirm, Translate) {
	$scope.publish = function () {
		Content.publish({ selected: $scope.Context.CurrentItem.Path, n2versionIndex: $scope.Context.CurrentItem.VersionIndex }, function (result) {
			$scope.previewUrl(result.Current.PreviewUrl);

			$scope.reloadNode(result.Current.Path, $scope.refreshContext);
		});
	};
	$scope.unpublish = function () {
	    var settings = {
	        title: Translate("confirm.unpublish.title"),
	        item: $scope.Context.CurrentItem,
	        template: "<b class='ico' ng-show='settings.item.IconClass || settings.item.IconUrl' ng-class='settings.item.IconClass' x-background-image='settings.item.IconUrl'></b> {{settings.item.Title}}",
	        confirmed: function () {
	            Content.unpublish(Content.applySelection({}, $scope.Context.CurrentItem), function (result) {
	                $scope.previewUrl(result.Current.PreviewUrl);

	                $scope.reloadNode(result.Current.Path, $scope.refreshContext);
	            })
	        }
	    };
	    if ($scope.Context.CurrentItem.MetaInformation.authority) {
	        settings.template = "<div class='alert alert-warnig'>{{settings.warning}}</div>" + settings.template;
	        settings.warning = Translate("confirm.unpublish.startpagewarning");
	    }
	    Confirm(settings);
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

			Content.schedule(Content.applySelection({ n2versionIndex: $scope.Context.CurrentItem.VersionIndex, publishDate: date }, $scope.Context.CurrentItem));
		}
	};
}

function FrameActionCtrl($scope, $rootScope, $timeout, FrameManipulator) {
	$scope.execute = function (action) {
		//FrameManipulator.click(action.Current.Selector);
	}
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
