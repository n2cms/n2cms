<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<%= Html.Content().LinkTo(Html.Content().Traverse.StartPage) %>