<%@ Page Language="C#" MasterPageFile="~/Templates/UI/Layouts/Top+SubMenu.master" AutoEventWireup="true" CodeBehind="Version.aspx.cs" Inherits="N2.Templates.Wiki.UI.Version" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TextContent" runat="server">
    <h1><%= CurrentVersion.Title %> (version #<%= CurrentVersion.ID %>)</h1>
    <n2:Display runat="server" PropertyName="Text" CurrentItem='<%$ Code: CurrentVersion %>' />
</asp:Content>
