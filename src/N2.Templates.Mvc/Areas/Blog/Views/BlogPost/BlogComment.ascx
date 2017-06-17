<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BlogComment>" %>
<%@ Import Namespace="N2.Templates.Mvc.Areas.Blog.Models.Pages" %>

<div class="blog-comment" id="c<%=Model.CommentID%>">
    <div class="blog-comment-title">
        <span><%= Html.Encode(Model.Title) %></span>
        <%--Only display edit and delte links if allowed--%>
        <% if (N2.Context.SecurityManager.IsAuthorized(Page.User, Model, N2.Security.Permission.Write)) 
            { %>
            <span class="blog-right"><a href="<%= ResolveUrl(N2.Context.Current.EditManager.GetEditExistingItemUrl(Model)) %>"><img src="<%= ResolveUrl("~/N2/Resources/icons/page_edit.png") %>" alt="Edit Comment" title="Edit Comment" /></a> | <a href="<%= ResolveUrl(N2.Context.Current.EditManager.GetDeleteUrl(Model)) %>"><img src="<%= ResolveUrl("~/N2/Resources/icons/cross.png") %>" alt="Delete Comment" title="Delete Comment" /></a></span>
        <% } %>
    </div>
    <%--Main text of comment--%>
    <%= Html.DisplayContent(Model, "Text").WrapIn("div", new { @class = "blog-comment-text" })%>
    <div class="blog-comment-footer">
        <%--Display author's name and URL if given--%>
        by <% if (!string.IsNullOrEmpty(Model.AuthorUrl))
            { %>
                <%= Html.DisplayContent(Model, "AuthorName").WrapIn("a", new { href = Model.AuthorUrl })%>
        <% }
            else
            { %>
                <%= Html.DisplayContent(Model, "AuthorName")%>
        <% } %> 
        <%--Display comment date and link to this comment--%>
            on <% if (Model.Published != null)
            { %><a href="#c<%=Model.CommentID%>"><%= Html.Encode(Model.Published.Value.ToString("dddd, MMMM dd, yyyy") + " at " + Model.Published.Value.ToString("hh:mm tt"))%></a><% }
            else
            { %>Not Published<% } %>
    </div>
</div>