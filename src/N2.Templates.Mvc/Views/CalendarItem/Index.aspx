<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Top+SubMenu.Master" AutoEventWireup="true"
	Inherits="N2.Web.Mvc.N2ViewPage<Event>" %>

<asp:Content ID="cpc" ContentPlaceHolderID="TextContent" runat="server">
	<%=this.EditableDisplay(m => m.Title)%>
	<span class="date"><%= CurrentItem.EventDateString %></span>
	<p class="introduction"><%=this.EditableDisplay(m => m.Introduction)%></p>
	<%=this.EditableDisplay(m => m.Text)%>
</asp:Content>