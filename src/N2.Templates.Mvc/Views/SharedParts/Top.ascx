<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<N2.Templates.Mvc.Models.Parts.Top>" %>
<%@ Import Namespace="N2.Templates.Mvc.Models.Parts"%>
<div class="uc">
	<%if(!String.IsNullOrEmpty(Model.LogoUrl)){%>
	<a class="siteLogo" href="<%= ResolveUrl(Model.LogoLinkUrl) %>">
		<%=Html.DisplayContent(top => top.LogoUrl).SwallowExceptions() %>
	</a>
	<%}%>
	<%if(!String.IsNullOrEmpty(Model.Title)){%>
	<h2 class="siteHeader"><a href="<%=ResolveUrl(Model.TopTextUrl)%>"><%= Model.Title %></a></h2>
	<%}%>
</div>