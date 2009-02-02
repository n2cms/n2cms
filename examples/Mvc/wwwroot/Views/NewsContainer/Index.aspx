<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<NewsContainerViewData>" %>
<%@ Import Namespace="MvcTest.Views.NewsContainer"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
	<h1><%= Model.Container.Title %></h1>
	<ul class="newsList">
	<% foreach (MvcTest.Models.NewsPage item in Model.News) { %>
		<li><%= N2.Web.Link.To(item) %></li>
	<% } %>
	</ul>
</asp:Content>
