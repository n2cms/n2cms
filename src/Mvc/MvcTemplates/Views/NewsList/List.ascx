<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<News>>" %>
<div class="uc">
	<% NewsList list = Html.CurrentItem<NewsList>(); %>
	<%= string.Format("<h{0}>{1}</h{0}>", list.TitleLevel, list.Title)%>

	<div class="sidelist">
	<%
		var i = 0;
		foreach(var item in Model){
			i++;%>
		<div class="item news i<%= i %> a<%= i % 2 %>">
			<a href='<%=item.Url%>' title='<%= item.Published + ", " + item.Summary %>'><%= item.Title %></a>
		</div>
	<%} %>
	</div>
</div>