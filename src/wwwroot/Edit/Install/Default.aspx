<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Install._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Install N2</title>
    <link rel="stylesheet" type="text/css" href="../css/all.css" />
    <link rel="stylesheet" type="text/css" href="../css/framed.css" />
    <style>
    	form{font-size:1.1em;width:800px;margin:10px auto;}
    	a{color:#00e;}
    	li{margin-bottom:10px}
    	form{padding:20px}
    	.warning{color:#f00;}
    	.ok{color:#0d0;}
    	textarea{width:80%;height:120px}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Literal ID="ltStartupError" runat="server" />

        <n2:TabPanel ToolTip="1. Welcome" runat="server">
            <h1>Install N2 CMS</h1>
			<p class='<%# Status.IsInstalled ? "ok" : "warning" %>'>
				<b> Advice: </b> <%# GetStatusText() %>
            </p>
            <p>To install N2 you need to create a database, update the connection string in web.config, create the tables needed by N2, add the root node to the database and make sure the root node's id is configured in web.config.</p>
            <p>The following tabs will help you in these. Just continue to tab 2.</p>
            <h3>System status</h3>
            <p><%# Status.ToStatusString() %></p>
        </n2:TabPanel>
        
        <n2:TabPanel ToolTip="2. Database connection" runat="server">
            <h1>Check database connection</h1>
            <asp:Literal runat="server" Visible='<%# Status.IsConnected %>'>
				<p class="ok"><b>Advice: </b>Since your database seems connected you may skip this step.</p>
            </asp:Literal>
            <p>
                First make sure you have database available and <a href="http://n2cms.com/Documentation/Connection strings.aspx">configure connection string and database dialect</a> in web.config.
            </p>
            <p>Once you're done you can <asp:Button ID="btnTest" runat="server" OnClick="btnTest_Click" Text="test the connection" CausesValidation="false" /></p>
            <p>
                <asp:Label ID="lblStatus" runat="server" />
            </p>
        </n2:TabPanel>
        
        <n2:TabPanel ToolTip="3. Database tables" runat="server">
			<h1>Create datbase tables</h1>
            <asp:Literal runat="server" Visible='<%# Status.HasSchema %>'>
				<p class="ok"><b>Advice: </b>The database tables are okay. You can move to the next tab (if you create them again you will delete any existing content).</p>
            </asp:Literal>
            <asp:Literal runat="server" Visible='<%# !Status.IsConnected %>'>
				<p class="warning"><b>Advice: </b>Go back and check database connection.</p>
            </asp:Literal>
            <p>
				Assuming the database connection is okay (step 2) you can 
				<asp:Button ID="btnInstall" runat="server" OnClick="btnInstall_Click" Text="create tables" OnClientClick="return confirm('Creating database tables will destory any existing data. Are you sure?');" ToolTip="Click this button to install database" CausesValidation="false" />
				or 
				<asp:Button ID="btnExport" runat="server" OnClick="btnExportSchema_Click" Text="generate sql script" ToolTip="Click this button to generate create database schema script" CausesValidation="false" />
				for the connection type <%= Status.ConnectionType %>.
				You can also download sql scripts from 
                <a href="http://n2cms.com/Documentation/The database.aspx">the n2 cms home page</a> 
                (if you're using MySQL this is the preferred option).
            </p>
            <p>
                <asp:Label CssClass="ok" runat="server" ID="lblInstall" />
            </p>
        </n2:TabPanel>
        
        <n2:TabPanel ToolTip="4. Root node" runat="server">
			<h1>Insert root node (required)</h1>
            <asp:Literal runat="server" Visible='<%# Status.IsInstalled %>'>
				<p class="ok"><b>Advice: </b>There root and start nodes are configured and present in the database. If you create more they will become detached nodes cluttering your database unless you point them out in web.config (which makes the existing nodes detached instead).</p>
            </asp:Literal>
            <asp:Literal runat="server" Visible='<%# !Status.HasSchema %>'>
				<p class="warning"><b>Advice: </b>Go back and check database connection and tables.</p>
            </asp:Literal>
            <p>
                N2 needs a root node and a start page in order to function correctly. These two "nodes" may be the same page for simple sites, e.g. if you don't forsee using multiple domains.
            </p>
            <ul>
                <li>
					Either, Select one 
					<asp:DropDownList ID="ddlRoot" runat="server" />
					and one 
					<asp:DropDownList ID="ddlStartPage" runat="server" />
					to 
					<asp:Button ID="btnInsert" runat="server" OnClick="btnInsert_Click" Text="insert" ToolTip="Insert different root and start nodes" CausesValidation="false" />
					as <b>two different</b> nodes.
				    <asp:CustomValidator ID="cvRootAndStart" runat="server" ErrorMessage="Root and start type required" Display="Dynamic" />
				</li>
                <li>
					Or, use the <b>one node</b> for both
					<asp:DropDownList ID="ddlRootAndStart" runat="server" />
					to
					<asp:Button ID="btnInsertRootOnly" runat="server" OnClick="btnInsertRootOnly_Click" Text="insert" ToolTip="Insert one node as root and start" CausesValidation="false" />.
				    <asp:CustomValidator ID="cvRoot" runat="server" ErrorMessage="Root type required" Display="Dynamic" />
				</li>
				<li>
					<table><tr><td>
						Or, select one of these existing export file to insert:
					</td><td>
						<asp:RadioButtonList ID="rblExports" runat="server" />
					</td><td>
						and 
						<asp:Button ID="btnInsertExport" runat="server" OnClick="btnInsertExport_Click" Text="insert" ToolTip="Insert existing export" CausesValidation="false" />
						<asp:CustomValidator ID="cvExisting" runat="server" ErrorMessage="Select an export file" Display="Dynamic" />
					</td></tr></table>
				</li>
                <li>
					Or, select an export file 
					<asp:FileUpload ID="fileUpload" runat="server" />
					(*.n2.xml) to
					<asp:Button ID="btnUpload" runat="server" OnClick="btnUpload_Click" Text="upload and insert" ToolTip="Upload root node." CausesValidation="false" />
					<asp:RequiredFieldValidator ID="rfvUpload" ControlToValidate="fileUpload" runat="server" Text="Select import file" Display="Dynamic" />
				</li>
            </ul>
			<p>
                <asp:Literal ID="ltRootNode" runat="server" />  
            </p>
<asp:PlaceHolder ID="phSame" runat="server" Visible="false">
            <h4>Example web.config with same root as start page</h4>
            <p>
                <textarea rows="4">
...
	<n2>
		<host rootID="<%# rootId %>" startPageID="<%# startId %>"/>
		...
	</n2>
...</textarea>
				<asp:Button runat="server" OnClick="btnUpdateWebConfig_Click" Text="Update web.config" />
            </p>
</asp:PlaceHolder>
<asp:PlaceHolder ID="phDiffer" runat="server" Visible="false">
            <h4>Example web.config with different root as start pages</h4>
            <p>
                <textarea rows="4">
...
	<n2>
		<host rootID="<%# rootId %>" startPageID="<%# startId %>"/>
		...
	</n2>
...</textarea>
				<asp:Button runat="server" OnClick="btnUpdateWebConfig_Click" Text="Update web.config" />
            </p>
</asp:PlaceHolder>
			<p><asp:Label runat="server" ID="lblWebConfigUpdated" /></p>
        </n2:TabPanel>
        
        <n2:TabPanel runat="server" tooltip="5. Finishing touches">
			<h1>Almost done!</h1>
            <p><b>IMPORTANT!</b> Change the default password in web.config. If you've installed, configured and created an administrator account using a membership provider, comment out this section entirely.</p>
            <p><b>Advice:</b> remove the installation directory (/edit/install) to prevent nasty surprises.</p>
            <p>It's advisable to 
				<asp:Button runat="server" OnClick="btnRestart_Click" Text="restart" CausesValidation="false" />
				before you continue.
            </p>
            <p>Good luck and happy <a href="..">editing</a>.</p>
            <p>/Cristian</p>
        </n2:TabPanel>
        <asp:Label ID="errorLabel" runat="server" CssClass="errorLabel" />
    </form>
</body>
</html>
