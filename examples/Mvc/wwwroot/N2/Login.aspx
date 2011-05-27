<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="N2.Edit.Login" Title="Login" meta:resourceKey="LoginPage" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
	<meta http-equiv="X-UA-Compatible" content="IE=edge"/>
    <title>Log in</title>
    <link rel="stylesheet" href="Resources/Css/All.css" type="text/css" />
    <style type="text/css">
        body { background:#AAAA99 url(Resources/Img/n2.png) no-repeat 98% 10px; }
        
        #login-container{
			-moz-border-radius: 1px;
			-webkit-border-radius : 1px;

			border: solid 1px #F8F8E8;
			margin: 0 0 -100px -150px;
			position:absolute;
			bottom:50%;
			left:50%;
			width:  350px;
			background: #F8F8E8 url(Resources/Img/darken10.png) repeat-x 100% 100%;
        }
        #login-container .inner	
        {
			margin:0 auto;
			padding:30px;
            background:transparent url(Resources/Img/lighten10.png) repeat-x;
        }
        table.login { margin:0 auto; }
        h1 { margin-bottom:20px; }
        .login-label { display:block; text-align:right }
        .login-button { margin-top:5px; width:100px;font-size:1.2em;float:right;}
        .login-input { width:160px;font-size:1.5em;}
        td { padding:2px; vertical-align:middle; }
    </style>
</head>
<body class="edit login">
    <form id="form1" runat="server">
		<div >
        </div>
		<div id="login-container">
		    <div class="inner">
			<asp:Login ID="Login1" TitleText="<h1>Log in</h1>" runat="server" meta:resourceKey="Login1" CssClass="login"
				MembershipProvider="AspNetSqlMembershipProvider" OnAuthenticate="Login1_Authenticate">
				<LoginButtonStyle CssClass="login-button" />
				<LabelStyle CssClass="login-label" />
				<TextBoxStyle CssClass="login-input" />
			</asp:Login></div>
		</div>
    </form>
</body>
</html>
