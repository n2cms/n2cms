$(function () {
    var loader = new N2.slidingLoader(".sliding-loader", { loaderImage: "Resources/img/sliding-loader.png" });

    loader.start();

    setTimeout(function () {
        loader.stop();
    }, 5000);

    var sidebarSplitter = new N2.splitter("#secondary-area", "#main-area");
});