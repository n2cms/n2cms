<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Install.Begin.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Install N2</title>
    <link rel="stylesheet" type="text/css" href="../../css/all.css" />
    <link rel="stylesheet" type="text/css" href="../../css/framed.css" />
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
    <div>
		<ul class="tabs">
			<li class="tab selected"><a href="#">0. Prepare yourself</a></li>
			<li class="tab"><a href="../default.aspx">1-5. Continue installation</a></li>
		</ul>
		<div class="tabPanel">
			<h1>Welcome to N2 CMS</h1>
			<p>This is the first step to install the database get started with N2 CMS.</p>
			<p>To continue to the installation you will have to provide a username and password. Unless you changed it (as you should) the username is "<strong>admin</strong>" and the password is "<strong>changeme</strong>".</p>
			<p>Okay, <a href="../default.aspx">please start the installation process &raquo;</a></p>
			<p><em>Already installed the database? Then there might be a problem with the database connection. To ensure that this screen doesn't appear to unsuspecting visitors you should remove &lt;add name="n2.installer" type="N2.Installation.InstallerModule, N2" /&gt; from the &lt;httpModules&gt; section in web.config.</em></p>
		</div>
    </div>
    </form>
</body>
</html>
