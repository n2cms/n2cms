<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="N2.Edit.Login" Title="Login" meta:resourceKey="LoginPage" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Log in</title>
    <link rel="stylesheet" href="Css/All.css" type="text/css" />
    <style type="text/css">
        body.login { background:#c3c0bd url(img/login_bg.gif) repeat-x; }
        body.login td { text-align:left; }
        
        body.login #login-container{
			-moz-border-radius: 5px;
			-webkit-border-radius : 5px;

			background-color: #FFFFFF;
			border: solid 1px #BBB8B3;
			margin: 200px auto;
			width:  300px;
			height: 250px;
        }
        body.login .login{
        	padding-left:20px; 
        	padding-right:20px;
        }
        body.login .login-button { width:75px;font-size:1.2em;float:right;}
    </style>
</head>
<body class="edit login">
    <form id="form1" runat="server">
		<div id="login-container">
			<img src="img/login_logo.png" alt="N2 logo" title="N2 logo" style="margin-top:5px" />
			<div class="login">
				<asp:Login ID="Login1" TitleText="<h1>Log in</h1>" runat="server" meta:resourceKey="Login1"
					MembershipProvider="AspNetSqlMembershipProvider" OnAuthenticate="Login1_Authenticate">
					<LoginButtonStyle CssClass="login-button" />
				</asp:Login>
			</div>
        </div>
    </form>
</body>
</html>
