<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<% var content = Html.Content(); %>
<% var searcher = content.Services.Resolve<N2.Persistence.ITextSearcher>(); %>
<% int total = 0; %>
<% var results = searcher.Search(content.Traverse.StartPage, Model, 0, 10, out total); %>
<% var filteredResults = content.Is.Page().Pipe(results).ToList(); %>
<% if(filteredResults.Any()) { %>
<ul>
	<% foreach(var item in filteredResults) { %>
	<li><span><%= content.LinkTo(item) %></span></li>
	<% } %>
</ul>
<% } %>