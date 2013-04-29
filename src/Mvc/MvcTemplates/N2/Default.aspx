<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Management.Default" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<!DOCTYPE html>
<!--[if lt IE 7]>      <html class="no-js lt-ie9 lt-ie8 lt-ie7"> <![endif]-->
<!--[if IE 7]>         <html class="no-js lt-ie9 lt-ie8"> <![endif]-->
<!--[if IE 8]>         <html class="no-js lt-ie9"> <![endif]-->
<!--[if gt IE 8]><!-->
<html class="no-js" ng-app="n2">
<!--<![endif]-->
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
    <title>N2 Management</title>
    <meta name="viewport" content="width=device-width">

    <%--<link rel="stylesheet" href="redesign/css/normalize.min.css">--%>
    <%--<link rel="stylesheet" type="text/css" href="http://yui.yahooapis.com/2.9.0/build/reset/reset-min.css">--%>
    <link rel="stylesheet" href="redesign/css/n2.css">

    <script src="redesign/js/vendor/modernizr-2.6.2.min.js"></script>
</head>
<body ng-controller="ManagementCtrl">
    <div id="top-area">
        <%--<div ng-include src="'App/Partials/UserTools.html'"></div>--%>
        <div ng-include src="'App/Partials/MainMenu.html'"></div>
    </div>
    <div class="sliding-loader loader"></div>
	<div id="main-wrapper">
        <div id="secondary-area">
            <div ng-include src="'App/Partials/PageTree.html'" ng-controller="NavigationCtrl"></div>
            <div class="dragbar"></div>
        </div>
        <div id="main-area"><%-- ng-bind-html-unsafe="Interface | pretty">--%>
            <div ng-include src="'App/Partials/PagePreview.html'"></div>
            <div ng-include src="'App/Partials/PageActionBar.html'"></div>
        </div>
    </div>

    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
    <script>window.jQuery || document.write('<script src="js/vendor/jquery-1.9.1.min.js"><\/script>')</script>

    <script src="redesign/js/plugins.js"></script>
    <script src="redesign/js/n2.js"></script>
    <%--<script src="redesign/js/n2.loaders.js"></script>--%>
    <script src="redesign/js/main.js"></script>
	<script src="Resources/angular-1.1.4/angular.js"></script>
	<script src="Resources/angular-1.1.4/angular-resource.js"></script>
	<script src="App/Js/Services.js"></script>
	<script src="App/Js/Routes.js"></script>
	<script src="App/Js/Controllers.js"></script>
	<script src="App/Js/Directives.js"></script>
	<link href="redesign/bootstrap/css/bootstrap.css" rel="stylesheet" />
	<script src="redesign/bootstrap/js/bootstrap.js"></script>
	<script src="Resources/angular-ui-0.4.0/build/angular-ui.js"></script>
	<link href="Resources/angular-ui-0.4.0/build/angular-ui.min.css" rel="stylesheet" />
	<style>
		ul, li {
			margin: 0;
			padding: 0;
		}
	</style>
</body>

</html>
