<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="MvcTest.Views.Static.Index" %>
<%@ Import Namespace="System.Web.Mvc" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <div>
    	<h2><%= ViewData["message"]%></h2>
    	<ul>
    	<%foreach (string id in (IEnumerable)ViewData["items"]){%>
    		<li><%= Html.ActionLink("Show " + id, "items", new { id })%></li>
    	<% } %>
    	</ul>
    </div>
</body>
</html>