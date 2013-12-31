<%@ Page Language="C#" MasterPageFile="Fallback.master" AutoEventWireup="true" CodeBehind="NoHits.aspx.cs" Inherits="N2.Addons.Wiki.UI.Views.NoHits" Title="Untitled Page" %>
<%@ Import Namespace="N2.Addons.Wiki.UI.Views"%>
<asp:Content ContentPlaceHolderID="TextContent" runat="server">
    <h1>No hits for '<%= CurrentArguments %>'</h1>
    <n2:EditableDisplay ID="edText" runat="server" PropertyName="NoHitsText" Path="<%$ CurrentPage: WikiRoot.Path %>" />
</asp:Content>

<asp:Content ID="sc" ContentPlaceHolderID="Sidebar" runat="server">
    <div id="extras" class="secondary">
        <n2:DroppableZone ID="zrr" ZoneName="RecursiveRight" runat="server" />
        <n2:DroppableZone ID="zr" ZoneName="Right" runat="server" />
        <n2:DroppableZone ID="zsr" ZoneName="SiteRight" runat="server" Path="~/" />
    </div>
</asp:Content>
