<%@ Page Language="C#" MasterPageFile="~/Layouts/Top+SubMenu.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Templates.News.UI.Default" %>
<asp:Content ContentPlaceHolderID="TextContent" runat="server">
    <span class="date"><%= CurrentItem.Published %></span>
    <p class="intro"><n2:EditableDisplay PropertyName="Introduction" runat="server" /></p>
    <n2:EditableDisplay PropertyName="Text" runat="server" />
</asp:Content>