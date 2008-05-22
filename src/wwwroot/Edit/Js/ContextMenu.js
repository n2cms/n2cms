n2nav.currentUrl = "/";
n2nav.memorize = function(selected,action){
    window.top.n2.memorize(selected,action);
}
n2nav.setupToolbar = function(path){
	if(window.top.n2){
	    window.top.n2.setupToolbar(path);
		path = encodeURIComponent(path);
		for(var i=0; i<navigationPlugIns.length; i++){
			var memory = window.top.n2.getMemory();
			var action = window.top.n2.getAction();
			var a = document.getElementById(navigationPlugIns[i].linkId);
			a.href = navigationPlugIns[i].urlFormat
				.replace("{selected}", path)
				.replace("{memory}", memory)
				.replace("{action}", action);
		}
	}
}
