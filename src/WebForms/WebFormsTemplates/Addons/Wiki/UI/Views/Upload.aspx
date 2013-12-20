<%@ Page Language="C#" MasterPageFile="Fallback.master" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="N2.Addons.Wiki.UI.Views.Upload" Title="Untitled Page" %>
<%@ Import Namespace="N2.Addons.Wiki.UI.Views"%>
<asp:Content ID="Content1" ContentPlaceHolderID="TextContent" runat="server">
    <h1>Upload '<%= FileName%></h1>
    <asp:CustomValidator ID="cvUpload" runat="server" Text="Select a file to upload." Display="Dynamic"/>
    <asp:CustomValidator ID="cvExtension" runat="server" Text="The file does not have the correct extension." Display="Dynamic"/>
    <asp:PlaceHolder ID="phUpload" runat="server">
        <asp:FileUpload ID="fuUpload" runat="server" />
        <asp:Button ID="btnUpload" OnClick="btnUpload_Click" Text="Upload" runat="server" />
    </asp:PlaceHolder>
</asp:Content>

<asp:Content ID="sc" ContentPlaceHolderID="Sidebar" runat="server">
    <div id="extras" class="secondary">
        <n2:DroppableZone ID="zrr" ZoneName="RecursiveRight" runat="server" />
        <n2:DroppableZone ID="zr" ZoneName="Right" runat="server" />
        <n2:DroppableZone ID="zsr" ZoneName="SiteRight" runat="server" Path="~/" />
    </div>
</asp:Content>
