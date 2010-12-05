<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage<BlogComment>" Title="" %>
<%@ Import Namespace="N2.Web" %>
<%@ Import Namespace="N2.Templates.Mvc.Areas.Blog.Models.Pages" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <% 
        N2.Web.Url url = new Url(Request.ServerVariables["HTTP_REFERER"]);
        N2.Web.Url editUrl = N2.Web.Url.ToAbsolute(N2.Context.Current.EditManager.GetEditExistingItemUrl(Model));

        // Probably came from edit page.  Redirect to Blog Post
        if (Server.UrlDecode(url.LocalUrl).Equals(Server.UrlDecode(editUrl.LocalUrl), StringComparison.CurrentCultureIgnoreCase))
        {
            Response.Redirect(Model.Parent.Url + "#c" + Model.CommentID);            
        } 
    %>
</asp:Content>