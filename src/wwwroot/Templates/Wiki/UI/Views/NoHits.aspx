<%@ Page Language="C#" MasterPageFile="~/Templates/UI/Layouts/Top+SubMenu.master" AutoEventWireup="true" CodeBehind="NoHits.aspx.cs" Inherits="N2.Templates.Wiki.UI.Views.NoHits" Title="Untitled Page" %>
<%@ Import Namespace="N2.Templates.Wiki.UI.Views"%>
<asp:Content ContentPlaceHolderID="TextContent" runat="server">
    <h1>No hits for '<%= CurrentPage.ActionParameter %>'</h1>
    <n2:EditableDisplay ID="edText" runat="server" PropertyName="NoHitsText" Path="<%$ CurrentPage: WikiRoot.Path %>" />
</asp:Content>
