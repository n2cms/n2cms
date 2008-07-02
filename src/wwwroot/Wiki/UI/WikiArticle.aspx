<%@ Page Language="C#" MasterPageFile="~/Layouts/Top+SubMenu.Master" AutoEventWireup="true" CodeBehind="WikiArticle.aspx.cs" Inherits="N2.Templates.Wiki.UI.WikiArticle" Title="Untitled Page" %>
<asp:Content ID="Content9" ContentPlaceHolderID="TextContent" runat="server">
    <n2:Display PropertyName="Title" runat="server" />
    <n2:Display PropertyName="Text" runat="server" />
</asp:Content>
