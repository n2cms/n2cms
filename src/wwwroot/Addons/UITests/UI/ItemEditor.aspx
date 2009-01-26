<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/UI/Layouts/Top+SubMenu.Master" AutoEventWireup="true" CodeBehind="ItemEditor.aspx.cs" Inherits="N2.Addons.UITests.UI.ItemEditor" %>
<asp:Content ID="Content8" ContentPlaceHolderID="Content" runat="server">
	<n2:ItemEditor ID="ie" runat="server" />
	<asp:Button runat="server" Text="Save" OnClick="Save_Click" />
</asp:Content>
