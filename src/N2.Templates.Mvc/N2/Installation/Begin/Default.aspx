<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Install.Begin.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Install N2</title>
    <link rel="stylesheet" type="text/css" href="../../Resources/css/all.css" />
    <link rel="stylesheet" type="text/css" href="../../Resources/css/framed.css" />
    <link rel="stylesheet" type="text/css" href="../../Resources/css/themes/default.css" />
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
			<% Version version = typeof(N2.ContentItem).Assembly.GetName().Version; %>
			<p>Congratulations! You're well on your way to installing or upgrading <a href="http://n2cms.com/">N2 CMS</a>.</p>
			<p>Next you need to log in. Until you change your credentials (please do this now in web.config) they are:<br /> Username: <strong>admin</strong> <br /> Password: <strong>changeme</strong>.</p>
			<p>Okay, <a href="../default.aspx">please help me <strong>install</strong> a the database on a new site &raquo;</a></p>
			<p>Wait, <a href="../upgrade.aspx">I want to <strong>upgrade</strong> from a previous version &raquo;</a></p>
			<h2>Other options...</h2>
			<p><strong>Already installed?</strong> There might be a problem with the database connection. To ensure that this screen doesn't appear to unsuspecting visitors you should set &lt;n2&gt;&lt;edit&gt;&lt;installer checkInstallationStatus="false"/&gt; in web.config.</p>
		</div>
    </div>
    </form>
</body>
</html>
