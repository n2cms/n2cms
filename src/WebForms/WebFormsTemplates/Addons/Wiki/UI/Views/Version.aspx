<%@ Page Language="C#" MasterPageFile="Fallback.master" AutoEventWireup="true" CodeBehind="Version.aspx.cs" Inherits="N2.Addons.Wiki.UI.Views.Version" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TextContent" runat="server">
    <h1><%= CurrentVersion.Title %> (version #<%= CurrentVersion.ID %>)</h1>
    <n2:Display runat="server" PropertyName="Text" CurrentItem='<%$ Code: CurrentVersion %>' />
</asp:Content>

<asp:Content ID="sc" ContentPlaceHolderID="Sidebar" runat="server">
    <div id="extras" class="secondary">
        <n2:DroppableZone ID="zrr" ZoneName="RecursiveRight" runat="server" />
        <n2:DroppableZone ID="zr" ZoneName="Right" runat="server" />
        <n2:DroppableZone ID="zsr" ZoneName="SiteRight" runat="server" Path="~/" />
    </div>
</asp:Content>
