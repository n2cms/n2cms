<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Upgrade.aspx.cs" Inherits="N2.Edit.Install.Upgrade" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Upgrade N2</title>
    <link rel="stylesheet" type="text/css" href="../Resources/css/all.css" />
    <link rel="stylesheet" type="text/css" href="../Resources/css/framed.css" />
    <link rel="stylesheet" type="text/css" href="../Resources/css/themes/default.css" />
    <style>
    	form{font-size:1.1em;width:800px;margin:10px auto;}
    	a{color:#00e;}
    	li{margin-bottom:10px}
    	form{padding:20px}
    	.warning{color:#f00;}
    	.ok{color:#0c0;}
    	textarea{width:95%;height:120px;border:none;background-color:#FFB}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<p style="background-color:salmon;font-weight:bold;padding:5px">This is an early alpha of the upgrade wizard. Please help out testing this feature.</p>
        <n2:TabPanel ID="TabPanel1" ToolTip="Upgrade" runat="server">
			<h1>Upgrade database from <%= Status.DatabaseVersion %> to <%= N2.Installation.DatabaseStatus.RequiredDatabaseVersion %></h1>
			
			<% if (Status.NeedsUpgrade) { %>
			<p class="warning">The database needs to be upgraded.</p>
			<% } else {%>
			<p class="ok">The database looks fine. Happy <a href="..">editing</a>.</p>
			<%} %>
			
			<p>Please review the following upgrade script carefully before continuing to update the database below.</p>
			<textarea readonly="readonly"><%= Installer.ExportUpgradeSchema() %></textarea>
            <p>
				The script looks fine please 
				<asp:Button ID="btnUpgrade" runat="server" OnClick="btnInstall_Click" Text="update tables" OnClientClick="return confirm('Updating the database makes changes to the information on the site. I confirm that everything is backed-up and want to continue.');" ToolTip="Click this button to update the database" CausesValidation="false"/>
				in my database.
			</p>
			<p>
				Optionally I can 
				<asp:Button ID="btnExport" runat="server" OnClick="btnExportSchema_Click" Text="download the SQL script" ToolTip="Click this button to generate update database script" CausesValidation="false" />
				for the connection type <strong><%= Status.ConnectionType %></strong> and create the tables myself.
            </p>
		</n2:TabPanel>
        <asp:Label EnableViewState="false" ID="errorLabel" runat="server" CssClass="errorLabel" />
    </div>
    </form>
</body>
</html>
