<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.N2ViewUserControl<Top>" %>
<%@ Import Namespace="N2.Templates.Mvc.Items.Items"%>
<%if(!String.IsNullOrEmpty(Model.LogoUrl)){%>
<a class="siteLogo" href="<%= ResolveUrl(Model.LogoLinkUrl) %>">
	<%=this.Display(top => top.LogoUrl).SwallowExceptions() %>
</a>
<%}%>
<%if(!String.IsNullOrEmpty(Model.Title)){%>
<h2 class="siteHeader"><a href="<%=ResolveUrl(Model.TopTextUrl)%>"><%= Model.Title %></a></h2>
<%}%>