<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Fix.aspx.cs" Inherits="N2.Edit.Install.Fix" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<asp:DropDownList ID="ddlCNs" runat="server" DataTextField="Name" DataValueField="ConnectionString"></asp:DropDownList>
		
		<asp:SqlDataSource ID="sdsItems" runat="server" 
			ProviderName="System.Data.SqlClient" 
			SelectCommand="SELECT * FROM [n2Item] ORDER BY [ParentID], [Name]"></asp:SqlDataSource>
		<asp:GridView ID="GridView1" runat="server" DataSourceID="sdsItems" PageSize="50"
			AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" 
			DataKeyNames="ID">
			<Columns>
				<asp:CommandField ShowSelectButton="True" />
				<asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False" 
					ReadOnly="True" SortExpression="ID" />
				<asp:BoundField DataField="Type" HeaderText="Type" SortExpression="Type" />
				<asp:BoundField DataField="Updated" HeaderText="Updated" 
					SortExpression="Updated" />
				<asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
				<asp:BoundField DataField="ZoneName" HeaderText="ZoneName" 
					SortExpression="ZoneName" />
				<asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title" />
				<asp:BoundField DataField="Created" HeaderText="Created" 
					SortExpression="Created" />
				<asp:BoundField DataField="Published" HeaderText="Published" 
					SortExpression="Published" />
				<asp:BoundField DataField="Expires" HeaderText="Expires" 
					SortExpression="Expires" />
				<asp:BoundField DataField="SortOrder" HeaderText="SortOrder" 
					SortExpression="SortOrder" />
				<asp:CheckBoxField DataField="Visible" HeaderText="Visible" 
					SortExpression="Visible" />
				<asp:BoundField DataField="SavedBy" HeaderText="SavedBy" 
					SortExpression="SavedBy" />
				<asp:BoundField DataField="VersionOfID" HeaderText="VersionOfID" 
					SortExpression="VersionOfID" />
				<asp:BoundField DataField="ParentID" HeaderText="ParentID" 
					SortExpression="ParentID" />
			</Columns>
		</asp:GridView>
    
    </div>
    </form>
</body>
</html>
