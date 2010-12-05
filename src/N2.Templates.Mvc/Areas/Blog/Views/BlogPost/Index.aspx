<%@ Page ValidateRequest="false" Language="C#" MasterPageFile="~/Views/Shared/Site.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage<BlogPost>" %>
<%@ Import Namespace="N2.Templates.Mvc.Areas.Blog.Models.Pages" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
	<% Html.RenderPartial("Styles"); %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TextContent" runat="server">
    <%--Set ViewData["full"] to true to display full post--%>
    <%  ViewData["full"] = true;
        Html.RenderPartial("BlogBody", Model); %>  

    <%--Only show comment input form if global blog setting and post setting allow--%>
    <% if (Model.Published != null && Model.EnableComments && ((BlogPostContainer) Model.Parent).EnableComments) { %>
        <div class="blog-comment-form">
            <% Html.RenderAction("BlogCommentInputForm", "BlogPost"); %>
        </div>
    <%--Only show comments if global blog setting and post setting allow--%>
    <% } if (Model.Published != null && Model.ShowComments && ((BlogPostContainer) Model.Parent).ShowComments) { %>
        <div class="blog-comment-container">
            <% 
           foreach (var comment in Model.Comments) {
               Html.RenderPartial("BlogComment", comment);
           } %>
        </div>
    <% } %>

</asp:Content>