<%@ Page Language="C#" MasterPageFile="../Framed.Master" AutoEventWireup="true" CodeBehind="Password.aspx.cs" Inherits="N2.Edit.Membership.Password" Title="Change password" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="Css/membership.css" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Toolbar" runat="server">
    <asp:LinkButton ID="btnSave" runat="server" OnClick="btnSave_Click" CssClass="command">save</asp:LinkButton>
    <asp:LinkButton ID="btnUnlock" runat="server" OnClick="btnUnlock_Click" CssClass="command">unlock</asp:LinkButton>
    <asp:HyperLink ID="hlBack" runat="server" NavigateUrl="Users.aspx" CssClass="command">cancel</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Outside" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content" runat="server">
    <asp:Label ID="Label1" runat="server" AssociatedControlID="txtPassword" Text="New password"></asp:Label>
    <asp:TextBox ID="txtPassword" runat="server"></asp:TextBox>
</asp:Content>
