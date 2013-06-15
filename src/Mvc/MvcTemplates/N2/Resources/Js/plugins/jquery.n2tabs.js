//(function ($) {
//	// initializes elements in query selection as tabs
//	var n2tabs = {};

//	$.fn.n2tabs = function (tabGroupName, initial, tabContainer) {
//		if (!this.length)
//			return;

//		if (!tabGroupName) tabGroupName = "tab";

//		// ensure each tab content has an id
//		this.each(function (i) {
//			if (!this.id) this.id = tabGroupName + i;
//		});

//		console.log("processing", tabGroupName, this.length, this);

//		var $tabContent = this.closest(".tab-content");
//		if (!$tabContent.length)
//			$tabContent = this.wrapAll("<div class='tab-content' />");

//		$tabContent.attr("data-tab-container", tabContainer);
//		$tabContent.css("border", "solid 1px red")

//		//var $tab = $("#" + tabGroupName);
//		//if (!$tab.length)
//		//	$tab = $("<ul id='" + tabGroupName + "' class='nav nav-tabs' />").insertBefore($tabContent);
//		//this.not(".tab-pane").each(function (i) {
//		//	var $li = $("<li><a href='#" + this.id + "'>" + (this.getAttribute("data-tab-text") || this.title) + "</a></li>").appendTo($tab);
//		//	if (i == 0) {
//		//		$li.addClass("active");
//		//		$(this).addClass("active");
//		//	}
//		//}).addClass("tab-pane");

//		//$tab.on("click", "a", function (e) {
//		//	e.preventDefault();
//		//	$(this).tab("show");
//		//});
//	}
//})(jQuery);


/*
* n2tabs 0.2 - Copyright (c) 2007 Cristian Libardo
*/

(function ($) {
	// initializes elements in query selection as tabs
	var n2tabs = {};

	$.fn.n2tabs = function (tabGroupName, initial, tabContainer) {
		if (this.length > 0) {
			if (!tabGroupName) tabGroupName = "tab";
			if (!tabContainer) tabContainer = n2tabs.createContainer(this.get(0));

			this.removeClass("tabPanel").addClass("tab-pane").wrapAll("<div class='tab-content' />");

			// ensure each tab content has an id
			this.each(function (i) {
				if (!this.id) this.id = tabGroupName + i;
				$("a[href='#" + this.id + "']").click(function () {
					n2tabs.show($(this.hash));
				});
				this.n2tab = { index: i,
					group: tabGroupName
				};
			});

			// ensure there's an initial tab
			if (!initial || initial == "") initial = this.get(0);

			var $current = this.filter(initial);
			if ($current.length == 0) {
				// try to select enclosing tab when nested tab is selected
				var $vertical = $(initial).parents(this.selector);
				$current = this.filter(function () { return $vertical.filter(this).length > 0 });
			}
			if ($current.length == 0)
				$current = $(this[0]);

			// store information about this tab group
			var tabSettings = {
				query: this,
				container: $(tabContainer),
				current: $current,
				tabs: new Array()
			};
			n2tabs.groups[tabGroupName] = tabSettings;

			n2tabs.buildTabs(tabSettings);

			//this.addClass("tabContentHidden");
			n2tabs.show(tabSettings.current);

			document.documentElement.scrollTop = 0;
		}

		return this;
	}

	// an array of tab groups (multiple tabs are supported)
	n2tabs.groups = new Array();

	// creates a tab container element
	n2tabs.createContainer = function (firstContents) {
		return $(firstContents).before("<ul class='nav nav-tabs'></ul>").prev().get(0);
	}

	// creates a tab element
	n2tabs.createTab = function (containerQuery, tabContents, index) {
		var li = "<li>";
		if (index == 0)
			li = "<li class='first'>";

		var a = "<a href='"
			+ (tabContents.getAttribute("data-tab-href") || ("#" + tabContents.id))
			+ "'>"
			+ (tabContents.getAttribute("data-tab-text") || tabContents.title)
			+ "</a>";
		containerQuery.append(li + a + "</li>");
		tabContents.title = "";

		if (tabContents.getAttribute("data-tab-selected"))
			return tabContents;
		return null;
	}

	// creates tab elements (ul:s and li:s) above the first tab content element
	n2tabs.buildTabs = function (tabSettings) {
		var lastIndex = tabSettings.query.length - 1;
		tabSettings.query.each(function (i) {
			var className = "tab";
			if (i == 0) className = className + " first";
			if (i == lastIndex) className = className + " last";
			var selectedTab = n2tabs.createTab(tabSettings.container, this, className);
			if (selectedTab) {
				tabSettings.current = $(selectedTab);
			}
		});
		$("a", tabSettings.container).each(function (i) {
			tabSettings.tabs[i] = $(this);
			if (this.hash) {
				$(this).click(function (e) {
					n2tabs.show($(this.hash), $(this));
				});
			}
		});
	}

	n2tabs.handlePostBack = function (tabId) {
		if (tabId && document.forms.length > 0) {
			var f = document.forms[0];
			var index = f.action.indexOf("#");
			if (index > 0)
				f.action = f.action.substr(0, index) + "#" + tabId;
			else
				f.action += "#" + tabId;
		}
	}

	// gets the settings for the first tab content in query selection
	$.fn.n2tab_settings = function () {
		var first = this.get(0).n2tab.group;
		return n2tabs.groups[first];
	}

	// gets the tab for the first tab content in query selection
	$.fn.n2tab_getTab = function () {
		var t = this.get(0).n2tab;
		return n2tabs.groups[t.group].tabs[t.index];
	}

	// show contents defined by the given expression
	n2tabs.show = function (contents, tab) {
		var tabSettings = contents.n2tab_settings();

		// show tab contents    
		tabSettings.current.addClass("inactive").removeClass("active");
		tabSettings.current = contents;
		contents.removeClass("inactive").addClass("active");

		// select tab
		if (!tab) tab = contents.n2tab_getTab();
		$(".active", tabSettings.container).removeClass("active");
		tab.blur().parent().addClass("active");

		// this prevents page from scrolling (stolen from jquery.tabs)
		var toShowId = contents.attr('id');
		contents.attr('id', '');
		setTimeout(function () {
			contents.attr('id', toShowId); // restore id
		}, 200);
		n2tabs.handlePostBack(toShowId);
	}
})(jQuery);