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

	<script src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.JQueryPath) %>"></script>
	<script src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.JQueryUiPath) %>"></script>

	<script src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.AngularPath) %>"></script>
	<script src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.AngularResourcesPath) %>"></script>

	<link href="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.TwitterBootstrapCssPath) %>" rel="stylesheet" />
	<script src="<%= N2.Web.Url.ResolveTokens(N2.Resources.Register.TwitterBootstrapJsPath) %>"></script>

	<link rel="stylesheet" href="Resources/font-awesome/css/font-awesome.min.css">

	<script src="Resources/angular-ui-0.4.0/angular-ui.min.js"></script>
	<link href="Resources/angular-ui-0.4.0/angular-ui.min.css" rel="stylesheet" />

	<link href="Resources/bootstrap-components/bootstrap-datepicker.css" rel="stylesheet" />
	<script src="Resources/bootstrap-components/bootstrap-datepicker.js"></script>

	<link href="Resources/bootstrap-components/bootstrap-timepicker.css" rel="stylesheet" />
	<script src="Resources/bootstrap-components/bootstrap-timepicker.js"></script>
	<link href="Resources/icons/flags.css" rel="stylesheet" />

	<script src="Resources/bootstrap-components/angular-strap.min.js"></script>

	<script src="Resources/js/n2.js"></script>
	<link rel="stylesheet" href="Resources/css/n2.css">

	<script src="<%= GetLocalizationPath() %>"></script>
	<script src="App/Js/Services.js"></script>
	<script src="App/Js/Controllers.js"></script>
	<script src="App/Js/Directives.js"></script>

	<asp:PlaceHolder runat="server">
	<% foreach(var module in N2.Context.Current.Container.ResolveAll<N2.Management.Api.ManagementModuleBase>()) { %>
	<!-- <%= module.GetType().Name %> -->
	<% foreach(var script in module.ScriptIncludes) { %>
	<script src="<%= N2.Web.Url.ResolveTokens(script) %>"></script>
	<% } %>
	<% foreach(var style in module.StyleIncludes) { %>
	<link href="<%= N2.Web.Url.ResolveTokens(style) %>" rel="stylesheet" />
	<% } %>
	<% } %>
	</asp:PlaceHolder>
</head>
<body ng-controller="ManagementCtrl" ng-app="n2" x-context-menu-trigger=".item" ng-include src="Context.Partials.Management">
	<%--<div id="debug-context" class="debug" ng-bind-html-unsafe="Context | pretty"></div>--%>
</body>
</html>

<script runat="server">

	protected string GetLocalizationPath()
	{
		var culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
		var languagePreferenceList = new[] { culture.ToString(), culture.TwoLetterISOLanguageName };
		foreach (var languageCode in languagePreferenceList)
		{
			var path = N2.Web.Url.ResolveTokens("{ManagementUrl}/App/i18n/" + languageCode + ".js.ashx");
			if (System.Web.Hosting.HostingEnvironment.VirtualPathProvider.FileExists(path))
				return path;
		}
		return "App/i18n/en.js.ashx";
	}
</script>
