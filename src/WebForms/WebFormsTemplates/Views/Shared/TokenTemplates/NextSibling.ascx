<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<% var content = Html.Content(); %>
<% using (Html.Content().BeginScope(Model)) {
       N2.ContentItem next = null;
       next = Html.Content().Traverse.Children(content.Traverse.Parent(),
           content.Is.AccessiblePage()).SkipWhile(i => i != content.Current.Item).Skip(1).FirstOrDefault(); %>
	<%= Html.Content().LinkTo(next) %>
<% } %>	