<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<object>" %>
<%@ Import Namespace="System.Web.Mvc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
	<h2><%= ViewData["message"] %></h2>
	<%= Html.ActionLink("Back", "index") %>
</asp:Content>