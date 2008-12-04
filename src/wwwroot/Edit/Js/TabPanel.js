/*
 * n2tabs 0.2 - Copyright (c) 2007 Cristian Libardo
 */

// initializes elements in query selection as tabs
$.fn.n2tabs = function(tabGroupName, initial, tabContainer){
    if(this.length>0){
        if(!tabGroupName) tabGroupName = "tab";
        if(!tabContainer) tabContainer = this.n2tabs_createContainer(this.get(0));
        
        // ensure each tab content has an id
        this.each(function(i){
            if(!this.id) this.id = tabGroupName + i;
            this.n2tab = {  index: i,
                            group: tabGroupName};
        });
        
        // ensure there's an initial tab
        if(!initial || initial == "") initial = this.get(0);

        // store information about this tab group
        var tabSettings = {
            query: this,
            container: $(tabContainer),
            current: $(initial),
            tabs: new Array()
        };
        this.n2tabs_groups[tabGroupName] = tabSettings;
        
        this.n2tabs_buildTabs(tabSettings);

        this.addClass("tabContentHidden");
        this.n2tabs_show(tabSettings.current);
    }
    return this;
}

// an array of tab groups (multiple tabs are supported)
$.fn.n2tabs_groups = new Array();

// creates a tab container element
$.fn.n2tabs_createContainer = function(firstContents){
    return $(firstContents).before("<ul class='tabs'></ul>").prev().get(0);
}

// creates a tab element
$.fn.n2tabs_createTab = function(containerQuery, tabContents, index){
	var li = "<li>";
	if(index == 0)
		li = "<li class='first'>";
    containerQuery.append(li + "<a href='#" + tabContents.id + "'>" + tabContents.title + "</a></li>");
	tabContents.title = "";
}

// creates tab elements (ul:s and li:s) above the first tab content element
$.fn.n2tabs_buildTabs = function(tabSettings){
	var lastIndex = tabSettings.query.length - 1;
    tabSettings.query.each( function(i){
		var className = "tab";
		if(i == 0) className = className  + " first";
		if(i == lastIndex) className = className  + " last";
        tabSettings.container.n2tabs_createTab(tabSettings.container, this, className);
    });
    $("a", tabSettings.container).each(function(i){
        tabSettings.tabs[i] = $(this);
    }).click(function(){
        if(this.hash!=location.hash)
			$.fn.n2tabs_show($(this.hash),$(this));
		else 
			return false;
    });
}

$.fn.n2tabs_handlePostBack = function(tabId){
    if(tabId && document.forms.length>0){
        var f = document.forms[0];
        var index = f.action.indexOf("#");
        if(index>0)
            f.action = f.action.substr(0,index) + "#" + tabId;
        else
            f.action += "#" + tabId;
    }
}

// gets the settings for the first tab content in query selection
$.fn.n2tab_settings = function(){
    return this.n2tabs_groups[this.get(0).n2tab.group];
}

// gets the tab for the first tab content in query selection
$.fn.n2tab_getTab = function(){
    var t = this.get(0).n2tab;
    return this.n2tabs_groups[t.group].tabs[t.index];
}

// show contents defined by the given expression
$.fn.n2tabs_show = function(contents, tab){
    var tabSettings = contents.n2tab_settings();

    // show tab contents    
    tabSettings.current.addClass("tabContentHidden");
    tabSettings.current = contents;
    contents.removeClass("tabContentHidden");
    
    // select tab
    if(!tab) tab = contents.n2tab_getTab();
    $(".selected", tabSettings.container).removeClass("selected");
    tab.blur().parent().addClass("selected");

    // this prevents page from scrolling (stolen from jquery.tabs)
    var toShowId = contents.attr('id');
    contents.attr('id', '');
    setTimeout(function() {
        contents.attr('id', toShowId); // restore id
    }, 200);
    this.n2tabs_handlePostBack(toShowId);
}