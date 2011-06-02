<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Install.Begin.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title>Install N2</title>
	<link rel="stylesheet" type="text/css" href="../../Resources/Css/all.css" />
	<link rel="stylesheet" type="text/css" href="../../Resources/Css/framed.css" />
	<link rel="stylesheet" type="text/css" href="../../Resources/Css/themes/default.css" />
	<style>
		form{font-size:1.1em;width:800px;margin:10px auto;}
		a{color:#00e;}
		li{margin-bottom:10px}
		form{padding:20px}
		.warning{color:#f00;}
		.ok{color:#0d0;}
		textarea{width:80%;height:120px}
		label { width:120px; display:inline-block; }
	</style>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<ul class="tabs">
			<li class="tab selected"><a href="#">0. Prepare yourself</a></li>
			<li class="tab"><a href="<%= continueUrl %>">1-3. Continue installation</a></li>
		</ul>
		<div class="tabPanel">
			<asp:CustomValidator ID="cvSave" runat="server" />
			<% if (needsPasswordChange && !cannotChangePassword) { %>
				<h1>Welcome to N2 CMS</h1>
				<p>Please give a new password for the user <strong>admin</strong>.</p>
				<p><asp:Label Text="Password" AssociatedControlID="txtPassword" runat="server" /><asp:TextBox TextMode="Password" ID="txtPassword" runat="server" /><asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtPassword" runat="server" /></p>
				<p><asp:Label Text="Repeat Password" AssociatedControlID="txtPassword" runat="server" /><asp:TextBox TextMode="Password" ID="txtRepeatPassword" runat="server" /><asp:RequiredFieldValidator ControlToValidate="txtRepeatPassword" runat="server" /><asp:CompareValidator ControlToValidate="txtRepeatPassword" ControlToCompare="txtPassword" runat="server" /></p>
				<p><asp:Button runat="server" Text="OK" OnCommand="OkCommand" /></p>
			<%} else if (action == "install"){%>
				<h1>Welcome to N2 CMS Installation</h1>
				<p>Congratulations! You're well on your way to the <a href="http://n2cms.com/">N2 CMS</a> experience.</p>
				<% if (cannotChangePassword) { %>
				<p>
					To continue you need to log in with the username <strong>admin</strong> and the password <strong>changeme</strong>. 
					Your configuration does not allow changing this password here, you must <strong>change this password manually</strong> in web.config.
				</p>
				<% } else { %>
				<p>To continue you need to log in with the username <strong>admin</strong> and the password you specified during installation.</p>
				<% } %>
				<p>Okay, <a href="<%= continueUrl %>">please help me <strong>install</strong> a the database for a new site &raquo;</a></p>
			<%} else if(action == "upgrade") {%>
				<h1>Welcome to N2 CMS Upgrade to <%= version %></h1>
				<% if (cannotChangePassword) { %>
				<p>
					To continue you need to log in with the username <strong>admin</strong> and the password <strong>changeme</strong>. 
					Your configuration does not allow changing this password here, you must <strong>change this password manually</strong> in web.config.
				</p>
				<% } else { %>
				<p>To continue you need to log in with the username <strong>admin</strong> and the password you specified during installation.</p>
				<% } %>
				<p><a href="<%= continueUrl %>">I want to <strong>upgrade</strong> from a previous version &raquo;</a></p>
			<%} else if (action == "rebase") {%>
				<h1>Welcome to N2 CMS Rebase Tool</h1>
				<% if (cannotChangePassword) { %>
				<p>
					To continue you need to log in with the username <strong>admin</strong> and the password <strong>changeme</strong>. 
					Your configuration does not allow changing this password here, you must <strong>change this password manually</strong> in web.config.
				</p>
				<% } else { %>
				<p>To continue you need to log in with the username <strong>admin</strong> and the password you specified during installation.</p>
				<% } %>
				<p><a href="<%= continueUrl %>"><strong>Rebase</strong> from a previous virtual directory &raquo;</a></p>
			<%} else {%>
				<h1>Welcome to N2 CMS</h1>
				<p>What do you want to do with <a href="http://n2cms.com/">N2 CMS</a>?</p>
				<p><a href="<%= N2.Web.Url.ResolveTokens(config.InstallUrl) %>"><strong>Install</strong> a the database for a new site &raquo;</a></p>
				<p><a href="<%= N2.Web.Url.ResolveTokens(config.UpgradeUrl) %>"><strong>Upgrade</strong> from a previous version &raquo;</a></p>
				<p><a href="<%= N2.Web.Url.ResolveTokens(config.RebaseUrl) %>"><strong>Rebase</strong> links from another virtual directory &raquo;</a></p>
			<%}%>

			<p><strong>Already done this?</strong> There might be a problem with the database connection. To ensure that this screen doesn't appear to unsuspecting visitors you should set &lt;n2&gt;&lt;edit&gt;&lt;installer checkInstallationStatus="false"/&gt; in web.config.</p>
		</div>
	</div>
	</form>
</body>
</html>
