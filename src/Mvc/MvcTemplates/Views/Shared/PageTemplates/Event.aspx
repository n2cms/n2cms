<%@ Page Title="" Language="C#" MasterPageFile="../Site.master" AutoEventWireup="true"
	Inherits="System.Web.Mvc.ViewPage<Event>" %>

<asp:Content ID="cpc" ContentPlaceHolderID="TextContent" runat="server">
	<%= Html.DisplayContent(m => m.Title)%>
	<%= Html.DisplayContent("EventDateString").WrapIn("span", new { @class = "date" }) %>
	<%= Html.DisplayContent(m => m.Summary).WrapIn("p", new { @class = "introduction" }) %>
	<%= Html.DisplayContent(m => m.Text)%>
</asp:Content>
