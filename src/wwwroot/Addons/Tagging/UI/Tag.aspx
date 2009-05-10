<%@ Page Language="C#" MasterPageFile="~/Templates/UI/Layouts/Top+SubMenu.Master" AutoEventWireup="true" CodeBehind="Tag.aspx.cs" Inherits="N2.Addons.Tagging.UI.Tag" %>
<%@ Import Namespace="N2.Addons.Tagging.UI"%>
<asp:Content ID="cpc" ContentPlaceHolderID="PostContent" runat="server">
<ul><% foreach(N2.ContentItem item in TaggedItems) { %>
	<li><%= N2.Web.Link.To(item) %></li>
<% } %></ul>
</asp:Content>