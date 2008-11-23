<%@ Page Language="C#" MasterPageFile="~/Templates/UI/Layouts/Top+SubMenu.master" AutoEventWireup="true" CodeBehind="Wiki.aspx.cs" Inherits="N2.Addons.Wiki.UI.Views.Wiki" Title="Untitled Page" %>
<%@ Import Namespace="N2.Addons.Wiki.UI.Views"%>
<asp:Content ContentPlaceHolderID="TextContent" runat="server">
    <n2:EditableDisplay PropertyName="Title" runat="server" />
    <n2:EditableDisplay PropertyName="Text" runat="server" />
</asp:Content>