<%@ Page Language="C#" MasterPageFile="../Layouts/Top+SubMenu.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Templates.News.UI.NewsItem" %>
<asp:Content ContentPlaceHolderID="TextContent" runat="server">
    <n2:EditableDisplay ID="edt" PropertyName="Title" runat="server" />
    <span class="date"><%= CurrentItem.Published %></span>
    <n2:EditableDisplay PropertyName="Introduction" runat="server">
        <HeaderTemplate><p class="introduction"></HeaderTemplate>
        <FooterTemplate></p></FooterTemplate>
    </n2:EditableDisplay>
    <n2:EditableDisplay PropertyName="Text" runat="server" />
</asp:Content>
