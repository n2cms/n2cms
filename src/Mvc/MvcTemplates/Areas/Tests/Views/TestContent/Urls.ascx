<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<table>
	<tr><th>
		Html.CurrentItem()
	</th></tr><tr><td>
		<%= Html.CurrentItem()%>
	</td></tr>
	<tr><th>
		Html.CurrentPage()
	</th></tr><tr><td>
		<%= Html.CurrentPage()%>
	</td></tr>
	
	<tr><th>
		Url.Action("TheAction")
	</th></tr><tr><td>
		<a href="<%= Url.Action("TheAction") %>">
			<%= Html.Encode(Url.Action("TheAction")) %>
		</a>
	</td></tr>

	<tr><th>
		Url.Action(null, new { item = Html.CurrentPage() })
	</th></tr><tr><td>
		<a href="<%= Url.Action(null, new { item = Html.CurrentPage() }) %>">
			<%= Html.Encode(Url.Action(null, new { item = Html.CurrentPage() }))%>
		</a>
	</td></tr>

	<tr><th>
		Url.Action(null, new { page = N2.Find.StartPage, item = Html.CurrentItem() })
	</th></tr><tr><td>
		<a href="<%= Url.Action(null, new { page = Html.CurrentPage(), item = Html.CurrentItem() }) %>">
			<%= Html.Encode(Url.Action(null, new { page = Html.CurrentPage(), item = Html.CurrentItem() }))%>
		</a>
	</td></tr>

</table>
