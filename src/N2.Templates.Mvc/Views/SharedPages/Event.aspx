<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Top+SubMenu.Master" AutoEventWireup="true"
	Inherits="System.Web.Mvc.ViewPage<Event>" %>

<asp:Content ID="cpc" ContentPlaceHolderID="TextContent" runat="server">
	<%= Html.Display(m => m.Title)%>
	<%= Html.Display("EventDateString").WrapIn("span", new { @class = "date" }) %>
	<%= Html.Display(m => m.Introduction).WrapIn("p", new { @class = "introduction" }) %>
	<%= Html.Display(m => m.Text)%>
</asp:Content>