<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>

<% if (string.IsNullOrEmpty(Model)) { %>
<input type="submit" />
<% } else { %>
<input type="submit" value="<%= Model %>"/>
<% } %>
