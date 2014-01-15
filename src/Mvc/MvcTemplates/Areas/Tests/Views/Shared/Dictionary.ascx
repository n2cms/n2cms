<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Collections.Generic.IDictionary<string, object>>" %>
<table>
	<% foreach (var p in Model) { %>
	<tr><th><%= p.Key %></th><td><%= p.Value %></td></tr>
	<% } %>
</table>
