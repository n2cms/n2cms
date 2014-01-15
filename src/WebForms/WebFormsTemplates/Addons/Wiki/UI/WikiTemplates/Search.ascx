<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Search.ascx.cs" Inherits="N2.Addons.Wiki.UI.WikiTemplates.Search" %>
<%@ Register TagPrefix="n2" Namespace="N2.Templates.Web.UI.WebControls" %>
<h4>Search</h4>
<n2:Box runat="server" DefaultButton="btnSearch">
    <asp:TextBox ID="txtSearch" runat="server" CssClass="tb" />
    <asp:Button ID="btnSearch" Text="OK" runat="server" onclick="btnSearch_Click" />
</n2:Box>