<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Upgrade.aspx.cs" Inherits="N2.Edit.Install.Upgrade" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Upgrade N2</title>
    <link rel="stylesheet" type="text/css" href="../Resources/Css/all.css" />
    <link rel="stylesheet" type="text/css" href="../Resources/Css/framed.css" />
    <link rel="stylesheet" type="text/css" href="../Resources/Css/themes/default.css" />
    <style>
    	form { font-size:1.1em;width:800px;margin:10px auto; }
    	a { color:#00e; }
    	li { margin-bottom:10px; }
    	form { padding:20px; }
    	.warning { color:#f00; }
    	.error { color:#f00; padding:5px; background-color:Yellow; }
    	.ok { color:#0c0; }
    	textarea { width:95%;height:120px;border:none;background-color:#FFB; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <n2:TabPanel ID="TabPanel1" ToolTip="Upgrade" runat="server">
			<h1>Upgrade database from <%= Status.DatabaseVersion %> to <%= N2.Edit.Installation.DatabaseStatus.RequiredDatabaseVersion%></h1>
			
			<% if (Status.NeedsUpgrade){ %>
			<p class="warning">The database needs to be upgraded.</p>
				<% if(Status.ConnectionType == "MySqlConnection") {%>
			<p class="warning">MySQL database might not be upgradeable from this interface. Execute <a href="mysql.upgrade.2.sql">mysql.upgrade.2.sql</a> with an SQL admin tool and manually execute migrations from advanced optins.</p>
				<% } %>
			<% } else if (!Status.IsInstalled){ %>
			<p class="error">No database to be upgraded. Please <a href="Default.aspx">install using the installation wizard</a>.</p>
			<hr />
			<% } else {%>
			<p class="ok">The database looks fine. Happy <a href="..">editing</a>.</p>
			<%} %>
			
			<p>Please review the following upgrade script carefully before continuing to update the database below.</p>
			<textarea readonly="readonly"><%= Installer.ExportUpgradeSchema() %></textarea>
			<p>In addition to updating the database schema, the following migrations will be executed on the <%= Status.Items %> items in your database: </p>
			<ul style="background-color:#FFB;">
				<% foreach (N2.Edit.Installation.AbstractMigration migration in Migrator.GetMigrations(Status)) { %>
				<li><strong><%= migration.Title %></strong> <%= migration.Description %></li>
				<%} %>
			</ul>
			<% if (Status.Items > 1000) { %>
			<p class="warning">The database contains a large number of items, the migration may take a while. It's recommended to increase the request execution timeout and the database connection timeout.</p>
			<% } %>
            <p>
				This looks fine and <em>I have back up</em> of my data. Please upgrade the database: 
				<asp:Button ID="btnUpgrade" runat="server" OnClick="btnInstallAndMigrate_Click" Text="update tables and run migratinos" OnClientClick="return confirm('Updating the database makes changes to the information on the site. I confirm that everything is backed-up and want to continue.');" ToolTip="Click this button to update the database and execute the migrations" CausesValidation="false"/>
			</p>
			<asp:Label ID="lblResult" runat="server" />
		    <script type="text/javascript">
		    	function showadvancedcontentoptions() {
		    		document.getElementById("advancedcontentoptions").style.display = "block";
		    		this.style.display = "none";
		    		return false;
		    	}
		    </script>
		    <hr />
			<p><a href="#advancedcontentoptions" onclick="return showadvancedcontentoptions.call(this);">Advanced options (includes update schema only and script download).</a></p>
			<div id="advancedcontentoptions" style="display:none;">
				<p>
					Only 
					<asp:Button ID="btnInstall" runat="server" OnClick="btnInstall_Click" Text="update tables" OnClientClick="return confirm('Updating the database makes changes to the information on the site. I confirm that everything is backed-up and want to continue.');" ToolTip="Click this button to update the database schema" CausesValidation="false"/> 
					without running migrations.
				</p>
				<p>
					<asp:Button ID="btnExport" runat="server" OnClick="btnExportSchema_Click" Text="download the SQL script" ToolTip="Click this button to generate update database script" CausesValidation="false" />
					for the connection type <strong><%= Status.ConnectionType %></strong> and manually update tables.
				</p>
				<p>
					Manually select 
					<blockquote>
						<asp:CheckBoxList ID="cblMigrations" runat="server" />
					</blockquote>
					and 
					<asp:Button ID="btnMigrate" runat="server" OnClick="btnMigrate_Click" Text="run selected migrations" OnClientClick="return confirm('Updating the database makes changes to the information on the site. I confirm that everything is backed-up and want to continue.');" ToolTip="Execute migrations against current database" CausesValidation="false"/> 
					against the current version of the database.
				</p>
            </div>
		</n2:TabPanel>
        <asp:Label EnableViewState="false" ID="errorLabel" runat="server" CssClass="errorLabel" />
    </div>
    </form>
</body>
</html>
