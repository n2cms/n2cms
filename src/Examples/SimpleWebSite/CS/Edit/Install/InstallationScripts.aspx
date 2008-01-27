<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="N2.Installation" %>
<script runat="server">
	protected void Page_Load(object sender, EventArgs args)
	{
		txtSqlServer2005.Text = InstallationManager.GetResourceString(InstallationManager.CreateSqlServer2005ResourceKey);
		txtSqlServer2000.Text = InstallationManager.GetResourceString(InstallationManager.CreateSqlServer2000ResourceKey);
		txtMySql5.Text = InstallationManager.GetResourceString(InstallationManager.CreateMySQLResourceKey);
		txtSQLite3.Text = InstallationManager.GetResourceString(InstallationManager.CreateSQLiteResourceKey);
	}
</script>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>Installation Scripts</title>
    <style>textarea {width:99%;height:100px;}</style>
</head>
<body>
	<form runat="server">
		<h1>Configuration Examples</h1>
		<p>These are some installation scripts I've tried with N2.</p>

		<h2>SQL Server 2005 (and Express)</h2>
		<asp:TextBox ID="txtSqlServer2005" runat="server" TextMode="MultiLine" />

		<h2>SQL Server 2000</h2>
		<asp:TextBox ID="txtSqlServer2000" runat="server" TextMode="MultiLine" />

		<h2>My SQL 5</h2>
		<asp:TextBox ID="txtMySql5" runat="server" TextMode="MultiLine" />

		<h2>SQLite 3</h2>
		<asp:TextBox ID="txtSQLite3" runat="server" TextMode="MultiLine" />

	</form>
</body>
</html>
