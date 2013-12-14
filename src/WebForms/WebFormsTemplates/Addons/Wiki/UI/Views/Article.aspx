<%@ Page Language="C#" MasterPageFile="Fallback.master" AutoEventWireup="true" CodeBehind="Article.aspx.cs" Inherits="N2.Addons.Wiki.UI.Views.WikiArticle" Title="Untitled Page" %>
<%@ Import Namespace="N2.Addons.Wiki.UI.Views"%>
<asp:Content ID="tc" ContentPlaceHolderID="TextContent" runat="server">
    <div class="controls">View
    | <a href="<%= CurrentPage.AppendUrl("Modify") %>">Modify</a>
    | <a href="<%= CurrentPage.AppendUrl("History") %>">History</a></div>
    <n2:Display PropertyName="Title" runat="server" />
    <n2:Display PropertyName="Text" runat="server" />
</asp:Content>
<asp:Content ID="sc" ContentPlaceHolderID="Sidebar" runat="server">
    <div id="extras" class="secondary">
        <n2:DroppableZone ID="zrr" ZoneName="RecursiveRight" runat="server" />
        <n2:DroppableZone ID="zr" ZoneName="Right" runat="server" />
        <n2:DroppableZone ID="zsr" ZoneName="SiteRight" runat="server" Path="~/" />
    </div>
</asp:Content>
