<%@ Page Language="C#" MasterPageFile="Fallback.master" AutoEventWireup="true" CodeBehind="Submit.aspx.cs" Inherits="N2.Addons.Wiki.UI.Views.SubmitArticle" Title="Untitled Page" %>
<%@ Import Namespace="N2.Addons.Wiki.UI.Views"%>
<%@ Register TagPrefix="parts" TagName="EditArticle" Src="../Parts/EditArticle.ascx" %>
<asp:Content ContentPlaceHolderID="TextContent" runat="server">
    <parts:EditArticle runat="server" IsNew="true" Text="<%$ CurrentPage: WikiRoot.SubmitText %>" />
</asp:Content>

<asp:Content ID="sc" ContentPlaceHolderID="Sidebar" runat="server">
    <div id="extras" class="secondary">
        <n2:DroppableZone ID="zrr" ZoneName="RecursiveRight" runat="server" />
        <n2:DroppableZone ID="zr" ZoneName="Right" runat="server" />
        <n2:DroppableZone ID="zsr" ZoneName="SiteRight" runat="server" Path="~/" />
    </div>
</asp:Content>
