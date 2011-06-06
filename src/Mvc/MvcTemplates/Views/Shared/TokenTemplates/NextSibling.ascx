<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<% using (Html.Content().BeginScope(Model)) { %>
	var next = Html.Content().Traverse.Children(Content.Traverse.Parent(), Content.Is.AccessiblePage()).SkipWhile(i => i != Content.Path.CurrentItem).Skip(1).FirstOrDefault();
	<%= Html.Content().LinkTo(next) %>
<% } %>	