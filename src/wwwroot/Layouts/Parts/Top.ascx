<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Top.ascx.cs" Inherits="N2.Templates.UI.Layouts.Parts.Top" %>
<a class="siteLogo" visible="<%$ HasValue: LogoUrl %>" href="<%$ CurrentItem: LogoLinkUrl %>" runat="server">
	<n2:Display PropertyName="LogoUrl" runat="server" />
</a>
<h2 class="siteHeader" runat="server" visible='<%$ HasValue: Title %>'><a href="<%= CurrentItem.TopTextUrl %>"><%= CurrentItem.Title %></a></h2>