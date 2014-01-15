<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/UI/Layouts/Top+SubMenu.Master" AutoEventWireup="true" CodeBehind="ItemEditor.aspx.cs" Inherits="N2.Addons.UITests.UI.ItemEditor" %>
<asp:Content ID="Content8" ContentPlaceHolderID="Content" runat="server">
	<a href="<%= N2.Web.Url.Parse(Request.RawUrl).AppendQuery("new", true) %>">Add new...</a>
	<n2:ItemEditor ID="ie" runat="server" />
	<asp:Button runat="server" Text="Save" OnClick="Save_Click" />
</asp:Content>
