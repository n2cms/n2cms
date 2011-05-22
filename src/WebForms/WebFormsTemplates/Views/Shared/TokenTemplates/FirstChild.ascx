<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<% using (Html.Content().BeginScope(Model)) { %>
<%= Html.Content().LinkTo(Html.Content().Traverse.Children().FirstOrDefault()) %>
<% } %>