<%@ Page Language="C#" AutoEventWireup="true" %>
<script runat="server">
	protected void Page_Load(object sender, EventArgs args)
	{
		txtInheritGlobalAsax.Text = "<%@ Application Language=\"C#\" Inherits=\"N2.Web.Global\" %>";
	}
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>Configuration Examples</title>
    <style>textarea {width:99%;height:100px;}</style>
</head>
<body>
	<form id="Form1" runat="server">
		<h1>Troubleshooting</h1>
		<p>Some help troubleshooting common problems.</p>
		<h2>N2 isn't initialized</h2>
		<p>
			In order for N2 to run property it must be initialized each time the application starts. This can be done in one of these ways:
			<ul>
				<li>
					Create a global.asax file that inherits N2.Web.Global.
					<asp:TextBox ID="txtInheritGlobalAsax" runat="server" TextMode="MultiLine" />
				</li>
				<li>
					Configure the httpModules section in web.config:
					<textarea>
<configuration>
	<!-- ... -->
	<system.web>
		<httpModules>
			<add name="SessionCloser" type="N2.Web.CloseSessionModule,N2"/>
			<add name="RewriteUrl" type="N2.Web.UrlRewriterModule,N2"/>
		</httpModules>
		<!-- ... -->
	</system.web>
	<!-- ... -->
</configuration>
					</textarea>
				</li>
			</ul>
		</p>
		</form>
	<!--
		<ul>
		<li>
			An attempt to attach an auto-named database for file C:\...\App_Data\N2.MDF failed. A database with the same name exists, or specified file cannot be opened, or it is located on UNC share. 
		</li>
		<li>
			An OLE DB Provider was not specified in the ConnectionString. An example would be, 'Provider=SQLOLEDB;'.
		</li>
		<li>
			An error has occurred while establishing a connection to the server. When connecting to SQL Server 2005, this failure may be caused by the fact that under the default settings SQL Server does not allow remote connections. (provider: Named Pipes Provider, error: 40 - Could not open a connection to SQL Server) 
		</li>
		<li>
			[DBNETLIB][ConnectionOpen (Connect()).]SQL Server does not exist or access denied. 
		</li>
		<li>
			ERROR [IM002] [Microsoft][ODBC Driver Manager] Data source name not found and no default driver specified 
		</li>
		</ul>
		-->
</body>
</html>
