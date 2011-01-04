<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Comment.ascx.cs" Inherits="N2.Templates.UI.Parts.Comment" %>
<div class="item i<%= Parent.Controls.IndexOf(this) %> a<%= Parent.Controls.IndexOf(this) % 2 %>">
    <h4><%= CurrentItem.Title %></h4>
    <p><%= CurrentItem.Text %></p>
    <span class="date"><%= CurrentItem.Published %>, 
        <% if (CurrentItem.AuthorUrl.Length > 0) {%>
            <a href="<%= CurrentItem.AuthorUrl %>" rel="nofollow"><%= CurrentItem.AuthorName %></a>
        <% } else { %>
            <%= CurrentItem.AuthorName %>
        <% } %>
    </span>
</div>