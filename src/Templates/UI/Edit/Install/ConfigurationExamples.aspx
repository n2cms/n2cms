<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="N2.Installation" %>
<script runat="server">
	protected void Page_Load(object sender, EventArgs args)
	{
		txtWebConfig.Text = InstallationManager.GetResourceString(InstallationManager.ConfigurationCompleteResourceKey);
		txtWebConfigSqlServer2005.Text = InstallationManager.GetResourceString(InstallationManager.ConfigurationSqlServer2005ResourceKey);
		txtWebConfigSqlServer2000.Text = InstallationManager.GetResourceString(InstallationManager.ConfigurationSqlServer2000ResourceKey);
		txtWebConfigMySQL.Text = InstallationManager.GetResourceString(InstallationManager.ConfigurationMySQLResourceKey);
		txtWebConfigSQLite.Text = InstallationManager.GetResourceString(InstallationManager.ConfigurationSQLiteResourceKey);
		txtWebConfigSqlServer2005Express.Text = InstallationManager.GetResourceString(InstallationManager.ConfigurationSqlServer2005ExpressResourceKey);
	}
</script>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>Configuration Examples</title>
    <style>textarea {width:99%;height:100px;border:solid 1px silver;font-size:11px}</style>
</head>
<body>
	<form runat="server">
	<h1>Configuration Examples</h1>
	<p>These are some web.config extracts displaying some database configuration options that have worked for me. The first section shows a simple N2 configuration that should be saved as web.config in the root directory of the site. The other sections display the minimal difference between different database configurations.</p>

	<h2>Complete web.config example</h2>
	<p>This configuration is for sql server 2005 express. Update it using the snippets further down for other databases.</p>
	<asp:TextBox ID="txtWebConfig" runat="server" TextMode="MultiLine" />

	<h2>SQL Server 2005</h2>
	<asp:TextBox ID="txtWebConfigSqlServer2005" runat="server" TextMode="MultiLine" />


	<h2>SQL Server 2000</h2>
	<asp:TextBox ID="txtWebConfigSqlServer2000" runat="server" TextMode="MultiLine" />


	<h2>My SQL 5</h2>
	<asp:TextBox ID="txtWebConfigMySQL" runat="server" TextMode="MultiLine" />


	<h2>SQLite 3</h2>
	<p>SQLite database "n2.db" located in the "App_Data" directory below the web root.</p>
	<asp:TextBox ID="txtWebConfigSQLite" runat="server" TextMode="MultiLine" />

	<h2>SQL Server 2005 Express Edition</h2>
	<p>SQL Express database named "N2.mdf" located in the "App_Data" directory below the web root.</p>
	<asp:TextBox ID="txtWebConfigSqlServer2005Express" runat="server" TextMode="MultiLine" />



</form>
</body>
</html>
