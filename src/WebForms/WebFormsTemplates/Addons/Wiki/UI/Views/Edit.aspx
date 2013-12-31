<%@ Page Language="C#" MasterPageFile="Fallback.master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="N2.Addons.Wiki.UI.Views.EditArticle" Title="Untitled Page" %>
<%@ Import Namespace="N2.Addons.Wiki.UI.Views"%>
<%@ Register TagPrefix="parts" TagName="EditArticle" Src="../Parts/EditArticle.ascx" %>
<asp:Content ContentPlaceHolderID="TextContent" runat="server">
    <div class="controls"><a href="<%= CurrentPage.Url %>">View</a>
    | Modify
    | <a href="<%= CurrentPage.AppendUrl("History") %>">History</a>
    </div>
    <parts:EditArticle runat="server" IsNew="false" Text="<%$ CurrentPage: WikiRoot.ModifyText %>" />
</asp:Content>

<asp:Content ID="sc" ContentPlaceHolderID="Sidebar" runat="server">
    <div id="extras" class="secondary">
        <n2:DroppableZone ID="zrr" ZoneName="RecursiveRight" runat="server" />
        <n2:DroppableZone ID="zr" ZoneName="Right" runat="server" />
        <n2:DroppableZone ID="zsr" ZoneName="SiteRight" runat="server" Path="~/" />
    </div>
</asp:Content>
