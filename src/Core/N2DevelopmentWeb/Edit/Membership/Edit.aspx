<%@ Page Language="C#" MasterPageFile="../Framed.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="N2.Edit.Membership.Edit" Title="Edit user" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="Css/membership.css" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Toolbar" runat="server">
   <asp:LinkButton ID="btnSave" runat="server" OnClick="btnSave_Click" CssClass="command">save</asp:LinkButton>
   <asp:HyperLink ID="hlBack" runat="server" NavigateUrl="Users.aspx" CssClass="command">cancel</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content" runat="server">
    <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" Text="Email" />
    <asp:TextBox ID="txtEmail" runat="server" />
    <asp:Label ID="lblRoles" runat="server" AssociatedControlID="cblRoles" Text="Roles" />
    <asp:CheckBoxList ID="cblRoles" runat="server" CssClass="cbl" DataSourceID="odsRoles" />
    <asp:ObjectDataSource ID="odsRoles" runat="server" TypeName="System.Web.Security.Roles" SelectMethod="GetAllRoles" />
</asp:Content>
