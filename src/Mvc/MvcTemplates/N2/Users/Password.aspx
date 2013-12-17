﻿<%@ Page Language="C#" MasterPageFile="../Content/Framed.Master" AutoEventWireup="true" CodeBehind="Password.aspx.cs" Inherits="N2.Edit.Membership.Password" Title="Change password" meta:resourcekey="PageResource1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="Css/membership.css" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Toolbar" runat="server">
    <asp:LinkButton ID="btnSave" runat="server" OnClick="btnSave_Click" 
		CssClass="command primary-action" meta:resourcekey="btnSaveResource1">Save</asp:LinkButton>
    <asp:LinkButton ID="btnUnlock" runat="server" OnClick="btnUnlock_Click" 
		CssClass="command command-action" meta:resourcekey="btnUnlockResource1">Unlock</asp:LinkButton>
    <asp:HyperLink ID="hlBack" runat="server" NavigateUrl="Users.aspx" 
		CssClass="command cancel" meta:resourcekey="hlBackResource1">Cancel</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content" runat="server">
	<div class="tabPanel">
    <asp:Label ID="Label1" runat="server" AssociatedControlID="txtPassword" 
		Text="New password" meta:resourcekey="Label1Resource1"></asp:Label>
    <asp:TextBox ID="txtPassword" runat="server" 
		meta:resourcekey="txtPasswordResource1"></asp:TextBox>
	</div>
</asp:Content>
