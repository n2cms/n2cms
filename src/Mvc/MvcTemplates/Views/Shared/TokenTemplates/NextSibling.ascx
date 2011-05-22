<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<% using (Html.Content().BeginScope(Model)) { %>
	var next = Html.Content().Traverse.Children(Content.CurrentItem.Parent, Content.Is.Navigatable()).SkipWhile(i => i != Content.CurrentItem).Skip(1).FirstOrDefault();
	<%= Html.Content().LinkTo(next) %>
<% } %>	