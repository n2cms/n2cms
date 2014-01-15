<%@ Page Language="C#" MasterPageFile="Layout.master" AutoEventWireup="true" CodeBehind="Tag.aspx.cs" Inherits="N2.Addons.Tagging.UI.Tag" %>
<%@ Import Namespace="N2.Addons.Tagging.UI"%>
<asp:Content ID="cpc" ContentPlaceHolderID="Content" runat="server">
	<h1><%= CurrentPage.Parent.Title %>: <%= CurrentPage.Title %></h1>
<ul><% foreach(N2.ContentItem item in TaggedItems) { %>
	<li><%= N2.Web.Link.To(item) %></li>
<% } %></ul>
</asp:Content>
