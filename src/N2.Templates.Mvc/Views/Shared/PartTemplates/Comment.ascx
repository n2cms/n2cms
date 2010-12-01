<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<Comment>" %>
<div class="uc">

<% if (N2.Context.Current.SecurityManager.IsEditor(ViewContext.HttpContext.User)){ %>
<div style="position:relative;top:20px;">
	<% N2.Web.UI.PartUtilities.WriteTitleBar(Html.ViewContext.Writer, N2.Context.Current.ManagementPaths, N2.Context.Current.Resolve<N2.Engine.IContentAdapterProvider>(), N2.Context.Current.Definitions.GetDefinition(Model.GetType()), Model); %>
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