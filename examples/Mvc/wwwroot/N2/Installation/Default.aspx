<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Install._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Install N2</title>
    <link rel="stylesheet" type="text/css" href="../Resources/Css/all.css" />
    <link rel="stylesheet" type="text/css" href="../Resources/Css/framed.css" />
    <link rel="stylesheet" type="text/css" href="../Resources/Css/themes/default.css" />
    <style>
    	form{font-size:1.1em;width:800px;margin:10px auto;}
    	a{color:#00e;}
    	li{margin-bottom:10px}
    	form{padding:20px}
    	.warning{color:#f00;}
    	.ok{color:#0c0;}
    	.buttons { text-align:right; }
    	textarea{width:95%;height:120px; border:none; background-color:#FFB}
    	pre { overflow:auto; font-size:10px; color:Gray; }
    </style>
	<script type="text/javascript">
		function show(id) {
			var el = document.getElementById(id);
			if (!el) return;
			el.style.display = "block";
			this.style.display = "none";
			return false;
		}
	</script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Literal ID="ltStartupError" runat="server" />
        
        <n2:TabPanel ID="Database" ToolTip="1. Database Connection & Tables" runat="server">
			<asp:PlaceHolder runat="server" Visible='<%# Status.HasSchema %>'>
				<p class="ok"><b>Advice: </b> <%= GetStatusText() %></p>
			</asp:PlaceHolder>
			<asp:Panel runat="server" Visible="<%# !Status.IsConnected %>">
				<h1>Check database connection</h1>
				<p>
					Make sure your database is online and <a href="http://n2cms.com/Documentation/Connection strings.aspx">configure connection string and database dialect</a> in web.config.
				</p>
				<p class="buttons"><asp:Button ID="btnTest" runat="server" OnClick="btnTest_Click" Text="Test the connection" CausesValidation="false" /></p>
				<p>
					<asp:Label ID="lblStatus" runat="server" />
				</p>
				<% if (Status.ConnectionType == "SqlCeConnection" && !Status.IsConnected){ %>
				<p class="buttons"><asp:Button ID="btnCreateSqlCe" runat="server" OnClick="btnCreateSqlCeFile_Click" Text="Create SqlCe database file" CausesValidation="false" /></p>
				<% } %>
            </asp:Panel>
            <asp:Panel ID="Panel1" runat="server" Visible="<%# Status.IsConnected %>">
				<% if (Status.HasSchema){ %>
				<p><a href="#createschema" onclick="return show.call(this, 'createschema');">Re-create database tables.</a></p>
			    <% } %>

				<div id="createschema" style='display: <%= Status.HasSchema ? "none" : "block"%>'>
					<h1>Create datbase tables</h1>
					<asp:Literal runat="server" Visible='<%# !Status.IsConnected %>'>
						<p class="warning"><b>Advice: </b>Go back and check database connection. </p>
					</asp:Literal>
					<p>
						Please review the following SQL script carefully before creating tables.
						<asp:Literal ID="Literal1" runat="server" Visible="<%# Status.HasSchema %>">
							<span class="warning">Creating tables will destroy any existing content.</span>
						</asp:Literal>
					</p>
					<textarea readonly="readonly"><%= Installer.ExportSchema() %></textarea>
					<p class="buttons"><asp:Button ID="btnInstall" runat="server" OnClick="btnInstall_Click" Text="Create tables" OnClientClick="return confirm('Creating database tables will destroy any existing data. Are you sure?');" ToolTip="Click this button to install database" CausesValidation="false"/></p>
				</div>
				<hr />
				<p>
					Optionally I can 
					<asp:LinkButton ID="btnExport" runat="server" OnClick="btnExportSchema_Click" Text="download the SQL script" ToolTip="Click this button to generate create database schema script" CausesValidation="false" />
					for the connection type <strong><%= Status.ConnectionType %></strong> and create the tables myself.
				</p>
				<%--<p>
					You can also download sql scripts from 
					<a href="http://n2cms.com/Documentation/The database.aspx">the n2 cms home page</a> 
					(if you're using MySQL this is the preferred option).
				</p>--%>
				<p>
					<asp:Label CssClass="ok" runat="server" ID="lblInstall" />
				</p>
			</asp:Panel>
        </n2:TabPanel>
        <n2:TabPanel ID="Content" ToolTip="2. Content Package" runat="server">				
            <asp:Literal runat="server" Visible='<%# Status.IsInstalled %>'>
				<p class="ok">
				    <b>Advice: </b> Proceed to <a href="#Finish">step 3</a>
				    There is content present in the database. 
				    If you add more the old content remain but only one root can be used per site.
				</p>
            </asp:Literal>
            <asp:Literal runat="server" Visible='<%# !Status.HasSchema %>'>
				<p class="warning"><b>Advice: </b>Go back to <a href='#Database'>step 1</a> and check database connection and tables.</p>
            </asp:Literal>

			<h1>Add Content Package</h1>
            <asp:PlaceHolder ID="plhAddContent" runat="server">
                <div  style="display:<%= rblExports.Items.Count == 0 ? "none" : "block" %>">
					<p>
						Pick the <b>content package</b> that tickle your fancy and import it into your site.
					</p>
					<div class="exports">
						<asp:RadioButtonList ID="rblExports" runat="server" RepeatLayout="Table" RepeatColumns="2" />
					</div>
					<p class="buttons">
						<asp:Button ID="btnInsertExport" runat="server" OnClick="btnInsertExport_Click" Text="Please import this" ToolTip="Insert existing package" CausesValidation="false" />
						<asp:CustomValidator ID="cvExisting" runat="server" ErrorMessage="Select an export file" Display="Dynamic" />
					</p>
					<hr />
					<p><a href="#advancedcontentoptions" onclick="return show.call(this, 'advancedcontentoptions');">Advanced options (includes upload and manual insert).</a></p>
			    </div>
			    <div id="advancedcontentoptions" style="display:<%= rblExports.Items.Count > 0 ? "none" : "block" %>">
					<p>
						N2 CMS needs content in the database to function correctly.
						The minimum required is a <a href="http://n2cms.com/wiki/Root-node.aspx">root node</a> and a <a href="http://n2cms.com/wiki/Start-Page.aspx">start page</a>.
					</p>
			        <h2>Upload and import package</h2>
			        <p>Select an export file you may have exported from another site and saved to disk to import on this installation.</p>
			        <p>Package: <asp:FileUpload ID="fileUpload" runat="server" />(*.n2.xml)</p>
			        <p class="buttons">
			            <asp:Button ID="btnUpload" runat="server" OnClick="btnUpload_Click" Text="Upload and add" ToolTip="Upload root node." CausesValidation="false" />
			            <asp:RequiredFieldValidator ID="rfvUpload" ControlToValidate="fileUpload" runat="server" Text="Select import file" Display="Dynamic" EnableClientScript="false" />
			        </p>
			        <h2>Manually insert nodes</h2>
			        <p>
			            Separate <asp:DropDownList ID="ddlRoot" runat="server" /> 
			            and <asp:DropDownList ID="ddlStartPage" runat="server" /> 
			            to insert as <b>two different</b> nodes (preferred)
			        </p>
			        <p class="buttons">
			            <asp:Button ID="btnInsert" runat="server" OnClick="btnInsert_Click" Text="Add two separate nodes" ToolTip="Insert different root and start nodes" CausesValidation="false" /> 
			            <asp:CustomValidator ID="cvRootAndStart" runat="server" ErrorMessage="Root and start type required" Display="Dynamic" />
					</p>
			        <p>
			            <b>Same node</b> for both <asp:DropDownList ID="ddlRootAndStart" runat="server" /> to insert (simple site).
			        </p>
			        <p class="buttons">
				        <asp:Button ID="btnInsertRootOnly" runat="server" OnClick="btnInsertRootOnly_Click" Text="Add a single node" ToolTip="Insert one node as root and start" CausesValidation="false" />
				        <asp:CustomValidator ID="cvRoot" runat="server" ErrorMessage="Root type required" Display="Dynamic" />
                    </p>
                </div>
            </asp:PlaceHolder>
			<p>
                <asp:Literal ID="ltRootNode" runat="server" />  
            </p>
<asp:PlaceHolder ID="phSame" runat="server" Visible="false">
            <h4>Example web.config with same root as start page</h4>
            <p>
                <textarea rows="4">
...
	<n2>
		<host rootID="<%# RootId %>" startPageID="<%# StartId %>"/>
		...
	</n2>
...</textarea>
				Please help me <asp:Button runat="server" OnClick="btnUpdateWebConfig_Click" Text="update web.config" CausesValidation="false" /> by writing these values to web.config (will cause app restart).
            </p>
</asp:PlaceHolder>
<asp:PlaceHolder ID="phDiffer" runat="server" Visible="false">
            <h4>Example web.config with different root as start pages</h4>
            <p>
                <textarea rows="4">
...
	<n2>
		<host rootID="<%# RootId %>" startPageID="<%# StartId %>"/>
		...
	</n2>
...</textarea>
				<asp:Button runat="server" OnClick="btnUpdateWebConfig_Click" Text="Update web.config" />
            </p>
</asp:PlaceHolder>
			<p><asp:Label runat="server" ID="lblWebConfigUpdated" /></p>
        </n2:TabPanel>
        
        <n2:TabPanel ID="Finish" runat="server" tooltip="3. Finishing touches">
			<asp:PlaceHolder runat="server" Visible='<%# !Status.IsInstalled %>'>
				<p class='warning'>
					<b> Advice: </b> <%# GetStatusText() %>
				</p>
			</asp:PlaceHolder>
			<h1>Almost done!</h1>
			<asp:PlaceHolder runat="server" Visible="<%# IsDefaultPassword() %>">
            <p><b>IMPORTANT!</b> Change the default password in web.config. Once you've created a new administrator user using the management interface, comment out the credentials configuration section entirely.</p>
            </asp:PlaceHolder>
            <p><b>Advice:</b> remove the installation wizard located below /n2/installation/ to prevent nasty surprises.</p>
            <p>Please 
				<asp:Button runat="server" OnClick="btnRestart_Click" Text="restart" CausesValidation="false" />
				before you continue.
            </p>
            <p>Good luck and happy <a href="..">managing</a>!</p>
            <p>/Cristian</p>
        </n2:TabPanel>
        <pre><asp:Label EnableViewState="false" ID="errorLabel" runat="server" CssClass="errorLabel" /></pre>
        <p style="color:#999"><%# Status.ToStatusString() %></p>
    </form>
</body>
</html>
