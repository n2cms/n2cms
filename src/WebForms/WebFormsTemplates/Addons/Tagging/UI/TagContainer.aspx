<%@ Page Language="C#" MasterPageFile="Layout.master" AutoEventWireup="true" CodeBehind="TagContainer.aspx.cs" Inherits="N2.Addons.Tagging.UI.TagContainer" %>
<%@ Import Namespace="N2.Addons.Tagging.UI"%>
<asp:Content ID="cpc" ContentPlaceHolderID="Content" runat="server">
<h1><%= CurrentPage.Title %></h1>
<ul><% foreach(N2.Addons.Tagging.Items.Tag item in CurrentPage.GetChildren()) { %>
	<li><%= N2.Web.Link.To(item) %> (<%= item.ReferenceCount %>)</li>
<% } %></ul>
</asp:Content>
