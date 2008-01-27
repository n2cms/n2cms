<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Install._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Install N2</title>
    <link rel="stylesheet" type="text/css" href="../css/all.css" />
    <link rel="stylesheet" type="text/css" href="../css/framed.css" />
    <style>a{color:#00e;}li{margin-bottom:10px}form{padding:20px}.warning{color:#f00;}.ok{color:#0d0;}textarea{width:80%;height:120px}</style>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Install N2</h1>
        <asp:Literal ID="ltStartupError" runat="server" />

        <n2:TabPanel ToolTip="1. Welcome" runat="server">
            <p class='<%# Status.Installation ? "ok" : "warning" %>'>
				<b> Advice: </b> <%# GetStatusText() %>
            </p>
            <p>To install N2 you need to create a database, update the connection string in web.config, create the tables needed by N2, add the root node to the database and make sure the root node's id is configured in web.config.</p>
            <p>The following tabs will help you in these. Just continue to tab 2.</p>
            <h3>System status</h3>
            <p><%# Status.ToStatusString() %></p>
        </n2:TabPanel>
        
        <n2:TabPanel ToolTip="2. Database connection" runat="server">
            <asp:Literal runat="server" Visible='<%# Status.Connection %>'>
				<p class="ok"><b>Advice: </b>Since your database seems connected you may skip this step.</p>
            </asp:Literal>
            <p>
                First make sure you have database available and <a href="http://n2cms.com/Documentation/Connection strings.aspx">configure connection string and database dialect</a>.
            </p>
            <p>Once you're done you can test the connetion</p>
            <p>
                <asp:Button ID="btnTest" runat="server" OnClick="btnTest_Click" Text="Test connection" CausesValidation="false" />
				<br /><br /><asp:Label ID="lblStatus" runat="server" />
            </p>
        </n2:TabPanel>
        
        <n2:TabPanel ToolTip="3. Database tables" runat="server">
            <asp:Literal runat="server" Visible='<%# Status.Schema %>'>
				<p class="ok"><b>Advice: </b>The database tables are already there. You can move to the next tab (if you create them again you will delete any existing content).</p>
            </asp:Literal>
            <p>
                When the connection is OK you should create the database tables needed by N2. To do this you can either <a href="http://n2cms.com/Documentation/The database.aspx">create the tables using one of these scripts</a> or install clicking this button: 
            </p>
            <p>
                <asp:Button ID="btnInstall" runat="server" OnClick="btnInstall_Click" Text="Create tables" OnClientClick="return confirm('Creating database tables will destory any existing data. Are you sure?');" ToolTip="Click this button to install database" CausesValidation="false" />
                <br /><br /><asp:Label CssClass="ok" runat="server" ID="lblInstall" />
            </p>
        </n2:TabPanel>
        
        <n2:TabPanel ToolTip="4. Root items" runat="server">
            <asp:Literal runat="server" Visible='<%# Status.Installation %>'>
				<p class="ok"><b>Advice: </b>There are already root and start nodes. If you create more they will become detached items unless you point them out in web.config (which makes the existing nodes detached instead).</p>
            </asp:Literal>
            <p>
                N2 needs a root node and a start page in order to function correctly. These two nodes may be the same page if you don't forsee using multiple domains. This gives you the choice of:
            </p>
            <ul>
                <li>Either, Select type of item to insert as root node AND start page (select first drop down only)</li>
                <li>Or, select type of item to insert as root node, and another type to add as start page (select both drop downs)</li>
            </ul>
            <p>
            	<asp:DropDownList ID="ddlRoot" runat="server" />
			    <asp:DropDownList ID="ddlStartPage" runat="server" />
			    <asp:Button ID="btnInsert" runat="server" OnClick="btnInsert_Click" Text="Insert" ToolTip="Insert root (and start) node" CausesValidation="false" />
			    <asp:CustomValidator ID="cvRoot" runat="server" ErrorMessage="Root type required" />
            </p><p>
                Instead of inserting root and start nodes you can upload an export file to insert.
            </p><p>
                <asp:FileUpload ID="fileUpload" runat="server" />
                <asp:RequiredFieldValidator ID="rfvUpload" ControlToValidate="fileUpload" runat="server" Text="Select import file" Display="dynamic" />
                <asp:Button ID="btnUpload" runat="server" OnClick="btnUpload_Click" Text="Upload and insert" ToolTip="Upload root node." CausesValidation="false" />

                <br /><br /><asp:Literal ID="ltRootNode" runat="server" />  
            </p>
<asp:PlaceHolder ID="phSame" runat="server" Visible="false">
            <h4>Example web.config with same root as start page</h4>
            <p>
                <textarea rows="5">
...
<castle>
    <properties>
        <rootItemID><%# rootId %></rootItemID>
        <nhSettings><!-- leave these as they are --></nhSettings>
    </properties>
    <include uri="assembly://N2/Engine/n2.configuration.xml"/>
</castle>
...</textarea>
            </p>
</asp:PlaceHolder>
<asp:PlaceHolder ID="phDiffer" runat="server" Visible="false">
            <h4>Example web.config with different root as start pages</h4>
            <p>
                <textarea rows="5">
...
<castle>
    <properties>
        <nhSettings><!-- leave these as they are --></nhSettings>
    </properties>
    <include uri="assembly://N2/Engine/n2.configuration.xml"/>
    <components>
      <component id="n2.defaultSite">
        <parameters>
          <rootItemID><%# rootId %></rootItemID>
          <startPageID><%# startId %></startPageID>
        </parameters>
      </component>
    </components>
</castle>
...</textarea>
            </p>
</asp:PlaceHolder>
        </n2:TabPanel>
        
        <n2:TabPanel runat="server" tooltip="5. Finishing touches">
            <p><b>IMPORTANT!</b> Change the default password in web.config. If you've installed, configured and created an administrator account using a membership provider, comment out this section entirely.</p>
            <p><b>Advice:</b> remove the installation directory to prevent nasty surprises.</p>
            <p>Good luck and happy <a href="..">editing</a>.</p>
            <p>/Cristian</p>
        </n2:TabPanel>
    </form>
</body>
</html>
