<%@ Page Language="C#" MasterPageFile="~/Templates/UI/Layouts/Top+SubMenu.master" AutoEventWireup="true" CodeBehind="Wiki.aspx.cs" Inherits="N2.Templates.Wiki.UI.Wiki" Title="Untitled Page" %>
<asp:Content ContentPlaceHolderID="TextContent" runat="server">
    <n2:EditableDisplay PropertyName="Title" runat="server" />
    <n2:EditableDisplay PropertyName="Text" runat="server" />
</asp:Content>