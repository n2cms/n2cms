<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Top+SubMenu.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage<News>" %>

<asp:Content ContentPlaceHolderID="Head" runat="server">
	<script type="text/javascript" src="<%=ResolveUrl("~/Content/Scripts/jquery-1.3.2.min.js")%>"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="TextContent" runat="server">
	<%= Html.Display(m => m.Title) %>
	<%= Html.Display("Published").WrapIn("span", new { @class = "date" })%>
	<%= Html.Display(m => m.Introduction).WrapIn("p", new { @class = "introduction" }) %>
	<%= Html.Display(m => m.Text) %>
</asp:Content>