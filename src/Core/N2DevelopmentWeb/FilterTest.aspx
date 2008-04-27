<%@ Page MasterPageFile="~/DefaultMasterPage.Master" Language="C#" AutoEventWireup="true" CodeBehind="FilterTest.aspx.cs" Inherits="N2DevelopmentWeb.FilterTest" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	access filter:
	<asp:GridView ID="gvAccess" runat="server" AutoGenerateColumns="false" DataSource='<%# new N2.Collections.ItemList(this.CurrentPage.Children, new N2.Collections.AccessFilter()) %>'>
		<Columns>
			<asp:BoundField DataField="Title" />
			<asp:BoundField DataField="Name" />
		</Columns>
	</asp:GridView>

	no filter:
	<asp:GridView ID="GridView4" runat="server" AutoGenerateColumns="false" DataSource='<%# this.CurrentPage.Children %>'>
		<Columns>
			<asp:BoundField DataField="Title" />
			<asp:BoundField DataField="Name" />
		</Columns>
	</asp:GridView>

	count filter:
	<asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" DataSource='<%# new N2.Collections.ItemList(this.CurrentPage.Children, new N2.Collections.CountFilter(2,2)) %>'>
		<Columns>
			<asp:BoundField DataField="Title" />
			<asp:BoundField DataField="Name" />
		</Columns>
	</asp:GridView>

	type filter:
	<asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="false" DataSource='<%# new N2.Collections.ItemList(this.CurrentPage.Children, new N2.Collections.TypeFilter(typeof(N2DevelopmentWeb.Domain.MyPageData))) %>'>
		<Columns>
			<asp:BoundField DataField="Title" />
			<asp:BoundField DataField="Name" />
		</Columns>
	</asp:GridView>
	
	zone filter:
	<asp:GridView ID="GridView3" runat="server" AutoGenerateColumns="false" DataSource='<%# new N2.Collections.ItemList(this.CurrentPage.Children, new N2.Collections.ZoneFilter("Right")) %>'>
		<Columns>
			<asp:BoundField DataField="Title" />
			<asp:BoundField DataField="Name" />
		</Columns>
	</asp:GridView>
	
</asp:Content>