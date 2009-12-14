<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<Top>" %>
<%@ Import Namespace="N2.Templates.Mvc.Items.Items"%>
<%if(!String.IsNullOrEmpty(Model.LogoUrl)){%>
<a class="siteLogo" href="<%= ResolveUrl(Model.LogoLinkUrl) %>">
	<%=Html.Display(top => top.LogoUrl).SwallowExceptions() %>
</a>
<%}%>
<%if(!String.IsNullOrEmpty(Model.Title)){%>
<h2 class="siteHeader"><a href="<%=ResolveUrl(Model.TopTextUrl)%>"><%= Model.Title %></a></h2>
<%}%>