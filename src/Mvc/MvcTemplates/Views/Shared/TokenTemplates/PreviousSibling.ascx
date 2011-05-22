<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<% var content = Html.Content(); %>
<% using (content.BeginScope(Model)) {
	N2.ContentItem prev = null;
	content.Traverse.Children(content.CurrentItem.Parent, content.Is.Navigatable())
		.SkipWhile(i => { if (i != content.CurrentItem) { prev = i; return true; } else return false; })
		.FirstOrDefault(); %>
	<%= content.LinkTo(prev) %>
<% } %>