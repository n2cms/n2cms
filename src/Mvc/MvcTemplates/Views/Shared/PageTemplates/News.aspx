<%@ Page Language="C#" MasterPageFile="../Site.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage<News>" %>

<asp:Content ContentPlaceHolderID="TextContent" runat="server">
	<%= Html.DisplayContent(m => m.Title) %>
	<%= Html.DisplayContent("Published").WrapIn("span", new { @class = "date" })%>
	<%= Html.DisplayContent(m => m.Text) %>
	<% if(Html.HasValue("Tags")) { %>
		<div class="tags"><label for="tags"><%= GetLocalResourceObject("Tags") %></label><span id="tags"><%= Html.DisplayContent("Tags") %></span></div>
	<% } %>
</asp:Content>
