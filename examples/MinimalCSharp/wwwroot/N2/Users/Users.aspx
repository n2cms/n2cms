<%@ Page Language="C#" MasterPageFile="../Content/Framed.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="N2.Edit.Membership.Users" Title="Users" meta:resourcekey="PageResource1" %>
<asp:Content ID="ContentHead" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Toolbar" runat="server">
    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="New.aspx" CssClass="command" meta:resourcekey="HyperLinkResource1">new</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content" runat="server">
	<asp:DataGrid ID="dgrUsers" runat="server" DataSourceID="odsUsers" 
		AutoGenerateColumns="False" UseAccessibleHeader="True" 
		OnItemCommand="odsUsers_ItemCommand" DataKeyField="UserName" BorderWidth="0px" 
		CssClass="gv" meta:resourcekey="dgrUsersResource1">
        <Columns>
            <asp:HyperLinkColumn DataNavigateUrlField="UserName" DataTextField="UserName" DataNavigateUrlFormatString="Edit.aspx?user={0}" meta:resourcekey="bcUserName"/>
            <asp:BoundColumn DataField="Email" HeaderText="email" meta:resourcekey="bcEmail"/>
            <asp:BoundColumn DataField="CreationDate" HeaderText="created" meta:resourcekey="bcCreated"/>
            <asp:HyperLinkColumn DataNavigateUrlField="UserName" 
				DataNavigateUrlFormatString="Password.aspx?user={0}" Text="password" 
				meta:resourcekey="HyperLinkColumnResource1" />
            <asp:ButtonColumn Text="delete" CommandName="Delete" 
				meta:resourcekey="ButtonColumnResource1" />
			<asp:TemplateColumn>
				<ItemTemplate>
				    <div><%# Eval("Comment") %></div>
				    <%# (bool)Eval("IsOnline") ? "<span title='Online'>O</span>" : string.Format("<span title='Offline, last login: {0}'>F</span>", Eval("LastLoginDate"))%>
				    <%# (bool)Eval("IsLockedOut") ? string.Format("<span title='Locked out: {0}'>L</span>", Eval("LastLockoutDate")) : ""%>
				    <%# (bool)Eval("IsApproved") ? "" : "<span title='Not Approved'>A</span>"%>
				</ItemTemplate>
			</asp:TemplateColumn>
        </Columns>
    </asp:DataGrid>
    <asp:ObjectDataSource ID="odsUsers" runat="server" TypeName="System.Web.Security.Membership" SelectMethod="GetAllUsers" />
</asp:Content>