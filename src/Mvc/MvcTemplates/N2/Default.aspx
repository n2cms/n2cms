<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Management.Default" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<!DOCTYPE html>
<!--[if lt IE 7]>      <html class="no-js lt-ie9 lt-ie8 lt-ie7"> <![endif]-->
<!--[if IE 7]>         <html class="no-js lt-ie9 lt-ie8"> <![endif]-->
<!--[if IE 8]>         <html class="no-js lt-ie9"> <![endif]-->
<!--[if gt IE 8]><!-->
<html class="no-js">
<!--<![endif]-->
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
    <title>N2 Management</title>
    <meta name="viewport" content="width=device-width">

    <link rel="stylesheet" href="Resources/css/n2.css">

	<script src="//ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
    <script>window.jQuery || document.write('<script src="Resources/Js/jquery-1.9.1.min.js"><\/script>')</script>
    <script src="Resources/js/plugins.js"></script>
    <script src="Resources/js/n2.js"></script>
    <script src="Resources/js/main.js"></script>
	<script src="Resources/angular-1.1.4/angular.js"></script>
	<script src="Resources/angular-1.1.4/angular-resource.js"></script>
	<link href="Resources/bootstrap/css/bootstrap.css" rel="stylesheet" />
	<script src="Resources/bootstrap/js/bootstrap.js"></script>
	<link rel="stylesheet" href="Resources/font-awesome/css/font-awesome.min.css">
	<script src="Resources/angular-ui-0.4.0/build/angular-ui.js"></script>
	<link href="Resources/angular-ui-0.4.0/build/angular-ui.min.css" rel="stylesheet" />
	<script src="Resources/jquery-ui-1.10.2.custom/js/jquery-ui-1.10.2.custom.js"></script>
	<link href="Resources/bootstrap-components/bootstrap-datepicker.css" rel="stylesheet" />
	<script src="Resources/bootstrap-components/bootstrap-datepicker.js"></script>
	<link href="Resources/bootstrap-components/bootstrap-timepicker.css" rel="stylesheet" />
	<script src="Resources/bootstrap-components/bootstrap-timepicker.js"></script>
	<script src="Resources/bootstrap-components/angular-strap.js"></script>

</head>
<body ng-controller="ManagementCtrl" ng-app="n2" x-context-menu-trigger=".item">
    <div id="top-area">
        <%--<div ng-include src="'App/Partials/UserTools.html'"></div>--%>
        <%--<div ng-include src="'App/Partials/MainMenu.html'"></div>--%>
		<div ng-include src="'App/Partials/Menu.html'"></div>
    </div>
    <div class="sliding-loader loader"></div>
	<div id="main-wrapper">
		<div id="secondary-area" ng-controller="NavigationCtrl">
			<div ng-include src="'App/Partials/PageSearch.html'" id="page-search"></div>
			<div ng-include src="'App/Partials/PageTree.html'" id="page-tree"></div>
			<div class="dragbar"></div>
		</div>
		<div id="main-area"><%-- ng-bind-html-unsafe="Interface | pretty">--%>
			<div ng-include src="'App/Partials/PagePreview.html'"></div>
		</div>
    </div>
	<div id="footer" ng-include src="'App/Partials/Footer.html'">
	</div>
    <%--<div id="debug-context" class="debug" ng-bind-html-unsafe="Context | pretty"></div>
    <div id="debug-interface" class="debug" ng-bind-html-unsafe="Interface | pretty"></div>--%>
	
	<script src="App/Js/Services.js"></script>
	<script src="App/Js/Routes.js"></script>
	<script src="App/Js/Controllers.js"></script>
	<script src="App/Js/Directives.js"></script>

	<style>
		ul, li {
			margin: 0;
			padding: 0;
		}
	</style>
</body>

</html>
