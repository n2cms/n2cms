<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Top.ascx.cs" Inherits="N2.Templates.UI.Layouts.Parts.Top" %>
<% if(CurrentItem.LogoUrl.Length > 0) {%>
<a href="<%=CurrentItem.LogoLinkUrl%>"><n2:Display PropertyName="LogoUrl" runat="server" /></a>
<%}%>
<h2 runat="server" visible='<%$ Code: CurrentItem.Title.Length>0 %>'><a href="<%= CurrentItem.TopTextUrl %>"><%= CurrentItem.Title %></a></h2>