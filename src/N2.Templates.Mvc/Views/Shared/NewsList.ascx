<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.N2ViewUserControl<NewsList>" %>

<h<%=Model.TitleLevel%>><%=Model.Title%></h<%=Model.TitleLevel%>>

<div class="sidelist">
<%
	var i = 0;
	foreach(var item in Model.FilteredNewsItems){
		i++;%>
	<div class="news i<%= i %> a<%= i % 2 %>">
		<a href='<%=item.Url%>' title='<%= item.Published + ", " + item.Introduction %>'><%= item.Title %></a>
	</div>
<%} %>
</div>