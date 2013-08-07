<%@ Page Language="C#" MasterPageFile="../Content/Framed.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="N2.Edit.Membership.Edit" Title="Edit user" meta:resourcekey="PageResource1" %>
<%--<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="Css/membership.css" type="text/css" />
</asp:Content>--%>
<asp:Content ID="Content2" ContentPlaceHolderID="Toolbar" runat="server">
   <asp:LinkButton ID="btnSave" runat="server" OnClick="btnSave_Click" 
		CssClass="command primary-action" meta:resourcekey="btnSaveResource1">Save</asp:LinkButton>
   <asp:HyperLink ID="hlPassword" runat="server" NavigateUrl="Password.aspx" 
		CssClass="command action" meta:resourcekey="hlPasswordResource1">Password</asp:HyperLink>
   <asp:HyperLink ID="hlBack" runat="server" NavigateUrl="Users.aspx" 
		CssClass="command" meta:resourcekey="hlBackResource1">Cancel</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content" runat="server">
	<div class="tabPanel">
    <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" 
		Text="Email" meta:resourcekey="lblEmailResource1" />
    <asp:TextBox ID="txtEmail" runat="server" 
		meta:resourcekey="txtEmailResource1" />
    <asp:Label ID="lblRoles" runat="server" AssociatedControlID="cblRoles" 
		Text="Roles" meta:resourcekey="lblRolesResource1" />
    <asp:CheckBoxList ID="cblRoles" runat="server" CssClass="cbl" 
		DataSourceID="odsRoles" meta:resourcekey="cblRolesResource1" />
    <asp:ObjectDataSource ID="odsRoles" runat="server" TypeName="System.Web.Security.Roles" SelectMethod="GetAllRoles" />
	</div>
</asp:Content>
