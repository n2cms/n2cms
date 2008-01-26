<%@ Page Language="C#" MasterPageFile="~/DefaultMasterPage.Master" AutoEventWireup="true" CodeBehind="ItemSelector.aspx.cs" Inherits="N2.TemplateWeb.Templates.ItemSelector" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<n2:ItemSelector ID="itse" runat="server" />
	<asp:Button ID="bu" runat="server" Text="postback" />
	<%# itse.SelectedItem %>
</asp:Content>
