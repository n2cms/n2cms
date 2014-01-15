<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
	var item = Html.CurrentItem();
	var page = Html.CurrentPage();
%>
<table>
	<tr><th>
		item:
	</th></tr><tr><td>
		<%= item %>
	</td></tr>
	<tr><th>
		page:
	</th></tr><tr><td>
		<%= page %>
	</td></tr>
	
	<tr><th>
		url action:
	</th></tr><tr><td>
		<a href="<%= Url.Action("TheAction") %>">
			<%= Html.Encode(Url.Action("TheAction")) %>
		</a>
	</td></tr>

	<tr><th>
		page action:
	</th></tr><tr><td>
		<a href="<%= Url.Action(page) %>">
			<%= Html.Encode(Url.Action(page))%>
		</a>
	</td><td>
		<a href="<%= Url.Action(page, "TheAction") %>">
			<%= Html.Encode(Url.Action(page, "TheAction"))%>
		</a>
	</td></tr>

	<tr><th>
		item action:
	</th></tr><tr><td>
		<a href="<%= Url.Action(item) %>">
			<%= Html.Encode(Url.Action(item))%>
		</a>
	</td><td>
		<a href="<%= Url.Action(item, "TheAction") %>">
			<%= Html.Encode(Url.Action(item, "TheAction"))%>
		</a>
	</td></tr>

</table>
