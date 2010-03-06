<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<NewsList>" %>
<div class="uc">
	<h<%=Model.TitleLevel%>><%=Model.Title%></h<%=Model.TitleLevel%>>

	<div class="sidelist">
	<%
		var i = 0;
		foreach(var item in Model.FilteredNewsItems){
			i++;%>
		<div class="item news i<%= i %> a<%= i % 2 %>">
			<a href='<%=item.Url%>' title='<%= item.Published + ", " + item.Introduction %>'><%= item.Title %></a>
		</div>
	<%} %>
	</div>
</div>