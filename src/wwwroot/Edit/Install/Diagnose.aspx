<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Diagnose.aspx.cs" Inherits="N2.Edit.Install.Diagnose" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Diagnose N2</title>
    <link rel="stylesheet" href="../Css/All.css" type="text/css" />
    <style>
        label{font-weight:bold;margin:5px 10px 0 0;}
        input{vertical-align:middle;margin-bottom:5px;}
        ul,li{margin-top:0;margin-bottom:0;}
        textarea{height:55px;width:70%;background-color:salmon;border:none;font-size:11px}
        .defs td{font-size:.75em;vertical-align:top;text-alignment:left;border:solid 1px silver;}
        .EnabledFalse { color:#999; }
        .IsDefinedFalse { color:Red; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>Diagnose N2</h1>
            <em>This page is used to upgrade N2 database schema or diagnose configuration problems. If you don't get a lot of okays here there probably is some kind of problem. Look at any error message to find clues about what's wrong.</em> <a href="default.aspx">Back to edit page</a><br /><br />

			<!-- Database -->	
            <label>Connection</label> <asp:Label ID="lblDbConnection" runat="server" /><br />
			
			<!-- root item -->	
            <label>Root item</label> <asp:Label ID="lblRootNode" runat="server" /><br />
            <label>Start page</label> <asp:Label ID="lblStartNode" runat="server" /><br />
            <br />
			<!-- assemblyl versions -->	
            <label>N2 version</label> <asp:Label ID="lblN2Version" runat="server" /><br />
            <label>N2.Edit version</label> <asp:Label ID="lblEditVersion" runat="server" /><br />
            <br />
			<!--  -->	
            <label>Last error</label><asp:Label ID="lblError" runat="server" /><br />
            
            <h2>Database operations</h2>
            <i>These buttons can be used to create N2 tables in the database configured in the connection string. Be careful. From here you can remove all content in one click.</i><br />

			<table>
			<tr><td>
				<label>Restart web application</label>
			</td><td>
				<asp:Button ID="btnRestart" runat="server" OnClick="btnRestart_Click" Text="RESTART" OnClientClick="return confirm('restart site?');" />
			</td></tr>
           
			<tr><td>
				<label>Drop tables clearing all content data in database</label>
			</td><td>
				<asp:Button ID="btnClearTables" runat="server" OnClick="btnClearTables_Click" Text="DROP" OnClientClick="return confirm('drop all tables?');" />
				<asp:Label runat="server" ID="lblClearTablesResult" />
			</td></tr>

			<tr><td>
				<label>Create database schema (this drops any existing tables)</label>
			</td><td>
				<asp:Button ID="btnAddSchema" runat="server" OnClick="btnAddSchema_Click" Text="CREATE" OnClientClick="return confirm('drop and recreate all tables?');" />
				<asp:Label runat="server" ID="lblAddSchemaResult" />
			</td></tr>
	            
			<tr><td>
				<label>Upgrade database (from version)</label>
			</td><td>
				<asp:TextBox ID="txtPreviousVersion" Text="1.0.0.0" runat="server" />
				<asp:Button ID="btnUpgrade" runat="server" OnClick="btnUpgrade_Click" Text="Upgrade" />
				<asp:Label runat="server" ID="lblUpgradeResult" />
			</td></tr>

			<tr><td>
	            <label>Insert root node</label>
			</td><td>
				<asp:Button ID="btnInsert" runat="server" OnClick="btnInsert_Click" Text="Select type..." />
				<asp:DropDownList ID="ddlTypes" runat="server" AutoPostBack="True" Visible="False">
				</asp:DropDownList><asp:Button runat="server" ID="btnInsertRootNode" Text="OK" Visible="false" OnClick="btnInsertRootNode_Click" />
				<asp:Label ID="lblInsert" runat="server"></asp:Label><br />
			</td></tr>
			</table>



            <h2>Definitions</h2>
            <i>These settings are generated at application start from attributes in the project source code.</i>
            <asp:Repeater ID="rptDefinitions" runat="server">
                <HeaderTemplate>
					<table class="defs">
						<thead>
							<tr>
								<td colspan="2">Definition</td>
								<td colspan="2">Zones</td>
								<td colspan="2">Details</td>
							</tr>
							<tr>
								<td>Type</td>
								<td>Allowed children</td>
								<td>Available</td>
								<td>Allowed in</td>
								<td>Editables</td>
								<td>Displayables</td>
							</tr>
						</thead>
						<tbody>
				</HeaderTemplate>
                <ItemTemplate>
                    <tr class="<%# Eval("Enabled", "Enabled{0}") %> <%# Eval("IsDefined", "IsDefined{0}") %>"><td>
                        <b><%# Eval("Title") %></b> - <%# Eval("ItemType") %> (<%# Eval("Discriminator") %>)
                    </td><td>
                        <!-- Child definitions -->
                        <asp:Repeater ID="Repeater1" runat="server" DataSource='<%# Eval("AllowedChildren") %>'>
                            <ItemTemplate> * <%# Eval("Title")%><br></ItemTemplate>
                        </asp:Repeater>
                    </td><td>
                        <!-- Available zones -->
                        <asp:Repeater ID="Repeater2" runat="server" DataSource='<%# Eval("AvailableZones") %>'>
                            <ItemTemplate> * <%# Eval("ZoneName") %> (<%# Eval("Title") %>)<br></ItemTemplate>
                        </asp:Repeater>
                    </td><td>
						<b><%# Eval("AllowedIn")%>: </b><br />
                        <!-- Allowed in zone -->
                        <asp:Repeater ID="Repeater3" runat="server" DataSource='<%# Eval("AllowedZoneNames") %>'>
                            <ItemTemplate> * <%# Container.DataItem %><br></ItemTemplate>
                        </asp:Repeater>
                    </td><td>
                        <!-- Editable attributes -->
                        <asp:Repeater ID="Repeater4" runat="server" DataSource='<%# Eval("Editables") %>'>
                            <ItemTemplate> * <%# Eval("Title")%> (<%# Eval("Name")%>)<br></ItemTemplate>
                        </asp:Repeater>
                    </td><td>
                        <!-- Displayable attributes -->
                        <asp:Repeater ID="Repeater5" runat="server" DataSource='<%# Eval("Displayables") %>'>
                            <ItemTemplate> * <%# ((N2.Details.IDisplayable)Container.DataItem).Name %><br></ItemTemplate>
                        </asp:Repeater>
                    </td></tr>
                </ItemTemplate>
                <FooterTemplate>
						</tbody>
					</table>
				</FooterTemplate>
            </asp:Repeater>
            <asp:Label ID="lblDefinitions" runat="server" />
        </div>
    </form>
</body>
</html>
