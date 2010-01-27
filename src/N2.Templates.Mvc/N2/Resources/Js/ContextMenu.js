n2nav.currentUrl = "/";
n2nav.memorize = function(selected,action){
    window.n2ctx.memorize(selected, action);
}
n2nav.setupToolbar = function(path, url) {
	n2ctx.update({ path: path, previewUrl: url });
	path = encodeURIComponent(path);
	for (var i = 0; i < navigationPlugIns.length; i++) {
		var memory = window.n2ctx.getMemory();
		var action = window.n2ctx.getAction();
		var a = document.getElementById(navigationPlugIns[i].linkId);
		a.href = navigationPlugIns[i].urlFormat
			.replace("{selected}", path)
			.replace("{memory}", memory)
			.replace("{action}", action);
	}
}
