<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage<N2.Definitions.IFeed>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TextContent" runat="server">
	
	<h1><a href="<%= Model.Url %>"><%= Model.Title %></a></h1>
	<p><%= Model.Tagline %></p>
	
	<% foreach (var item in Model.GetItems()) { %>
	<h3><a href="<%= item.Url %>"><%= item.Title %></a></h3>
	<p><%= item.Summary %></p>
	<%} %>
</asp:Content>
