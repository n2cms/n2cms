<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<Comment>" %>
<div class="uc">

<% if (N2.Context.Current.SecurityManager.IsEditor(ViewContext.HttpContext.User)){ %>
<div style="position:relative;top:20px;">
	<% Html.ResolveService<N2.Web.UI.PartUtilities>().WriteTitleBar(Html.ViewContext.Writer, Model, Request.RawUrl); %>
</div>
<% } %>

	<div class="item i<%= Model.Parent.Children.IndexOf(Model) %> a<%= Model.Parent.Children.IndexOf(Model) % 2 %>">
		<h4><%= Model.Title %></h4>
		<p><%= Model.Text%></p>
		<span class="date"><%= Model.Published%>, 
			<% if (Model.AuthorUrl.Length > 0){%>
				<a href="<%= Model.AuthorUrl %>" rel="nofollow"><%= Model.AuthorName%></a>
			<% } else { %>
				<%= Model.AuthorName%>
			<% } %>
		</span>
	</div>
</div>