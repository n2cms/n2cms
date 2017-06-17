<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<BlogPostContainerModel>" %>
<%@ Import Namespace="N2.Templates.Mvc.Areas.Blog.Models.Pages" %>
<%@ Import Namespace="N2.Templates.Mvc.Areas.Blog.Models" %>
<asp:Content ContentPlaceHolderID="Head" runat="server">
	<% Html.RenderPartial("Styles"); %>
</asp:Content>
<asp:Content ID="mc" ContentPlaceHolderID="PostContent" runat="server">

    <div class="blog-list">
		<% Html.RenderPartial("BlogList", Model); %>
    </div>

</asp:Content>
