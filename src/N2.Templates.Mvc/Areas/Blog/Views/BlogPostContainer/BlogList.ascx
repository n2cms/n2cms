<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BlogPostContainerModel>" %>
<%@ Import Namespace="N2.Templates.Mvc.Areas.Blog.Models.Pages" %>
<%@ Import Namespace="N2.Templates.Mvc.Areas.Blog.Models" %>

<div class="blog-post-list-container">
    <%--Loop through posts--%>
    <% for(var i = 0; i < Model.Posts.Count; i++)
       {
          //Set ViewData["full"] to false to display summarized post
          ViewData["full"] = false;
          Html.RenderPartial("BlogBody", Model.Posts[i]);
       }
        
       // Get current action.  Pagination action will be 'Page' if we're on index.  Otherwise keep what is there
       string action = N2.Context.Current.RequestContext.CurrentPath.Action;
       action = action.Equals("Index", StringComparison.CurrentCultureIgnoreCase) ? "Page" : action;
       var parms = new object(); %>

    <div class="blog-navigation-links">
        <%--Display previous page link--%>
        <% if (!Model.IsFirst) {
               if (action.Equals("Tag", StringComparison.CurrentCultureIgnoreCase))
               {
                   parms = new { t = ViewData["Tag"], p = Model.Page - 1 };
               }
               else
               {
                   parms = new { p = Model.Page - 1 };
               }%>
        <div class="blog-left">
            <%= Html.ActionLink(string.Format("« Page {0}", Model.Page - 1), action, parms, new { @class = "blog-page-link" })%>
        </div>
        <% } %>
        
        <%--Display Next page link--%>
        <% if (!Model.IsLast)
           {
               if (action.Equals("Tag", StringComparison.CurrentCultureIgnoreCase))
               {
                   parms = new { t = ViewData["Tag"], p = Model.Page + 1 };
               }
               else
               {
                   parms = new { p = Model.Page + 1 };
               }%>
        <div class="blog-right">
            <%= Html.ActionLink(string.Format("Page {0} »", Model.Page + 1), action, parms, new { @class = "blog-page-link" })  %>
        </div>
        <% } %>
        <div class="clear"></div>
    </div>
</div>