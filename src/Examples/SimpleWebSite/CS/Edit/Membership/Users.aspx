<%@ Page Language="C#" MasterPageFile="../Framed.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="N2.Edit.Membership.Users" Title="Users" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="../Css/gridView.css" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Toolbar" runat="server">
    <asp:HyperLink runat="server" NavigateUrl="New.aspx" CssClass="command">new</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content" runat="server">
    <asp:DataGrid ID="dgrUsers" runat="server" DataSourceID="odsUsers" AutoGenerateColumns="false" UseAccessibleHeader="true" OnItemCommand="odsUsers_ItemCommand" DataKeyField="UserName" BorderWidth="0" CssClass="gv">
        <Columns>
            <asp:BoundColumn DataField="UserName" HeaderText="User name" />
            <asp:BoundColumn DataField="Email" HeaderText="email" />
            <asp:BoundColumn DataField="CreationDate" HeaderText="created" />
            <asp:BoundColumn DataField="IsOnline" HeaderText="online" />
            <asp:BoundColumn DataField="IsLockedOut" HeaderText="locked out" />
            <asp:BoundColumn DataField="IsApproved" HeaderText="approved" />
            <asp:BoundColumn DataField="LastLockoutDate" HeaderText="last locked out" />
            <asp:BoundColumn DataField="LastLoginDate" HeaderText="last login" />
            <asp:BoundColumn DataField="LastActivityDate" HeaderText="last activity" />
            <asp:BoundColumn DataField="LastPasswordChangedDate" HeaderText="last password change" />
            <asp:BoundColumn DataField="Comment" HeaderText="comment" />
            <asp:HyperLinkColumn DataNavigateUrlField="UserName" DataNavigateUrlFormatString="Password.aspx?user={0}" Text="password" />
            <asp:HyperLinkColumn DataNavigateUrlField="UserName" DataNavigateUrlFormatString="Edit.aspx?user={0}" Text="edit" />
            <asp:ButtonColumn Text="delete" CommandName="Delete" />
        </Columns>
    </asp:DataGrid>
    <asp:ObjectDataSource ID="odsUsers" runat="server" TypeName="System.Web.Security.Membership" SelectMethod="GetAllUsers" />
</asp:Content>
