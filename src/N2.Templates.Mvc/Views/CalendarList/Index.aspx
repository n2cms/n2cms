<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Top+SubMenu.Master" AutoEventWireup="true"
	Inherits="N2.Web.Mvc.N2ModelViewPage<CalendarModel, N2.Templates.Mvc.Items.Pages.Calendar>" %>
<%@ Import Namespace="N2.Collections"%>

<asp:Content runat="server" ContentPlaceHolderID="Head">
	<link rel="stylesheet" type="text/css" href="<%=ResolveUrl("~/Content/Css/Calendar.css") %>" />
</asp:Content>

<asp:Content ID="cpc" ContentPlaceHolderID="PostContent" runat="server">
	<%if(Model.Events.Count > 0){%>
		<div class="list">
		<% var i = 0; %>
		<%foreach(var result in Model.Events){%>
			<div class="item i<%= i++ %> a<%= i % 2 %>">
				<span class="date"><%= result.EventDateString %></span>
				<a href='<%= result.Url %>'><%=result.Title%></a>
				<p><%=result.Introduction%></p>
			</div>
		<%} %>
		</div>
	<%}else{%>
		<p><%= GetLocalResourceObject("NoItemsFound") %></p>
	<%}%>
</asp:Content>