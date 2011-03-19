(function ($) {
	$.fn.n2autocomplete = function (options) {
		function filter(term, filterText, children){
		    filterText = filterText || "";
			var filtered = new Array();
			for(var i in children) {
				var name = children[i].name.match(/[^/]+$/).toString();
				if(name.toLowerCase().indexOf(filterText.toLowerCase()) >= 0) {
					filtered.push(term + children[i].name);
				}
			}
			return filtered;
		};
		
		var cache = {};
		
		function source(request, response) {
		    var term = request.term.replace(/[/][^/]+$/, "/") ||  "/";
		    var filterText = (request.term.match(/[^/]+$/) || "").toString();
		    if (term in cache) {
		    	response(filter(term, filterText, cache[term]));
		    } else {
				var lastXhr = $.getJSON("children.n2.ashx", { action:"children", path:term, filter:options.filter }, function (data, status, xhr) {
		    		cache[term] = data.children;
					if (xhr === lastXhr) {
		    			response(filter(term, filterText, data.children));
		    		}
				});
			}
		};

		this.autocomplete($.extend({ source:source }, options));
		return this;
	}
})(jQuery);