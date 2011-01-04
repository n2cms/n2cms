<%@ Page Language="C#" MasterPageFile="../Site.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage<News>" %>

<asp:Content ContentPlaceHolderID="TextContent" runat="server">
	<%= Html.DisplayContent(m => m.Title) %>
	<%= Html.DisplayContent("Published").WrapIn("span", new { @class = "date" })%>
	<%= Html.DisplayContent(m => m.Introduction).WrapIn("p", new { @class = "introduction" }) %>
	<%= Html.DisplayContent(m => m.Text) %>
</asp:Content>