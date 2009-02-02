<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<object>" %>
<%@ Import Namespace="System.Web.Mvc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
	<h2><%= ViewData["message"]%></h2>
	<ul>
	<%foreach (string id in (IEnumerable)ViewData["items"]){%>
		<li><%= Html.ActionLink("Show " + id, "items", new { id })%></li>
	<% } %>
	</ul>
</asp:Content>