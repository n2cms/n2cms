<%@ Page MasterPageFile="~/Layouts/Top+SubMenu.Master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Templates.Calendar.UI.Default" %>
<asp:Content ID="cc" ContentPlaceHolderID="TextContent" runat="server">
    <n2:EditableDisplay PropertyName="Title" runat="server" />
    <span class="date"><%= CurrentItem.EventDate %></span>
    <n2:EditableDisplay PropertyName="Introduction" runat="server">
        <HeaderTemplate><p class="introduction"></HeaderTemplate>
        <FooterTemplate></p></FooterTemplate>
    </n2:EditableDisplay>
    <n2:EditableDisplay PropertyName="Text" runat="server" />
</asp:Content>