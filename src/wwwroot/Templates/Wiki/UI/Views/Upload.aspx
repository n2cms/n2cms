<%@ Page Language="C#" MasterPageFile="~/Templates/UI/Layouts/Top+SubMenu.master" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="N2.Templates.Wiki.UI.Views.Upload" Title="Untitled Page" %>
<%@ Import Namespace="N2.Templates.Wiki.UI.Views"%>
<asp:Content ID="Content1" ContentPlaceHolderID="TextContent" runat="server">
    <h1>Upload '<%= FileName%></h1>
    <asp:CustomValidator ID="cvUpload" runat="server" Text="Select a file to upload." Display="Dynamic"/>
    <asp:CustomValidator ID="cvExtension" runat="server" Text="The file does not have the correct extension." Display="Dynamic"/>
    <asp:PlaceHolder ID="phUpload" runat="server">
        <asp:FileUpload ID="fuUpload" runat="server" />
        <asp:Button ID="btnUpload" OnClick="btnUpload_Click" Text="Upload" runat="server" />
    </asp:PlaceHolder>
</asp:Content>
