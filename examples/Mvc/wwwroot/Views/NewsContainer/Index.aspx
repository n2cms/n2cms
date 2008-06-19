<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="MvcTest.Views.NewsContainer.Index" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
	<h1><%= ViewData.Container.Title %></h1>
	<ul class="newsList">
	<% foreach (MvcTest.Models.NewsPage item in ViewData.News) { %>
		<li><%= N2.Web.Link.To(item) %></li>
	<% } %>
	</ul>
</asp:Content>
