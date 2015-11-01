function openMediaSelectorPopup(popupUrl, tbId, popupOptions, preferredSize, availableExtensions) {
	var tb = document.getElementById(tbId);
	window.open(popupUrl
			+ '&tbid=' + tbId
			+ '&preferredSize=' + preferredSize
			+ '&selectedUrl=' + encodeURIComponent(tb.value)
			+ '&selectableExtensions=' + availableExtensions,
	null,
	popupOptions);
}