<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<table>
	<tr><th>
		Url.Action("TheAction")
	</th><td>
		<%= Url.Action("TheAction") %>
	</td></tr>

	<tr><th>
		Url.Action("TheAction", new { item = Html.CurrentItem().Parent })
	</th><td>
		<%= Url.Action("TheAction", new { item = Html.CurrentItem().Parent })%>
	</td></tr>

	<tr><th>
		Url.Action("TheAction", new { page = N2.Find.StartPage, part = Html.CurrentItem() })
	</th><td>
		<%= Url.Action("TheAction", new { page = N2.Find.StartPage, part = Html.CurrentItem() })%>
	</td></tr>

</table>
