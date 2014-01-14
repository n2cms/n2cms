<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.master" AutoEventWireup="true"
	Inherits="N2.Web.Mvc.ContentViewPage<CalendarModel, N2.Templates.Mvc.Models.Pages.Calendar>" %>
<%@ Import Namespace="N2.Collections"%>

<asp:Content ID="cpc" ContentPlaceHolderID="PostContent" runat="server">
	<%if(Model.Events.Count > 0){%>
		<div class="list">
		<% var i = 0; %>
		<%foreach(var result in Model.Events){%>
			<div class="item i<%= i++ %> a<%= i % 2 %>">
				<span class="date"><%= result.EventDateString %></span>
				<a href='<%= result.Url %>'><%=result.Title%></a>
				<p><%=result.Summary%></p>
			</div>
		<%} %>
		</div>
	<%}else{%>
		<p><%= GetLocalResourceObject("NoItemsFound") %></p>
	<%}%>
</asp:Content>
