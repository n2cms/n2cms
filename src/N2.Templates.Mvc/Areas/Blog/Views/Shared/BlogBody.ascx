<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<BlogPost>" %>
<%@ Import Namespace="N2.Templates.Mvc.Areas.Blog.Models.Pages" %>
    <div class="blog-container">
    
           <div class="blog-header">    
	        <%=  (bool)ViewData["full"] == false ? Html.DisplayContent(m => m.Title).WrapIn("a", new { href = Model.Url, @class = "blog-title-link" }) : Html.DisplayContent(m => m.Title).WrapIn("span", new { @class = "blog-title" })%>
            by <%= Html.DisplayContent(m => m.Author).WrapIn("span", new { @class = "blog-author" })%><br />
            <span class="blog-date">
                <% if (Model.Published != null)
                { %><%= Html.Encode(Model.Published.Value.ToString("dddd, MMMM dd, yyyy") + " at " + Model.Published.Value.ToString("hh:mm tt"))%><% }
                else
                { %>Not Published<% } %> 
            </span>
        </div>
        <div class="blog-body">
            <% if (!String.IsNullOrEmpty(Model.ImageUrl))
               {%>
                <div class="blog-image">
                    <img src="<%= ResolveUrl(Model.GetResizedImageUrl(Html.ResolveService<N2.Edit.FileSystem.IFileSystem>()))%>" alt="<%=Html.DisplayContent(m => m.Author) %>" />
                    <br />
                    <%= Html.DisplayContent(m => m.ImageCaption).WrapIn("span", new { @class = "blog-image-caption" })%>
                </div>
            <% } %>
            <%= (bool)ViewData["full"] == false ? Html.DisplayContent(m => m.Introduction) : Html.DisplayContent(m => m.Text)%>
            <div style="clear: both;"></div>
        </div>
        <div class="blog-footer">
            <div>
                <%--Only display edit and delte links if allowed--%>
                <% if (N2.Context.SecurityManager.IsAuthorized(Page.User, Model, N2.Security.Permission.Write)) 
                   {%>
                   <a href="<%= ResolveUrl(N2.Context.Current.EditManager.GetEditExistingItemUrl(Model)) %>"><img src="<%= ResolveUrl("~/N2/Resources/icons/page_edit.png") %>" alt="Edit Post" title="Edit Post" /></a> | <a href="<%= ResolveUrl(N2.Context.Current.EditManager.GetDeleteUrl(Model)) %>"><img src="<%= ResolveUrl("~/N2/Resources/icons/cross.png") %>" alt="Delete Post" title="Delete Post" /></a><% if ((bool)ViewData["full"] == false) { %> | <% } %>
                <% } %>
                <% if ((bool)ViewData["full"] == false) { %><a href="<%= Model.Url %>"><%= Model.IsSummarized ? "Read More..." : "Discuss"%></a> (<%= Model.Comments.Count()%> Comments) <% } %>
            </div>
            <%--Display tag links--%>
            <% if (!string.IsNullOrEmpty(Model.Tags.Trim())) {
                   StringBuilder tagLinks = new StringBuilder("<div class=\"blog-tag-list\">Tags: ");
                   string[] tags = Model.Tags.Split(new char[] { ',' });
                   
                   foreach (string tag in tags)
                   {
                       if ((bool)ViewData["full"])
                       {
                           tagLinks.Append(Html.ActionLink(tag.Trim(), Model.Parent, "Tag", new { t = tag.Trim() }, new { @class = "blog-tag-link" }));
                       }
                       else
                       {
                           tagLinks.Append(Html.ActionLink(tag.Trim(), "Tag", new { t = tag.Trim() }, new { @class = "blog-tag-link" }));
                       }
                       tagLinks.Append(", ");
                   }
                   
                   tagLinks.Remove(tagLinks.Length - 2, 2).Append("</div>");
                   Response.Write(tagLinks.ToString());
               } %>
            <hr />
        </div>
    </div>