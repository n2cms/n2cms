<%@ Page Language="C#" MasterPageFile="../Framed.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="N2.Edit.Membership.Users" Title="Users" meta:resourcekey="PageResource1" %>
<asp:Content ID="ContentHead" ContentPlaceHolderID="Head" runat="server">
	<script src="../Js/plugins.ashx" type="text/javascript" ></script>
    <script src="../Js/plugins.ashx" type="text/javascript" ></script>
    <script type="text/javascript">
        $(document).ready(function(){
			toolbarSelect("users");
		});
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Toolbar" runat="server">
    <asp:HyperLink runat="server" NavigateUrl="New.aspx" CssClass="command" meta:resourcekey="HyperLinkResource1">new</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content" runat="server">
    <asp:DataGrid ID="dgrUsers" runat="server" DataSourceID="odsUsers" 
		AutoGenerateColumns="False" UseAccessibleHeader="True" 
		OnItemCommand="odsUsers_ItemCommand" DataKeyField="UserName" BorderWidth="0px" 
		CssClass="gv" meta:resourcekey="dgrUsersResource1">
        <Columns>
            <asp:BoundColumn DataField="UserName" HeaderText="User name" meta:resourcekey="bcUserName"/>
            <asp:BoundColumn DataField="Email" HeaderText="email" meta:resourcekey="bcEmail"/>
            <asp:BoundColumn DataField="CreationDate" HeaderText="created" meta:resourcekey="bcCreated"/>
            <asp:BoundColumn DataField="IsOnline" HeaderText="online" meta:resourcekey="bcOnline"/>
            <asp:BoundColumn DataField="IsLockedOut" HeaderText="locked out" meta:resourcekey="bcLockedOut"/>
            <asp:BoundColumn DataField="IsApproved" HeaderText="approved" meta:resourcekey="bcApproved" />
            <asp:BoundColumn DataField="LastLockoutDate" HeaderText="last locked out" meta:resourcekey="bcLockOutDate"/>
            <asp:BoundColumn DataField="LastLoginDate" HeaderText="last login" meta:resourcekey="bcLogin"/>
            <asp:BoundColumn DataField="LastActivityDate" HeaderText="last activity" meta:resourcekey="bcActivity"/>
            <asp:BoundColumn DataField="LastPasswordChangedDate" HeaderText="last password change" meta:resourcekey="bcPasswordChanged"/>
            <asp:BoundColumn DataField="Comment" HeaderText="comment" meta:resourcekey="bcComment"/>
            <asp:HyperLinkColumn DataNavigateUrlField="UserName" 
				DataNavigateUrlFormatString="Password.aspx?user={0}" Text="password" 
				meta:resourcekey="HyperLinkColumnResource1" />
            <asp:HyperLinkColumn DataNavigateUrlField="UserName" 
				DataNavigateUrlFormatString="Edit.aspx?user={0}" Text="edit" 
				meta:resourcekey="HyperLinkColumnResource2" />
            <asp:ButtonColumn Text="delete" CommandName="Delete" 
				meta:resourcekey="ButtonColumnResource1" />
        </Columns>
    </asp:DataGrid>
    <asp:ObjectDataSource ID="odsUsers" runat="server" TypeName="System.Web.Security.Membership" SelectMethod="GetAllUsers" />
</asp:Content>
